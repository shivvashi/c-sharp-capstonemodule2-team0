using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Exceptions;
using TenmoServer.Models;
using TenmoServer.Security;
namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountDao AccountDao;
        public AccountController(IAccountDao accountDao)
        {
            this.AccountDao = accountDao;
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount (int id)
        {
            Account account = AccountDao.GetAccountBalance(id);
            if (account != null)
            {
                return account;
            }
            else
            {
                return NotFound();
            }
        }
        
    }


}
