using System.Threading.Tasks;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Cloud persistence backend (e.g. Unity Cloud Save). Kept as an interface so the
    /// game compiles without the cloud SDK; plug in an adapter when the package is added.
    /// </summary>
    public interface ICloudSaveProvider
    {
        /// <summary>True when the provider is signed in and reachable.</summary>
        bool IsAvailable { get; }

        Task UploadAsync(string slotName, string json);

        /// <summary>Returns the cloud payload, or null if none exists.</summary>
        Task<string> DownloadAsync(string slotName);
    }
}
