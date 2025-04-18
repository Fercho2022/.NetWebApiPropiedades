﻿


using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using WebApiPropiedades.Helpers;
using WebApiPropiedades.Interface;

namespace WebApiPropiedades.Services
{


    public class PhotoService : IPhotoService

    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IConfiguration configuration)
        {
            var cloudinaryConfig = configuration.GetSection("Cloudinary");

            Account account = new Account(
                cloudinaryConfig["CloudName"],
                cloudinaryConfig["ApiKey"],
                cloudinaryConfig["ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<UploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation()
                        .Height(500).Width(500)
                        .Crop("fill")
                        .Gravity("face"),
                    Folder = "properties"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;

        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}