using System.Net;
using System.Runtime.Serialization;

namespace StackbuldInventoryOrderManagement.Common.CustomException
{
    [Serializable]
    public class ApplicationException : Exception, ISerializable
    {
        public HttpStatusCode StatusCode { get; }

        public ApplicationException() { }

        public ApplicationException(string message)
            : base(message) { }

        public ApplicationException(string message, Exception inner)
            : base(message, inner) { }

        public ApplicationException(string message, HttpStatusCode statusCode)
            : this(message)
        {
            StatusCode = statusCode;
        }

        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusCode = (HttpStatusCode)info.GetInt32("StatusCode");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StatusCode", (int)StatusCode);
        }
    }
}
