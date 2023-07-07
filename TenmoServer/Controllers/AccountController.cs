using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Exceptions;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountDao AccountDao;
        public AccountController(IAccountDao accountDao)
        {
            this.AccountDao = accountDao;
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountByUserId (int id)
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

        [HttpPut("{accountId}")]
        public ActionResult<Account> UpdateAccount(Account account, int accountId)
        {
            Account updatedAccount = AccountDao.UpdateAccountBalance(account);
            

            if(updatedAccount != null)
            {
                return Ok(updatedAccount);
            }
            else
            {
                return NotFound();
            }
        }
        
    }


}
