using Microsoft.AspNetCore.Components.Forms;

namespace GrooveBoxDesktop.Helpers;
public interface IFileProcessor
{
    Task<string> CompressAndStoreImageAsync(IBrowserFile imageFile);
}