# Development Time Log – O.S. Auto Repair

**Developer:** Cody Pekarek  
**Repository:** OSAutoRepair (ASP.NET Core MVC + Entity Framework Core)  
**Target Launch:** Monday, November 23, 2026  

---

## Daily & Milestone Time Tracking

| Date | Milestone / Issue | Branch | Tasks Completed | Hours |
| :--- | :--- | :--- | :--- | :--- |
| Nov 17, 2026 | Project Setup & Planning | `master` / `develop` | Client requirements discovery (2 hours), ASP.NET Core MVC scaffolding, NuGet/LocalDB setup, GitHub milestones & issues | 4.5 |
| Nov 19, 2026 | M1: Database Schema & Technician Auth (`#1`, `#2`) | `develop` / `feat/issue-1-ef-core-models` / `feat/issue-2-lock-tech-registration` | Create core models (2.5 hrs), Study & Comprehension (1.75 hrs), client communication (0.25 hr), DbContext & EF Core migration (1 hr), scaffold Identity & lock registration (1 hr) | 6.5 |
| Nov 23, 2026 | M2: Public Storefront & 48-Hour Booking (`#3`, `#4`) | `develop` / `feat/M2-issue-3-public-booking` | Create `PublicBookingViewModel.cs` (1 hr), `SchedulingService.cs` & DI registration (1 hr), `HomeController.cs` & storefront `Views/Home/Index.cshtml` (1 hr), Study & Comprehension (1 hr) | 4 |
| Nov 24, 2026 | M2: Public Storefront & 48-Hour Booking (`#3`, `#4`) | `develop` / `feat/M2-issue-3-public-booking` | Client Communication (0.5 hours), Adding Weekends Off (0.5 hours), Study & Comprehension (1.5 hrs), implemented `Views/Home/Book.cshtml` and `Views/Home/BookingConfirmation.cshtml` (0.5 hr) | 3 |
| Nov 25, 2026 | M3: 5-Bay Spreadsheet Dispatch Grid (`#5`, `#6`) | `develop` / `feat/M3-issue-5-dispatch-grid` | Study & Comprehension on Domain Models, ViewModels, and DTOs (2 hrs); built `DispatchViewModel`, `[Authorize]` `DispatchController` (`Index`, `DetailsJson`, `UpdateDetails`, `CreateWalkIn`, `ClosedDates`), `Views/Dispatch/Index.cshtml`, `Views/Dispatch/ClosedDates.cshtml`, and `_LoginPartial.cshtml` technician nav/routing (2 hrs) | 4 |


**Total Logged Hours:** `22 hrs` *(Update at the end of each work session)*

---

## Session Notes & Blockers Resolved
* **Nov 17, 2026:** Pivoted stack from Next.js/Supabase to ASP.NET Core 10.0 MVC with SQL Server LocalDB to align with course rubric requirements. Resolved NuGet transitive security audit warning (`NU1902`) on `System.Security.Cryptography.Pkcs` before applying initial Identity database migration.
* **Nov 19, 2026:** Built `ServiceCategory`, `Appointment`, `ClosedDate`, and `TireStorageItem` models tailored to client requirements (`Oil` and `Repair` categories, friction-free booking without `VehicleInfo`), configured `ApplicationDbContext` Fluent API + seed data, and applied `AddWorkshopdomainModels` migration. Scaffolded `Account\Register` with `[Authorize]`, hid public register link in `_LoginPartial.cshtml`, and resolved `Microsoft.EntityFrameworkCore.Design` (`10.0.15`) downgrade warning and `ApplicationUser` vs. `IdentityUser` scaffolding mismatch.
* **Nov 23, 2026:** Synced `master` and `develop` branches after completing Milestone 1. Created `PublicBookingViewModel` with XML documentation to protect internal bay/technician fields from over-posting, implemented `SchedulingService` (enforcing the 2-day advance booking rule, `ClosedDates` checks, 9:00 AM–5:00 PM shop hours, and lowest-available-bay assignment across Bays 1–5), updated `HomeController` (`[AllowAnonymous]` storefront, JSON `AvailableSlots` endpoint, and `Book` actions), and built the public storefront landing page (`Views/Home/Index.cshtml`).
* **Nov 24, 2026:** Completed `Views/Home/Book.cshtml` (with `min` date lead-time enforcement, `@Html.AntiForgeryToken()`, and live `/Home/AvailableSlots` `fetch` slot filtering) and `Views/Home/BookingConfirmation.cshtml` (displaying confirmed appointment details while keeping internal bay assignments hidden from public view). Verified 5-bay auto-assignment (`Bay 1`–`Bay 5`) and 6th-booking rejection on `12/1/2026` at `9:00 AM`, plus Sunday/Monday closure blocking.
* **Nov 25, 2026:** Built the technician-only 5-bay × 9-hour (`9:00 AM–5:00 PM`) spreadsheet dispatch grid (`Controllers/DispatchController.cs`, `ViewModels/DispatchViewModel.cs`, `Views/Dispatch/Index.cshtml`) locked behind `[Authorize]`. Added the click-to-inspect slide-over offcanvas drawer (`DetailsJson` & `UpdateDetails` JSON endpoints), same-day walk-in override workflow (`CreateWalkIn`), `/Dispatch/ClosedDates` holiday management, and authenticated technician routing/navbar link in `_LoginPartial.cshtml`.
