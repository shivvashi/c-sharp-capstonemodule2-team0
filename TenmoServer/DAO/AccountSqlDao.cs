using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Data.SqlClient;
using TenmoServer.Exceptions;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {

        private readonly string ConnectionString;

        public AccountSqlDao (string dbconnectionString)
        {
            ConnectionString = dbconnectionString;
        }



        public Account GetAccountBalance(int id)
        {
            Account account = null;
            string sql = "SELECT * FROM account " +
                "WHERE user_id = @user_id";
             
            try
            {
                using(SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        account = MapRowToAccount(reader);
                    }
                }                
            }
            catch(SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return account;           
        }

        public Account MapRowToAccount(SqlDataReader reader)
        {
            Account account = new Account();
            account.AccountId = Convert.ToInt32(reader["account_id"]);
            account.UserId = Convert.ToInt32(reader["user_id"]);
            account.Balance = Convert.ToDecimal(reader["balance"]);

            return account;
        }
        

    }
}
