namespace Matterway.Common.Models;

public class ProductImage
{
    public Guid Id { get; set; }
    public int OrderIndex { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
}
