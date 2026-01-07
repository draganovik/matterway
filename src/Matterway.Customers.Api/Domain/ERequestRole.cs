using System.Runtime.Serialization;

namespace Matterway.Customers.Api.Domain;

public enum ERequestRole
{
    [EnumMember(Value = "Admin")]
    Admin = 0,

    [EnumMember(Value = "Manager")]
    Manager = 1,

    [EnumMember(Value = "Customer")]
    Customer = 2
}