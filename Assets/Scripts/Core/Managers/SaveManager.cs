using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrotroNation.Core.Interfaces;
using UnityEngine;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Collects <see cref="ISaveable"/> objects and persists them as a single JSON envelope.
    /// Local JSON is the source of truth; the cloud provider is optional and receives the same payload,
    /// so both backends stay in sync with one format.
    /// </summary>
    public sealed class SaveManager : MonoSingleton<SaveManager>
    {
        public const int EnvelopeFormatVersion = 1;

        [SerializeField] private string _slotName = "savegame";

        private readonly Dictionary<string, ISaveable> _saveables = new Dictionary<string, ISaveable>();

        private ILocalSaveStorage _localStorage;
        private ICloudSaveProvider _cloudProvider = new NullCloudSaveProvider();

        /// <summary>Raised after a successful local save or load: (slotName, isLoad).</summary>
        public event Action<string, bool> SaveCompleted;

        public string SlotName => _slotName;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            if (_localStorage == null) _localStorage = new FileSaveStorage();
        }

        /// <summary>Replaces the local backend (e.g. in-memory storage for tests).</summary>
        public void SetLocalStorage(ILocalSaveStorage storage) => _localStorage = storage;

        /// <summary>Configures the optional cloud backend. Pass null to disable.</summary>
        public void SetCloudProvider(ICloudSaveProvider provider) =>
            _cloudProvider = provider ?? new NullCloudSaveProvider();

        public void Register(ISaveable saveable)
        {
            if (saveable == null) throw new ArgumentNullException(nameof(saveable));
            if (string.IsNullOrEmpty(saveable.SaveKey))
                throw new ArgumentException("SaveKey must be non-empty.", nameof(saveable));

            _saveables[saveable.SaveKey] = saveable;
        }

        public void Unregister(ISaveable saveable)
        {
            if (saveable == null) return;
            if (_saveables.TryGetValue(saveable.SaveKey, out ISaveable existing) && existing == saveable)
            {
                _saveables.Remove(saveable.SaveKey);
            }
        }

        /// <summary>Captures all registered state and writes it to local storage.</summary>
        public bool SaveLocal()
        {
            try
            {
                string json = BuildEnvelopeJson();
                _localStorage.Write(_slotName, json);
                SaveCompleted?.Invoke(_slotName, false);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Local save failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Loads the local slot and restores every registered object found in it.</summary>
        public bool LoadLocal()
        {
            string json = _localStorage.Read(_slotName);
            return ApplyEnvelopeJson(json);
        }

        /// <summary>Builds the current payload and uploads it to the cloud, if available.</summary>
        public async Task<bool> SaveToCloudAsync()
        {
            if (!_cloudProvider.IsAvailable) return false;

            try
            {
                await _cloudProvider.UploadAsync(_slotName, BuildEnvelopeJson());
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Cloud save failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Downloads the cloud slot and restores registered objects from it.</summary>
        public async Task<bool> LoadFromCloudAsync()
        {
            if (!_cloudProvider.IsAvailable) return false;

            try
            {
                string json = await _cloudProvider.DownloadAsync(_slotName);
                return ApplyEnvelopeJson(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Cloud load failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Returns the JSON envelope that would be written right now (useful for debugging and previews).</summary>
        public string BuildEnvelopeJson()
        {
            var envelope = new SaveEnvelope
            {
                formatVersion = EnvelopeFormatVersion,
                savedAtUtc = DateTime.UtcNow.ToString("o"),
                entries = new List<SaveEntry>(_saveables.Count)
            };

            foreach (KeyValuePair<string, ISaveable> pair in _saveables)
            {
                envelope.entries.Add(new SaveEntry
                {
                    key = pair.Key,
                    version = pair.Value.SaveVersion,
                    json = pair.Value.CaptureState()
                });
            }

            return JsonUtility.ToJson(envelope, prettyPrint: true);
        }

        private bool ApplyEnvelopeJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;

            SaveEnvelope envelope;
            try
            {
                envelope = JsonUtility.FromJson<SaveEnvelope>(json);
            }
            catch (ArgumentException ex)
            {
                Debug.LogError($"[SaveManager] Corrupt save payload: {ex.Message}");
                return false;
            }

            if (envelope == null || envelope.entries == null) return false;
            if (envelope.formatVersion > EnvelopeFormatVersion)
            {
                Debug.LogWarning("[SaveManager] Save was written by a newer format; refusing to load.");
                return false;
            }

            bool allApplied = true;
            foreach (SaveEntry entry in envelope.entries)
            {
                if (entry == null || !_saveables.TryGetValue(entry.key, out ISaveable target))
                {
                    continue;
                }

                if (!target.RestoreState(entry.json, entry.version))
                {
                    Debug.LogWarning($"[SaveManager] Failed to restore '{entry.key}'.");
                    allApplied = false;
                }
            }

            SaveCompleted?.Invoke(_slotName, true);
            return allApplied;
        }

        [Serializable]
        public sealed class SaveEnvelope
        {
            public int formatVersion;
            public string savedAtUtc;
            public List<SaveEntry> entries;
        }

        [Serializable]
        public sealed class SaveEntry
        {
            public string key;
            public int version;
            public string json;
        }
    }
}
