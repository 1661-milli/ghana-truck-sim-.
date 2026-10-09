namespace TrotroNation.Core.Interfaces
{
    /// <summary>
    /// Contract for objects whose state can be persisted locally or to cloud storage.
    /// State is exchanged as JSON so the same payload works for both backends.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>Unique key under which this object's state is stored.</summary>
        string SaveKey { get; }

        /// <summary>Schema version of the serialized payload, for future migrations.</summary>
        int SaveVersion { get; }

        /// <summary>Serializes current state to a JSON string.</summary>
        string CaptureState();

        /// <summary>Restores state from a JSON string previously produced by CaptureState.</summary>
        /// <param name="json">The serialized payload.</param>
        /// <param name="savedVersion">The SaveVersion that produced the payload.</param>
        /// <returns>True if the payload was applied successfully.</returns>
        bool RestoreState(string json, int savedVersion);
    }
}
