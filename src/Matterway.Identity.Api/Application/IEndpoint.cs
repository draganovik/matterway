using JetBrains.Annotations;

namespace Matterway.Identity.Api.Application;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}