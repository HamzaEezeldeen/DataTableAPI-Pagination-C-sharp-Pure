using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportProductsWeb.Models
{
    [Table("tblUsers")]
    public class UserApplication : IdentityUser
    {
        [MaxLength(70)]
        public string FullName { get; set; }

        [MaxLength(70)]
        public string Address { get; set; }
    }
}