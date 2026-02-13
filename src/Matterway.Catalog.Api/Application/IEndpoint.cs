using JetBrains.Annotations;

namespace Matterway.Catalog.Api.Application;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    void MapEndpoint(EndpointRouter endpoints);
}