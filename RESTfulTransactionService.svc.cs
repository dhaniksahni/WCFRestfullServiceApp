using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Activation;
namespace TestApp
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RESTfulTransactionService : IRESTfulTransactionService
    {
        public RESTfulTransactionService()
        {

        }

        string ConnString = "Server=localhost;Initial Catalog=TestDatabase;User=sa;Password=pass";

        /// <summary>
        /// Method to Get the Balance from the  given Account Number
        /// </summary>
        /// <param name="AccountNumber">Account Number</param>
        /// <returns>Balance</returns>
        public ResponseBalance GetBalance(string AccountNumber)
        {
            decimal balance = 0;
            using (SqlConnection SqlConn = new SqlConnection(ConnString))
            {
                SqlConn.Open();
                string CheckBalanceForWithdrawQuery = "Select Sum(Amount) from BankAccountDetail Where AccountNumber = @AccountNumber";
                using (SqlCommand cmd = new SqlCommand(CheckBalanceForWithdrawQuery, SqlConn))
                {
                    cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                    balance = Convert.ToInt32(cmd.ExecuteScalar());
                }
                SqlConn.Close();
            }
            return new ResponseBalance { Balance = balance };
        }

        //To Deposit Amount in the Account
        public ResponseData Deposit(int AccountNumber, decimal Amount, string Currency)
        {
            try
            {
                ResponseData SuccessResponse = null;
                int InsertResult = 0;
                using (SqlConnection SqlConn = new SqlConnection(ConnString))
                {
                    SqlConn.Open();
                    string DepositAmountQuery = "insert into BankAccountDetail(AccountNumber,Amount,Currency,Deposit,Withdraw) values(@AccountNumber,@Amount,@Currency,@Deposit,@Withdraw)";

                    using (SqlCommand cmd = new SqlCommand(DepositAmountQuery, SqlConn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Deposit", true);
                        cmd.Parameters.AddWithValue("@Withdraw", false);
                        InsertResult = cmd.ExecuteNonQuery();
                    }
                    if (InsertResult == 1)
                    {
                        string GetBalanceAfterInsertQuery = "SELECT Sum(Amount) FROM BankAccountDetail where AccountNumber = @AccountNumber";
                        decimal Balance = 0;
                        DataTable dt = null;
                        using (SqlCommand cmd2 = new SqlCommand(GetBalanceAfterInsertQuery, SqlConn))
                        {
                            cmd2.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                            Balance = Convert.ToInt32(cmd2.ExecuteScalar());
                        }

                        SuccessResponse = new ResponseData
                        {
                            AccountNumber = AccountNumber,
                            Success = true,
                            Balance = Balance,
                            Currency = Currency,
                            Message = "Amount Deposited Successfully"
                        };
                    }
                    else
                    {
                        SuccessResponse = new ResponseData
                        {
                            AccountNumber = AccountNumber,
                            Success = true,
                            Balance = 0,
                            Currency = Currency,
                            Message = "Amount Deposit Failed Please try Again"
                        };
                    }
                    SqlConn.Close();
                }
                return SuccessResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //To Deposit Withdraw in the Account
        public ResponseData Withdraw(int AccountNumber, decimal Amount, string Currency)
        {
            try
            {
                ResponseData SuccessResponse = null;
                decimal InitialBalance = 0;
                int InsertResult = 0;
                using (SqlConnection SqlConn = new SqlConnection(ConnString))
                {
                    SqlConn.Open();
                    string CheckBalanceForWithdrawQuery = "Select Sum(Amount) from BankAccountDetail Where AccountNumber = @AccountNumber";
                    using (SqlCommand cmd = new SqlCommand(CheckBalanceForWithdrawQuery, SqlConn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                        InitialBalance = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (InitialBalance > Amount)
                    {
                        string WithdrawAmountQuery = "insert into BankAccountDetail(AccountNumber,Amount,Currency,Deposit,Withdraw) values(@AccountNumber,@Amount,@Currency,@Deposit,@Withdraw)";
                        using (SqlCommand cmd2 = new SqlCommand(WithdrawAmountQuery, SqlConn))
                        {
                            cmd2.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                            cmd2.Parameters.AddWithValue("@Amount", (-1 * Amount));
                            cmd2.Parameters.AddWithValue("@Currency", Currency);
                            cmd2.Parameters.AddWithValue("@Deposit", true);
                            cmd2.Parameters.AddWithValue("@Withdraw", false);
                            InsertResult = cmd2.ExecuteNonQuery();
                        }
                        if (InsertResult == 1)
                        {
                            SuccessResponse = new ResponseData
                            {
                                AccountNumber = AccountNumber,
                                Success = true,
                                Balance = InitialBalance - Amount,
                                Currency = Currency,
                                Message = "Amount Withdrawl Successfully"
                            };
                        }
                        else
                        {
                            SuccessResponse = new ResponseData
                            {
                                AccountNumber = AccountNumber,
                                Success = false,
                                Balance = 0,
                                Currency = Currency,
                                Message = "Amount Withdrawl Failed Please try Again!!"
                            };
                        }
                    }
                    else
                    {
                        SuccessResponse = new ResponseData
                        {
                            AccountNumber = AccountNumber,
                            Success = false,
                            Balance = 0,
                            Currency = Currency,
                            Message = "Insufficient Balance to Withdraw."
                        };
                    }
                    SqlConn.Close();
                }
                return SuccessResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
