using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace dotnet
{
    public class CustomException<T> : Exception where T : Exception
    {
        public CustomException() { }
        public CustomException(string message) : base(message) { }
        public CustomException(string message, Exception innerException) : base(message, innerException) { }
        public CustomException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}