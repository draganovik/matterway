using System.Runtime.Serialization;

namespace Matterway.Identity.Api.Domain;

public enum EIdentityRole
{
    [EnumMember(Value = "Customer")]
    Customer = 0,

    [EnumMember(Value = "Employee")]
    Employee = 1
}