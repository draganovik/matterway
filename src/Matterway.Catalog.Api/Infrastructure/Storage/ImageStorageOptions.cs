using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Infrastructure.Storage;

public class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    [Required]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string Bucket { get; set; } = string.Empty;

    [Required]
    public string AccessKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    public string? PublicBaseUrl { get; set; }

    public string? Region { get; set; }

    public bool AllowPublicRead { get; set; }
}