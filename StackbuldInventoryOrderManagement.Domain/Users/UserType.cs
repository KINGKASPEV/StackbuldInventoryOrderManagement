using System.Runtime.Serialization;

namespace StackbuldInventoryOrderManagement.Domain.Users
{
    public enum UserType
    {
        [EnumMember(Value = "Customer")]
        Customer = 1,

        [EnumMember(Value = "Admin")]
        Admin = 2,
    }
}
