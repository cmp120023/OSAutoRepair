using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OSAutoRepair.Models
{

    /// <summary>
    /// this class defines the shape of each field of a service catergory.
    /// </summary>
    public class ServiceCategory
    {
        /// <summary>
        /// gets/sets primary key for service category.
        /// </summary>
        [Key] 
        public int ServiceCategoryId { get; set; }

        /// <summary>
        /// sets the name for the service category
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;//string.Empty sets the default value to "" instead of null.(an empty string)

        /// <summary>
        /// gets/sets default duration in hours required for this service
        /// </summary>
        [Range(1, 8)]
        public int DefaultDurationHours { get; set; } = 1;//default time block is 1 hour. tech can extend or reduce up to 1 business day (8 hours) or minimally 1 hour.

        /// <summary>
        /// gets/sets baseline estimate shown to technicians.
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseEstimate { get; set; } = 0.00m;//we used m to define this property as a decimal to avoid a compiler error, and prevents precision loss like a double would

        /// <summary>
        /// gets/sets a value indicating whether a customer can currently book this service
        /// allows technicians to temporarily suspend certain services.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// gets/sets appointments scheduled under this service category.
        /// this is a one-to-many relationship, allowing for one category to be associated with many appointments
        /// </summary>
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}