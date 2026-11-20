using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSAutoRepair.Models
{
    /// <summary>
    /// This creates a scheduled service appointment
    /// that will be assigned to 1 of 5 working bays
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// assigns each appointment an Id.
        /// </summary>
        [Key]
        public int AppointmentId { get; set; }

        /// <summary>
        /// this establishes that an appointment Id can optionally have a parent Id.
        /// the app will have a copy and paste feature for appointments that can extend
        /// beyond the length of a day. the copied appointment will assume the parent Id
        /// and the subsequent child will be assigned it's own Id
        /// </summary>
        public int? ParentAppointmentId { get; set; }

        /// <summary>
        /// this will connect the child to the parent appointment
        /// </summary>
        [ForeignKey(nameof(ParentAppointmentId))]
        public Appointment? ParentAppointment { get; set; }

        /// <summary>
        /// This sets the number of bays available to hold appointments. so that 5 customers, each can have an appt
        /// at the same time. but no more. 
        /// </summary>
        [Range(1,5)]
        public int BayNumber { get; set; }

        /// <summary>
        /// sets the calendar date of the appointment
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }


        /// <summary>
        /// this is the starting hour of the appt. (9am to 5pm are the store hours)
        /// </summary>
        [Range(9, 17)]
        public int StartHour { get; set; }

        /// <summary>
        /// this ensures that the appointment duration stays within range of a single workday. if longer is needed the technician will copy the appt
        /// over to another date. All appts are defaulted to 1 hour.
        /// </summary>
        [Range(1, 8)]
        public int DurationHours { get; set; } = 1;

        /// <summary>
        /// requires that an appointment have a category Id, I.E. Oil, tires, brakes etc.(the appropriate Id, not the defined string name)
        /// </summary>
        [Required]
        public int ServiceCategoryId { get; set; }

        /// <summary>
        /// Defines the service category the appointment falls under. Currenty the client only wants two categories, Oil and Repair
        /// </summary>
        public ServiceCategory? ServiceCategory { get; set; }

        /// <summary>
        /// Defines the customers first name
        /// </summary>
        [Required, StringLength(50)]
        public string CustomerFirstName { get; set; } = string.Empty;//the combination of string.emtpy and required, prevents the customer from leaving the name blank
                                                                     //but allows the C# compiler to build the appt object when first created in memory.
        /// <summary>
        /// defines the customers last name
        /// </summary>
        [Required, StringLength(50)]
        public string CustomerLastName { get; set; } = string.Empty;

        /// <summary>
        /// defines the customers phone number
        /// </summary>
        [Required, Phone, StringLength(25)]
        public string CustomerPhone { get; set; } = String.Empty;

        /// <summary>
        /// this optionally defines a customers email. the client has customers who may not have an email and does not want them to have to create 
        /// an email, just to make an appt and never use the email again. 
        /// </summary>
        [EmailAddress, StringLength(100)]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// defines the notes the customer wants to leave for the technician
        /// </summary>
        [StringLength(1000)]
        public string? CustomerServiceNotes { get; set; }

        /// <summary>
        /// this defines the notes that only technicians will see.
        /// </summary>
        [StringLength (2000)]
        public string? TechnicianServiceNotes { get; set; }

        /// <summary>
        /// defines the technicians estimated cost for the service.
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal EstimatedCost { get; set; } = 0.00m;

        /// <summary>
        /// Time stamp of when record was created
        /// </summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}