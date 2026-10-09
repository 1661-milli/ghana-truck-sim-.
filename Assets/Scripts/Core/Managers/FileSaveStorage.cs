using System.IO;
using UnityEngine;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Stores save slots as JSON files under Application.persistentDataPath.
    /// Writes go to a temporary file first and are then moved into place.
    /// </summary>
    public sealed class FileSaveStorage : ILocalSaveStorage
    {
        private const string Extension = ".json";
        private const string TempExtension = ".tmp";

        private readonly string _directory;

        public FileSaveStorage(string directory = null)
        {
            _directory = string.IsNullOrEmpty(directory) ? Application.persistentDataPath : directory;
        }

        public bool Exists(string slotName) => File.Exists(PathFor(slotName));

        public void Write(string slotName, string json)
        {
            Directory.CreateDirectory(_directory);

            string finalPath = PathFor(slotName);
            string tempPath = finalPath + TempExtension;

            File.WriteAllText(tempPath, json);
            if (File.Exists(finalPath))
            {
                File.Replace(tempPath, finalPath, null);
            }
            else
            {
                File.Move(tempPath, finalPath);
            }
        }

        public string Read(string slotName)
        {
            string path = PathFor(slotName);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public void Delete(string slotName)
        {
            string path = PathFor(slotName);
            if (File.Exists(path)) File.Delete(path);
        }

        private string PathFor(string slotName) => Path.Combine(_directory, slotName + Extension);
    }
}
