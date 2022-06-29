using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AccomodationManagementSystem.VacancyDatabaseClasses
{
    public class roomInfo
    {
        [Key]
        public int id { get; set; }
        public string RoomType { get; set; } = string.Empty;
    }
}
