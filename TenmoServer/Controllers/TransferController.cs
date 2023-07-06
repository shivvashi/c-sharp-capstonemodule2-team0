using TenmoServer.DAO;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.Exceptions;
using TenmoServer.Models;
using TenmoServer.Security;
using System.Collections.Generic;
using System.Security.Policy;

namespace TenmoServer.Controllers
{
    [Route("transfers")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private ITransferDao TransferDao;
        public TransferController(ITransferDao transferDao)
        {
            this.TransferDao = transferDao;
        }

        //working
        [HttpGet("/account/{userId}/transfers")]
        public List<Transfer> ListTransfers(int userId)
        {
            return TransferDao.GetTransfersForUser(userId);
        }

        //working
        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransfer(int transferId)
        {
            //Get a specific transfer by Id
            Transfer transfer = TransferDao.GetTransferByTransferId(transferId);
            if(transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
            
        }

        [HttpGet("/from/{id}")]
        public ActionResult<string> GetUsernameByFromId(int id)
        {
            string username = TransferDao.GetFromUserById(id);
            if(username != null)
            {
                return Ok(username);
            }
            else
            {
                return NotFound();
            }

           
        }

        [HttpGet("/to/{id}")]
        public ActionResult<string> GetUsernameByToId(int id)
        {
            string username = TransferDao.GetToUserById(id);
            if (username != null)
            {
                return Ok(username);
            }
            else
            {
                return NotFound();
            }


        }

        [HttpPost()]
        public ActionResult<Transfer> AddTransfer(Transfer transfer)
        {
            //Create a new Transfer
            Transfer added = TransferDao.CreateTransfer(transfer);
            return Created($"/transfer/{added.TransferId}", added);
        }

        [HttpPut("{transferId}")]
        public ActionResult<Transfer> UpdateTransfer(Transfer transfer, int transferId)
        {
            //Update a transfer
            transfer.TransferId = transferId;

            try
            {
                Transfer result = TransferDao.UpdateTransfer(transfer);
                return Ok(result);
            }
            catch (DaoException)
            {
                return NotFound();
            }
            
        }

    }
}
