namespace StackbuldInventoryOrderManagement.Common.Utilities
{
    public static class Constants
    {
        public const string ExceptionCommonWords =
       "unexpected characted,unexpected end,object reference,exception occured,sqlexception,ioexception,runtimeexception,index out of bound, index out of range";

        public const string DefaultExceptionFriendlyMessage =
            "Unable to process your request at the moment, please try again later!";
        public const string SuccessMessage = "Successful";
        public const string FailedMessage = "Operation can't not be completed";
        public const string DuplicateMessage = "Duplicate record found";
        public const string NotFoundMessage = "User not found";
        public const string InvalidCredentials = "Invalid Credential";
        public const string AccountNotActive = "Account is not active.";
        public const string LoginFailure = "Invalid credentials supplied";
        public const string UserDuplicateMessage = "User already exists";
        public const string OnlyCustomer = "This is for customers sign up only.";
        public const string CustomerNotFound = "Customer not found";
        public const string ConcurentUpdate = "Unable to complete order due to concurrent updates. Please try again.";
    }
}
