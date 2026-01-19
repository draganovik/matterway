using JetBrains.Annotations;

namespace Matterway.Sales.Api.Application;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
