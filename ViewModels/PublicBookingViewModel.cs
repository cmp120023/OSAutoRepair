using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OSAutoRepair.ViewModels
{
    /// <summary>
    /// Builds the form to hold the fields of data below.
    /// </summary>
    public class PublicBookingViewModel
    {
        /// <summary>
        /// The service type.
        /// </summary>
        [Required(ErrorMessage = "Please select a service type.")]
        [Display(Name = "Service Type")]
        public int ServiceCategoryId { get; set; }

        /// <summary>
        /// The appointment date.
        /// </summary>
        [Required(ErrorMessage = "Please select an appointment date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(2);

        /// <summary>
        ///  The appointment time.
        /// </summary>
        [Required(ErrorMessage = "Please select an arrival time.")]
        [Range(9, 17, ErrorMessage = "Appointments must start between 9:00 AM and 5:00 PM.")]
        [Display(Name = "Preferred Time")]
        public int StartHour { get; set; } = 9;

        /// <summary>
        /// Customer's first name.
        /// </summary>
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string CustomerFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Customer's last name.
        /// </summary>
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string CustomerLastName { get; set; } = string.Empty;

        /// <summary>
        /// Customer's phone number.
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// Customer's email address.
        /// </summary>
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        [Display(Name = "Email Address (Optional)")]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Customer's notes for technician.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        [Display(Name = "Vehicle & Issue Notes (Optional)")]
        public string? CustomerServiceNotes { get; set; }

        /// <summary>
        /// The dropdown list of service categories available.
        /// </summary>
        public SelectList? ServiceCategories { get; set; }
    }
}
