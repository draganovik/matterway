using JetBrains.Annotations;

namespace Matterway.Customers.Api.Application;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}