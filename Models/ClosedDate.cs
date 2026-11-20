using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

namespace OSAutoRepair.Models
{
    /// <summary>
    /// creates the dates that the shop is closed and blocked from customer booking. (the technician should have the authority to overide
    /// </summary>
    public class ClosedDate
    {

        /// <summary>
        /// defines the Id for the closed date
        /// </summary>
        [Key]
        public int ClosedDateId { get; set; }

        /// <summary>
        /// defines the date that is closed 
        /// </summary>
        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// the optional reasoning the date is closed for booking
        /// </summary>
        [StringLength(150)]
        public string? Reason { get; set; }
    }
}
