using CloudinaryDotNet.Actions;

namespace WebApiPropiedades.Interface
{
    public interface IPhotoService
    {
        Task<UploadResult> AddPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
