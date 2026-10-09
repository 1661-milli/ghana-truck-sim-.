using System.Threading.Tasks;

namespace TrotroNation.Core.Managers
{
    /// <summary>Cloud provider used when no cloud backend is configured. Always unavailable.</summary>
    public sealed class NullCloudSaveProvider : ICloudSaveProvider
    {
        public bool IsAvailable => false;

        public Task UploadAsync(string slotName, string json) => Task.CompletedTask;

        public Task<string> DownloadAsync(string slotName) => Task.FromResult<string>(null);
    }
}
