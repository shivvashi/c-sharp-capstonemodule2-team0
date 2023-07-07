using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class Transfer
    {
        [Required(ErrorMessage ="The field 'TransferId' must not be blank")]
        public int TransferId { get; set; }


        [Required(ErrorMessage = "The field 'TransferTypeId' must not be blank")]
        public int TransferTypeId { get; set; }


        [Required(ErrorMessage = "The field 'TransferStatusId' must not be blank")]
        public int TransferStatusId { get; set; }


        [Required(ErrorMessage = "The field 'AccountFrom' must not be blank")]
        public int AccountFrom { get; set; }


        [Required(ErrorMessage = "The field 'AccountTo' must not be blank")]
        public int AccountTo { get; set; }


        [Range(0.01,double.PositiveInfinity,ErrorMessage = "The field 'Amount' must be greater than 0")]
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
                if(TransferStatusId == 1)
                {
                    return "Pending";
                }
                else if(TransferStatusId == 2)
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
