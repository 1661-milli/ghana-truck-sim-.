namespace TrotroNation.Core.Managers
{
    /// <summary>Local persistence backend for save payloads (JSON text).</summary>
    public interface ILocalSaveStorage
    {
        bool Exists(string slotName);

        /// <summary>Writes the payload atomically so a crash never leaves a half-written file.</summary>
        void Write(string slotName, string json);

        /// <summary>Returns the stored payload, or null if the slot does not exist.</summary>
        string Read(string slotName);

        void Delete(string slotName);
    }
}
