using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AccomodationManagementSystem
{

    public class loginInfo
    {
        [Key]
        public int id { get; set; }

        public string user { get; set; } = string.Empty;    
        public string Password { get; set; } = string.Empty;



    }
}