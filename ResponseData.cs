using System.Runtime.Serialization;

namespace TestApp
{
    [DataContract ]    
    public class ResponseData
    {
        [DataMember]
        public int  AccountNumber { get; set; }
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class ResponseBalance
    {
        [DataMember]
        public decimal Balance { get; set; }
    }
}