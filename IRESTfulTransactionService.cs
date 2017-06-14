using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TestApp
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IRESTfulTransactionService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "withdraw")]
        ResponseData Withdraw(int AccountNumber, decimal Amount, string Currency);

        [OperationContract]
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
           UriTemplate = "deposit")]
        ResponseData Deposit(int AccountNumber, decimal Amount, string Currency);

        [OperationContract]
        [WebGet(UriTemplate = "balance/{AccountNumber}",
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBalance GetBalance(string AccountNumber);
    }
}
