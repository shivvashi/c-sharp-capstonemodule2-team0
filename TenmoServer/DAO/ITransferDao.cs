using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public Transfer UpdateTransfer(Transfer transfer);
        public Transfer GetTransferByTransferId(int transferId);
        public List<Transfer> GetTransfersForUser(int accountId);
        public Transfer CreateTransfer(Transfer transfer);
        public IList<Transfer> GetPendingRequestsForUser(int userId);


    }
}
