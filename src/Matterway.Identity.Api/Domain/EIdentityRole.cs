using System.Runtime.Serialization;

namespace Matterway.Identity.Api.Domain;

public enum EIdentityRole
{
    [EnumMember(Value = "Admin")]
    Admin = 0,

    [EnumMember(Value = "Manager")]
    Manager = 1,

    [EnumMember(Value = "Customer")]
    Customer = 2
}