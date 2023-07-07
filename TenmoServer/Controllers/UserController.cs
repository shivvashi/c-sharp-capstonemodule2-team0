using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Policy;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private  IUserDao UserDao;
        public UserController(IUserDao userDao)
        {
            this.UserDao = userDao;
        }


        [HttpGet("{userId}")]
        public ActionResult<User> GetUserByUserId(int userId)
        {
            User user = UserDao.GetUserById(userId);
            if(user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet()]
        public ActionResult<IList<User>> List(string username = "", int accountId = 0)
        {
            IList<User> users = new List<User>();
            if(username == "" && accountId == 0)
            {
                users = UserDao.GetUsers();
                return Ok(users);
            }
            else if(username != "" && accountId == 0)
            {
                User user = UserDao.GetUserByUsername(username);
                users.Add(user);
                return Ok(users);
            }
            else if(username == "" && accountId != 0)
            {
                User user = UserDao.GetUserByAccountId(accountId);
                users.Add(user);
                return Ok(users);
            }
            else
            {
                return NotFound();
            }
        }

        

        

    }
}
