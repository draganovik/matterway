using Matterway.Identity.Api.Domain;

namespace Matterway.Identity.Api.Infrastructure.Persistence.SystemUserEntity;

public interface ISystemUserRepository
{
    Task<int> Count(EIdentityRole? roleFilter, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemUserRepositoryModel>> Query(
        int pageIndex,
        int pageSize,
        EIdentityRole? roleFilter,
        CancellationToken cancellationToken = default);

    Task<SystemUserRepositoryModel?> GetById(Guid id, CancellationToken cancellationToken = default);
}

public sealed record SystemUserRepositoryModel
{
    public required Guid Id { get; init; }

    public string? Email { get; init; }

    public required DateTime Created { get; init; }

    public required EIdentityRole Role { get; init; }
}