using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OSAutoRepair.Models;

namespace OSAutoRepair.ViewModels
{
    /// <summary>
    /// View model representing the technician 5-bay by 9-hour spreadsheet dispatch grid for a single date.
    /// </summary>
    public class DispatchGridViewModel
    {
        /// <summary>
        /// Gets or sets the date currently displayed on the dispatch grid.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Gets or sets the closed date record if the selected date is marked closed or a holiday.
        /// </summary>
        public ClosedDate? ClosedDateInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the selected date falls on a weekly closed day (Sunday or Monday) or a scheduled holiday.
        /// </summary>
        public bool IsShopClosed { get; set; }

        /// <summary>
        /// Gets or sets all appointments scheduled on the selected date across Bays 1 through 5.
        /// </summary>
        public List<Appointment> Appointments { get; set; } = new();

        /// <summary>
        /// Gets or sets the available service categories for the walk-in appointment form.
        /// </summary>
        public List<SelectListItem> ServiceCategories { get; set; } = new();

        /// <summary>
        /// Gets or sets the form input model for creating a same-day or future walk-in appointment.
        /// </summary>
        public WalkInAppointmentInputModel WalkInForm { get; set; } = new();
    }

    /// <summary>
    /// Input model used by technicians to update internal notes and estimated cost from the inspection drawer.
    /// </summary>
    public class UpdateAppointmentDetailsModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the appointment being updated.
        /// </summary>
        [Required]
        public int AppointmentId { get; set; }

        /// <summary>
        /// Gets or sets the internal technician service notes, including vehicle and parts details.
        /// </summary>
        [StringLength(2000)]
        public string? TechnicianServiceNotes { get; set; }

        /// <summary>
        /// Gets or sets the technician-adjusted estimated cost for the appointment.
        /// </summary>
        [Range(0, 99999.99)]
        public decimal EstimatedCost { get; set; }
    }

    /// <summary>
    /// Input model used by technicians to schedule a same-day or future walk-in appointment, bypassing the 2-day customer lead time.
    /// </summary>
    public class WalkInAppointmentInputModel
    {
        /// <summary>
        /// Gets or sets the scheduled date for the walk-in appointment.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Gets or sets the assigned service bay number (1 through 5).
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "Bay number must be between 1 and 5.")]
        public int BayNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the starting hour in 24-hour shop time (9 = 9:00 AM through 17 = 5:00 PM).
        /// </summary>
        [Required]
        [Range(9, 17, ErrorMessage = "Start hour must be between 9:00 AM and 5:00 PM.")]
        public int StartHour { get; set; } = 9;

        /// <summary>
        /// Gets or sets the appointment duration in hours (1 through 8).
        /// </summary>
        [Required]
        [Range(1, 8, ErrorMessage = "Duration must be between 1 and 8 hours.")]
        public int DurationHours { get; set; } = 1;

        /// <summary>
        /// Gets or sets the selected service category identifier.
        /// </summary>
        [Required]
        [Display(Name = "Service Category")]
        public int ServiceCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string CustomerFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string CustomerLastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's contact phone number.
        /// </summary>
        [Required]
        [Phone]
        [StringLength(25)]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's optional email address.
        /// </summary>
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email (Optional)")]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Gets or sets the customer's reported issue or vehicle notes.
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Customer / Vehicle Notes")]
        public string? CustomerServiceNotes { get; set; }

        /// <summary>
        /// Gets or sets optional internal technician notes entered at walk-in creation.
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Technician Notes")]
        public string? TechnicianServiceNotes { get; set; }
    }

    /// <summary>
    /// View model for managing scheduled shop holidays and closed dates.
    /// </summary>
    public class ManageClosedDatesViewModel
    {
        /// <summary>
        /// Gets or sets the new date to mark as closed.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Closed Date")]
        public DateTime NewClosedDate { get; set; } = DateTime.Today.AddDays(1);

        /// <summary>
        /// Gets or sets an optional reason or holiday label for the closed date.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Reason / Holiday (Optional)")]
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or sets all existing closed dates ordered chronologically.
        /// </summary>
        public List<ClosedDate> ExistingClosedDates { get; set; } = new();
    }
}
