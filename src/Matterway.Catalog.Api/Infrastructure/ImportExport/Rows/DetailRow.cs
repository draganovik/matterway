using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record DetailRow(string Slug, string Title, string? Unit)
{
    public static DetailRow FromEntity(Detail entity)
    {
        return new DetailRow(entity.Slug, entity.Title, entity.Unit);
    }

    public Detail ToEntity()
    {
        return new Detail
        {
            Slug = Slug,
            Title = Title,
            Unit = Unit
        };
    }
}