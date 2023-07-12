using TenmoServer.DAO;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.Exceptions;
using TenmoServer.Models;
using TenmoServer.Security;
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Authorize]
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
        [HttpGet("/{userId}/transfers")]
        public List<Transfer> ListTransfers(int userId)
        {
            return TransferDao.GetTransfersForUser(userId);
        }

        [HttpGet("/{userId}/pending/transfers")]
        public IList<Transfer> ListPendingRequests(int userId)
        {
            return TransferDao.GetPendingRequestsForUser(userId);
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
            Transfer updatedTransfer = TransferDao.UpdateTransfer(transfer);

           if(updatedTransfer != null)
            {
                return Ok(updatedTransfer);
            }
            else
            {
                return NotFound();
            }
            
        }

    }
}
