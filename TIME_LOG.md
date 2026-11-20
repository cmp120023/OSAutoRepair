# Development Time Log – O.S. Auto Repair

**Developer:** Cody Pekarek  
**Repository:** OSAutoRepair (ASP.NET Core MVC + Entity Framework Core)  
**Target Launch:** Monday, November 23, 2026  

---

## Daily & Milestone Time Tracking

| Date | Milestone / Issue | Branch | Tasks Completed | Hours |
| :--- | :--- | :--- | :--- | :--- |
| Nov 17, 2026 | Project Setup & Planning | `master` / `develop` | Client requirements discovery (2 hours), ASP.NET Core MVC scaffolding, NuGet/LocalDB setup, GitHub milestones & issues | 4.5 |
| Nov 19, 2026 | M1: Database Schema & Technician Auth (`#1`, `#2`) | `develop` / `feat/issue-1-ef-core-models` / `feat/issue-2-lock-tech-registration` | Create core models (2.5 hrs), notes/comprehension (1.75 hrs), client communication (15 min), DbContext & EF Core migration (1 hr), scaffold Identity & lock registration (1 hr) | 6.5 |

**Total Logged Hours:** `11 hrs` *(Update at the end of each work session)*

---

## Session Notes & Blockers Resolved
* **Nov 17, 2026:** Pivoted stack from Next.js/Supabase to ASP.NET Core 10.0 MVC with SQL Server LocalDB to align with course rubric requirements. Resolved NuGet transitive security audit warning (`NU1902`) on `System.Security.Cryptography.Pkcs` before applying initial Identity database migration.
* **Nov 19, 2026:** Built `ServiceCategory`, `Appointment`, `ClosedDate`, and `TireStorageItem` models tailored to client requirements (`Oil` and `Repair` categories, friction-free booking without `VehicleInfo`), configured `ApplicationDbContext` Fluent API + seed data, and applied `AddWorkshopdomainModels` migration. Scaffolded `Account\Register` with `[Authorize]`, hid public register link in `_LoginPartial.cshtml`, and resolved `Microsoft.EntityFrameworkCore.Design` (`10.0.15`) downgrade warning and `ApplicationUser` vs. `IdentityUser` scaffolding mismatch.
