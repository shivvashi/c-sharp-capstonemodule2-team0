using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        public Account GetAccountBalance(int id);

    }
}
