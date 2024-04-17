using WebApplication2.Models.DTOs;

namespace WebApplication2.Services.Interfaces
{
    public interface IPhotoManager
    {
        Task<PhotoResult> UploadImage(IFormFile file);
        Task<bool> DeleteImage(string publicId);
    }
}
