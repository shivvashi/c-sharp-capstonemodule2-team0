using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
        public string TransferTypeDesc
        {
            get
            {
                return TransferTypeId == 1 ? "Request" : "Send";
            }
        }
        public string TransferStatusDesc
        {
            get
            {
                if (TransferStatusId == 1)
                {
                    return "Pending";
                }
                else if (TransferStatusId == 2)
                {
                    return "Approved";
                }
                else
                {
                    return "Rejected";
                }
            }
        }
    }
}
