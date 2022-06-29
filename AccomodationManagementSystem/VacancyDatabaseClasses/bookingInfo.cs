using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AccomodationManagementSystem.VacancyDatabaseClasses
{
    public class bookingInfo
    {

        [Key]
        public int id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set; } = string.Empty;
        public string ExtraDetails { get; set; } = string.Empty;
        public string BookTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public float DailyRate { get; set; }
        public int RoomId { get; set; }




    }
}
