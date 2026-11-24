using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OSAutoRepair.Data;
using OSAutoRepair.Models;
using OSAutoRepair.Services;
using OSAutoRepair.ViewModels;

namespace OSAutoRepair.Controllers
{
    /// <summary>
    /// Handles public storefront pages and anonymous customer appointment booking requests.
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SchedulingService _schedulingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="schedulingService">The scheduling service used to verify availability and assign bays.</param>
        public HomeController(ApplicationDbContext context, SchedulingService schedulingService)
        {
            _context = context;
            _schedulingService = schedulingService;
        }

        /// <summary>
        /// Displays the public O.S. Auto Repair storefront landing page and active services.
        /// </summary>
        /// <returns>The storefront landing view populated with active service categories.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var activeServices = await _context.ServiceCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.ServiceCategoryId)
                .ToListAsync();

            return View(activeServices);
        }

        /// <summary>
        /// Displays the public appointment booking form with active services and a 2-day minimum lead date.
        /// </summary>
        /// <param name="categoryId">Optional pre-selected service category identifier from the storefront.</param>
        /// <returns>The public booking form view.</returns>
        [HttpGet]
        public async Task<IActionResult> Book(int? categoryId = null)
        {
            var viewModel = new PublicBookingViewModel
            {
                AppointmentDate = GetDefaultBookingDate(),
                ServiceCategoryId = categoryId ?? 0
            };

            await PopulateServiceCategoriesAsync(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Returns JSON availability details (open hours 9–17 and closed-date status) for a selected date and service category.
        /// </summary>
        /// <param name="date">The requested appointment date.</param>
        /// <param name="categoryId">The selected service category identifier.</param>
        /// <returns>A JSON object indicating whether the date is closed and listing open starting hours.</returns>
        [HttpGet]
        public async Task<IActionResult> AvailableSlots(DateTime date, int categoryId)
        {
            var category = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.ServiceCategoryId == categoryId && c.IsActive);

            int durationHours = category?.DefaultDurationHours ?? 1;
            bool isClosed = await _schedulingService.IsDateClosedAsync(date);
            var availableHours = await _schedulingService.GetAvailableHoursAsync(date, durationHours);

            return Json(new
            {
                isClosed,
                availableHours
            });
        }

        /// <summary>
        /// Validates and processes a public customer appointment booking, automatically assigning the lowest available bay.
        /// </summary>
        /// <param name="model">The submitted booking form data.</param>
        /// <returns>The booking confirmation view on success, or the booking form with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(PublicBookingViewModel model)
        {
            var minAllowedDate = DateTime.Today.AddDays(2);

            if (model.AppointmentDate.Date < minAllowedDate)
            {
                ModelState.AddModelError(nameof(model.AppointmentDate),
                    $"Appointments must be booked at least 2 days in advance ({minAllowedDate:MMMM d, yyyy} or later).");
            }
            else if (await _schedulingService.IsDateClosedAsync(model.AppointmentDate))
            {
                ModelState.AddModelError(nameof(model.AppointmentDate),
                    "The shop is closed on the selected date. Please choose another day.");
            }

            var selectedCategory = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.ServiceCategoryId == model.ServiceCategoryId && c.IsActive);

            if (selectedCategory == null)
            {
                ModelState.AddModelError(nameof(model.ServiceCategoryId),
                    "Please select a valid active service type.");
            }

            int? assignedBay = null;

            if (ModelState.IsValid && selectedCategory != null)
            {
                assignedBay = await _schedulingService.AssignLowestAvailableBayAsync(
                    model.AppointmentDate,
                    model.StartHour,
                    selectedCategory.DefaultDurationHours);

                if (assignedBay == null)
                {
                    ModelState.AddModelError(nameof(model.StartHour),
                        "All 5 service bays are booked for that time window. Please select a different hour or date.");
                }
            }

            if (!ModelState.IsValid || selectedCategory == null || assignedBay == null)
            {
                await PopulateServiceCategoriesAsync(model);
                return View(model);
            }

            var appointment = new Appointment
            {
                BayNumber = assignedBay.Value,
                AppointmentDate = model.AppointmentDate.Date,
                StartHour = model.StartHour,
                DurationHours = selectedCategory.DefaultDurationHours,
                ServiceCategoryId = selectedCategory.ServiceCategoryId,
                CustomerFirstName = model.CustomerFirstName.Trim(),
                CustomerLastName = model.CustomerLastName.Trim(),
                CustomerPhone = model.CustomerPhone.Trim(),
                CustomerEmail = string.IsNullOrWhiteSpace(model.CustomerEmail) ? null : model.CustomerEmail.Trim(),
                CustomerServiceNotes = string.IsNullOrWhiteSpace(model.CustomerServiceNotes) ? null : model.CustomerServiceNotes.Trim(),
                EstimatedCost = selectedCategory.BaseEstimate,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            appointment.ServiceCategory = selectedCategory;
            return View("BookingConfirmation", appointment);
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>The privacy view.</returns>
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the standard application error page.
        /// </summary>
        /// <returns>The error view with request diagnostic information.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Populates the active service categories dropdown list on the booking view model.
        /// </summary>
        /// <param name="model">The booking view model to populate.</param>
        private async Task PopulateServiceCategoriesAsync(PublicBookingViewModel model)
        {
            var categories = await _context.ServiceCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.ServiceCategoryId)
                .ToListAsync();

            model.ServiceCategories = new SelectList(categories, nameof(ServiceCategory.ServiceCategoryId), nameof(ServiceCategory.Name), model.ServiceCategoryId);
        }

        /// <summary>
        /// Calculates the earliest default booking date at least 2 days ahead, skipping Sundays and Mondays.
        /// </summary>
        /// <returns>The earliest open date at least 2 days from today.</returns>
        private static DateTime GetDefaultBookingDate()
        {
            var candidate = DateTime.Today.AddDays(2);
            while (candidate.DayOfWeek == DayOfWeek.Sunday || candidate.DayOfWeek == DayOfWeek.Monday)
            {
                candidate = candidate.AddDays(1);
            }
            return candidate;
        }

    }
}
