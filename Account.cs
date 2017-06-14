using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp
{
    [Serializable()]
    public class Account
    {
        public int AccountNumber;
        public decimal Amount;
        public string Currency;
    }
}