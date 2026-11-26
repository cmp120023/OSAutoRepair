using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OSAutoRepair.Data;
using OSAutoRepair.Models;
using OSAutoRepair.ViewModels;

namespace OSAutoRepair.Controllers
{
    /// <summary>
    /// Authenticated technician controller managing the 5-bay spreadsheet dispatch grid,
    /// appointment inspection drawer, same-day walk-in overrides, and shop closed dates.
    /// </summary>
    [Authorize]
    public class DispatchController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchController"/> class.
        /// </summary>
        /// <param name="context">The Entity Framework database context.</param>
        public DispatchController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the 5-bay by 9-hour technician dispatch grid for the requested date.
        /// </summary>
        /// <param name="date">Optional target date; defaults to today.</param>
        /// <returns>The dispatch grid view populated with appointments and shop status.</returns>
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            var selectedDate = (date ?? DateTime.Today).Date;
            var model = await BuildDispatchGridViewModelAsync(selectedDate);
            return View(model);
        }

        /// <summary>
        /// Returns full customer and technician details for a single appointment as JSON for the slide-over inspection drawer.
        /// </summary>
        /// <param name="id">The appointment identifier.</param>
        /// <returns>A JSON object containing appointment details, or 404 if not found.</returns>
        [HttpGet]
        public async Task<IActionResult> DetailsJson(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.ServiceCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            return Json(new
            {
                appointmentId = appointment.AppointmentId,
                parentAppointmentId = appointment.ParentAppointmentId,
                bayNumber = appointment.BayNumber,
                appointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                startHour = appointment.StartHour,
                durationHours = appointment.DurationHours,
                serviceCategoryName = appointment.ServiceCategory?.Name ?? "Service",
                customerFirstName = appointment.CustomerFirstName,
                customerLastName = appointment.CustomerLastName,
                customerPhone = appointment.CustomerPhone,
                customerEmail = appointment.CustomerEmail ?? string.Empty,
                customerServiceNotes = appointment.CustomerServiceNotes ?? string.Empty,
                technicianServiceNotes = appointment.TechnicianServiceNotes ?? string.Empty,
                estimatedCost = appointment.EstimatedCost
            });
        }

        /// <summary>
        /// Saves technician notes and estimated cost updates submitted from the inspection side drawer.
        /// </summary>
        /// <param name="input">The updated technician notes and estimated cost payload.</param>
        /// <returns>A JSON status response indicating whether the update succeeded.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDetails([FromBody] UpdateAppointmentDetailsModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid appointment update values." });
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == input.AppointmentId);

            if (appointment == null)
            {
                return NotFound(new { success = false, message = "Appointment not found." });
            }

            appointment.TechnicianServiceNotes = string.IsNullOrWhiteSpace(input.TechnicianServiceNotes)
                ? null
                : input.TechnicianServiceNotes.Trim();
            appointment.EstimatedCost = input.EstimatedCost;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Appointment details saved."
            });
        }

        /// <summary>
        /// Creates a same-day or future walk-in appointment in a technician-selected bay and hour,
        /// bypassing the public 2-day advance booking restriction.
        /// </summary>
        /// <param name="walkInForm">The walk-in appointment details submitted from the dispatch modal.</param>
        /// <returns>Redirects back to the dispatch grid for the scheduled date.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWalkIn([Bind(Prefix = "WalkInForm")] WalkInAppointmentInputModel walkInForm)
        {
            var targetDate = walkInForm.AppointmentDate.Date;

            if (walkInForm.StartHour + walkInForm.DurationHours > 18)
            {
                ModelState.AddModelError("WalkInForm.DurationHours", "Appointment duration cannot extend past the 6:00 PM closing boundary (5:00 PM last start hour).");
            }

            var category = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.ServiceCategoryId == walkInForm.ServiceCategoryId && c.IsActive);

            if (category == null)
            {
                ModelState.AddModelError("WalkInForm.ServiceCategoryId", "Please select a valid active service category.");
            }

            var proposedEnd = walkInForm.StartHour + walkInForm.DurationHours;
            bool hasBayConflict = await _context.Appointments.AnyAsync(a =>
                a.AppointmentDate == targetDate &&
                a.BayNumber == walkInForm.BayNumber &&
                a.StartHour < proposedEnd &&
                (a.StartHour + a.DurationHours) > walkInForm.StartHour);

            if (hasBayConflict)
            {
                ModelState.AddModelError("WalkInForm.BayNumber", $"Bay {walkInForm.BayNumber} already has an overlapping job during that time window.");
            }

            if (!ModelState.IsValid)
            {
                TempData["WalkInError"] = string.Join(" ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Index), new { date = targetDate.ToString("yyyy-MM-dd") });
            }

            var appointment = new Appointment
            {
                AppointmentDate = targetDate,
                BayNumber = walkInForm.BayNumber,
                StartHour = walkInForm.StartHour,
                DurationHours = walkInForm.DurationHours,
                ServiceCategoryId = category!.ServiceCategoryId,
                CustomerFirstName = walkInForm.CustomerFirstName.Trim(),
                CustomerLastName = walkInForm.CustomerLastName.Trim(),
                CustomerPhone = walkInForm.CustomerPhone.Trim(),
                CustomerEmail = string.IsNullOrWhiteSpace(walkInForm.CustomerEmail) ? null : walkInForm.CustomerEmail.Trim(),
                CustomerServiceNotes = string.IsNullOrWhiteSpace(walkInForm.CustomerServiceNotes) ? null : walkInForm.CustomerServiceNotes.Trim(),
                TechnicianServiceNotes = string.IsNullOrWhiteSpace(walkInForm.TechnicianServiceNotes) ? null : walkInForm.TechnicianServiceNotes.Trim(),
                EstimatedCost = category.BaseEstimate,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Walk-in appointment added to Bay {appointment.BayNumber} at {DateTime.Today.AddHours(appointment.StartHour):h:00 tt}.";
            return RedirectToAction(nameof(Index), new { date = targetDate.ToString("yyyy-MM-dd") });
        }

        /// <summary>
        /// Displays the shop closed dates and holidays management view.
        /// </summary>
        /// <returns>The closed dates management view.</returns>
        [HttpGet]
        public async Task<IActionResult> ClosedDates()
        {
            var viewModel = new ManageClosedDatesViewModel
            {
                NewClosedDate = DateTime.Today.AddDays(1),
                ExistingClosedDates = await _context.ClosedDates
                    .OrderBy(c => c.Date)
                    .AsNoTracking()
                    .ToListAsync()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Adds a new closed date or shop holiday to block public online bookings.
        /// </summary>
        /// <param name="model">The closed date form input.</param>
        /// <returns>Redirects to the closed dates management view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClosedDate(ManageClosedDatesViewModel model)
        {
            var targetDate = model.NewClosedDate.Date;

            bool exists = await _context.ClosedDates.AnyAsync(c => c.Date == targetDate);
            if (exists)
            {
                TempData["ErrorMessage"] = $"{targetDate:MMMM d, yyyy} is already marked as closed.";
                return RedirectToAction(nameof(ClosedDates));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide a valid date.";
                return RedirectToAction(nameof(ClosedDates));
            }

            var closedDate = new ClosedDate
            {
                Date = targetDate,
                Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim()
            };

            _context.ClosedDates.Add(closedDate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Added {targetDate:MMMM d, yyyy} to shop closed dates.";
            return RedirectToAction(nameof(ClosedDates));
        }

        /// <summary>
        /// Removes a scheduled closed date or shop holiday.
        /// </summary>
        /// <param name="id">The identifier of the closed date record to delete.</param>
        /// <returns>Redirects back to the closed dates management view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClosedDate(int id)
        {
            var closedDate = await _context.ClosedDates.FindAsync(id);
            if (closedDate != null)
            {
                _context.ClosedDates.Remove(closedDate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Removed {closedDate.Date:MMMM d, yyyy} from closed dates.";
            }

            return RedirectToAction(nameof(ClosedDates));
        }

        /// <summary>
        /// Builds the populated dispatch grid view model for a given date.
        /// </summary>
        /// <param name="selectedDate">The normalized date to load.</param>
        /// <returns>A populated <see cref="DispatchGridViewModel"/> instance.</returns>
        private async Task<DispatchGridViewModel> BuildDispatchGridViewModelAsync(DateTime selectedDate)
        {
            var appointments = await _context.Appointments
                .Include(a => a.ServiceCategory)
                .Where(a => a.AppointmentDate == selectedDate)
                .OrderBy(a => a.BayNumber)
                .ThenBy(a => a.StartHour)
                .AsNoTracking()
                .ToListAsync();

            var closedDateInfo = await _context.ClosedDates
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Date == selectedDate);

            bool isWeeklyClosed = selectedDate.DayOfWeek == DayOfWeek.Sunday || selectedDate.DayOfWeek == DayOfWeek.Monday;

            var categories = await _context.ServiceCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.ServiceCategoryId)
                .Select(c => new SelectListItem
                {
                    Value = c.ServiceCategoryId.ToString(),
                    Text = $"{c.Name} (Est. {c.DefaultDurationHours} hr{(c.DefaultDurationHours == 1 ? "" : "s")})"
                })
                .ToListAsync();

            return new DispatchGridViewModel
            {
                SelectedDate = selectedDate,
                ClosedDateInfo = closedDateInfo,
                IsShopClosed = isWeeklyClosed || closedDateInfo != null,
                Appointments = appointments,
                ServiceCategories = categories,
                WalkInForm = new WalkInAppointmentInputModel
                {
                    AppointmentDate = selectedDate,
                    BayNumber = 1,
                    StartHour = 9,
                    DurationHours = 1
                }
            };
        }
    }
}
