using Microsoft.EntityFrameworkCore;
using OSAutoRepair.Data;

namespace OSAutoRepair.Services
{
    /// <summary>
    /// Encapsulates shop scheduling rules, 5-bay capacity checks, and automatic bay assignment.
    /// </summary>
    public class SchedulingService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingService"/> class.
        /// </summary>
        /// <param name="context">The database context used to query appointments and closed dates.</param>
        public SchedulingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks whether the shop is closed on the specified date.
        /// </summary>
        /// <param name="date">The calendar date to check.</param>
        /// <returns><c>true</c> if the date is listed in ClosedDates; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsDateClosedAsync(DateTime date)
        {
            var targetDate = date.Date;
            return await _context.ClosedDates
                .AnyAsync(c => c.Date.Date == targetDate);
        }

        /// <summary>
        /// Retrieves available starting hours (9:00 AM to 5:00 PM) for a public booking on the specified date.
        /// </summary>
        /// <param name="date">The requested appointment date.</param>
        /// <param name="durationHours">The expected duration of the service in hours (defaults to 1).</param>
        /// <returns>A list of open starting hours (9 through 17) that can accommodate the requested duration.</returns>
        public async Task<List<int>> GetAvailableHoursAsync(DateTime date, int durationHours = 1)
        {
            var targetDate = date.Date;

            // Enforce the 2-day public booking lead time and reject closed shop dates.
            if (targetDate < DateTime.Today.AddDays(2) || await IsDateClosedAsync(targetDate))
            {
                return new List<int>();
            }

            var existingAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == targetDate)
                .Select(a => new { a.BayNumber, a.StartHour, a.DurationHours })
                .ToListAsync();

            var availableHours = new List<int>();

            // Shop hours run from 9 (9:00 AM) through 17 (5:00 PM), closing at 18 (6:00 PM).
            for (int startHour = 9; startHour <= 17; startHour++)
            {
                if (startHour + durationHours > 18)
                {
                    continue;
                }

                int endHour = startHour + durationHours;

                // Count how many distinct bays have an overlapping appointment in [startHour, endHour).
                int occupiedBays = existingAppointments
                    .Where(a => a.StartHour < endHour && (a.StartHour + a.DurationHours) > startHour)
                    .Select(a => a.BayNumber)
                    .Distinct()
                    .Count();

                if (occupiedBays < 5)
                {
                    availableHours.Add(startHour);
                }
            }

            return availableHours;
        }

        /// <summary>
        /// Finds the lowest-numbered available service bay (1 through 5) for a given date, start hour, and duration.
        /// </summary>
        /// <param name="date">The requested appointment date.</param>
        /// <param name="startHour">The starting hour in 24-hour shop time (9 through 17).</param>
        /// <param name="durationHours">The duration of the appointment in hours.</param>
        /// <returns>The lowest open bay number (1–5), or <c>null</c> if all 5 bays are booked during that window.</returns>
        public async Task<int?> AssignLowestAvailableBayAsync(DateTime date, int startHour, int durationHours)
        {
            int endHour = startHour + durationHours;

            if (startHour < 9 || endHour > 18)
            {
                return null;
            }

            var targetDate = date.Date;

            var existingAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == targetDate)
                .Select(a => new { a.BayNumber, a.StartHour, a.DurationHours })
                .ToListAsync();

            for (int bay = 1; bay <= 5; bay++)
            {
                bool hasConflict = existingAppointments.Any(a =>
                    a.BayNumber == bay &&
                    a.StartHour < endHour &&
                    (a.StartHour + a.DurationHours) > startHour);

                if (!hasConflict)
                {
                    return bay;
                }
            }

            return null;
        }
    }
}
