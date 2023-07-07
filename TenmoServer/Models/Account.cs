using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class Account
    {
        [Required(ErrorMessage = "The field  'AccountId' is required.")]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "The field 'UserId' is required")]
        public int UserId { get; set; }
        [Range(0.00, double.PositiveInfinity,ErrorMessage = "The field 'Balance' must be greater than 0")]
        public decimal Balance { get; set; } = 1000.00M;



    }
}
