using System.Runtime.Serialization;

namespace Matterway.Common.Enums;

public enum SystemUserRole
{
    [EnumMember(Value = "Admin")]
    Admin = 0,

    [EnumMember(Value = "Manager")]
    Manager = 1,

    [EnumMember(Value = "Customer")]
    Customer = 2
}