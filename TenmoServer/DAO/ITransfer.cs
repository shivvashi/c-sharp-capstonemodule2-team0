using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransfer
    {
        public Transfer UpdateTransfer(Transfer transfer);
        public Transfer GetTransferByTransferId(int transferId);
        public List<Transfer> GetTransfersForUser(int userId);
        public Transfer CreateTransfer(Transfer transfer);


    }
}
