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
        public int columnGenerated_S { get; set; } = 30;
        public bool randomCellColour_S { get; set; } = true;



    }
}