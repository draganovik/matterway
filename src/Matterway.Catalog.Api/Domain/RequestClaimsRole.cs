using System.Runtime.Serialization;

namespace Matterway.Catalog.Api.Domain;

public enum RequestClaimsRole
{
    [EnumMember(Value = "Admin")]
    Admin = 0,

    [EnumMember(Value = "Manager")]
    Manager = 1,

    [EnumMember(Value = "Customer")]
    Customer = 2
}