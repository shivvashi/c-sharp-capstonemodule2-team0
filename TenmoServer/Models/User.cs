using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class User
    {
        [Required(ErrorMessage = "The field 'UserId' must not be blank.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The field 'Username' must not be blank.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The field 'PasswordHash' must not be blank.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "The field 'Salt' must not be blank.")]
        public string Salt { get; set; }


        public string Email { get; set; }
    }

    /// <summary>
    /// Model to return upon successful login
    /// </summary>
    public class ReturnUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        //public string Role { get; set; }
        public string Token { get; set; }
    }

    /// <summary>
    /// Model to accept login parameters
    /// </summary>
    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
