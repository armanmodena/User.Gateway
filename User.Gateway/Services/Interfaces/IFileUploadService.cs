using Microsoft.AspNetCore.Http;

namespace User.Gateway.Services.Interfaces
{
    public interface IFileUploadService
    {
        object uploadImage(IFormFileCollection Files, string index);

        object uploadMultipleImage(IFormFileCollection Files);

        string genImageName(IFormFileCollection Files, string index);

        string saveImage(IFormFileCollection Files, string index, string fileName);

        string saveFile(IFormFileCollection Files, string index, string fileName);

        (bool, string) deleteImage(string fileName);
        (bool, string) deleteFile(string fileName);
    }
}
