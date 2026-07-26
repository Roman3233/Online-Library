using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace API.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

      public async Task<string> UploadImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "covers",
            AccessMode = "public" 
        };
        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
        {
            throw new Exception($"Cloudinary image upload failed: {result.Error.Message}");
        }
        if (result.SecureUrl == null)
        {
            throw new Exception("Cloudinary image upload failed: SecureUrl is null");
        }
        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadPdfAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "books",
            AccessMode = "public" 
        };
        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
        {
            throw new Exception($"Cloudinary PDF upload failed: {result.Error.Message}");
        }
        if (result.SecureUrl == null)
        {
            throw new Exception("Cloudinary PDF upload failed: SecureUrl is null");
        }
        return result.SecureUrl.ToString();
    }

    public async Task DeleteFileAsync(string url, bool isRaw)
    {
        string publicId = GetPublicIdFromUrl(url, isRaw);
        if (string.IsNullOrEmpty(publicId)) return;

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = isRaw ? ResourceType.Raw : ResourceType.Image
        };

        var result = await _cloudinary.DestroyAsync(deletionParams);
        if (result.Error != null)
        {
            throw new Exception($"Cloudinary deletion failed: {result.Error.Message}");
        }
    }

    private string GetPublicIdFromUrl(string url, bool isRaw)
    {
        var uploadIndex = url.IndexOf("/upload/");
        if (uploadIndex == -1) return string.Empty;

        var pathAfterUpload = url.Substring(uploadIndex + 8);
        var firstSlashIndex = pathAfterUpload.IndexOf('/');
        if (firstSlashIndex == -1) return string.Empty;

        var publicIdWithExtension = pathAfterUpload.Substring(firstSlashIndex + 1);

        if (isRaw)
        {
            return publicIdWithExtension;
        }
        else
        {
            var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                return publicIdWithExtension.Substring(0, lastDotIndex);
            }
            return publicIdWithExtension;
        }
    }
}