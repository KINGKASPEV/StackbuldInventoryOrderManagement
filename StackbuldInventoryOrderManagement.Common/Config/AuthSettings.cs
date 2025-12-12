namespace StackbuldInventoryOrderManagement.Common.Config
{
    public class AuthSettings
    {
        public string SecretKey { get; set; } = String.Empty;

        public string Issuer { get; set; } = String.Empty;

        public string Audience { get; set; } = String.Empty;

        public int TokenLifeTimeInHours { get; set; }

        public int TokenLifeTimeDays { get; set; }
       
    }

}
