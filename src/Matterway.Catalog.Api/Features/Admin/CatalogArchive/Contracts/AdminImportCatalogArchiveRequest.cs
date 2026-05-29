using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.CatalogArchive.Contracts;

public record AdminImportCatalogArchiveRequest
{
    [Required]
    public IFormFile? File { get; init; }
}
