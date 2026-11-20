using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSAutoRepair.Models;

namespace OSAutoRepair.Data
{
    /// <summary>
    /// This opening signature for Entityframework is mostly boilerplate with the exception of using OSAutoRepair.Models seen above.
    /// It imports the namespaces necessary for VS to build database context and uses this file to communicate with SQL.
    /// </summary>
    /// <param name="options"></param>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // The following DbSets are telling EF that these are the tables (plural) and the model that each row will represent/resemble.

        /// <summary>
        /// builds the table "appointments" with appointment objects' fields as the rows.
        /// </summary>
        public DbSet<Appointment> Appointments { get; set; }

        /// <summary>
        /// builds the table "ClosedDates" with ClosedDate objects' fields as the rows.
        /// </summary>
        public DbSet<ClosedDate> ClosedDates { get; set; }

        /// <summary>
        /// builds the table "ServiceCategories" with ServiceCategory objects' fields as the rows.
        /// </summary>
        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        /// <summary>
        /// builds the table "TireStorageItems" with TireStorageItem objects' fields as the rows.
        /// </summary>
        public DbSet<TireStorageItem> TireStorageItems { get; set; }

        /// <summary>
        /// Configures Entity relationships, Indexes and Seed data.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //this line is apart of the boiler plate code
            base.OnModelCreating(builder);

            //here is where we will begin our code configuring entity relationships, Db indexes and initial seed data

            //builder.Entity tells who we are giving a rule for, builder.Entity<model name here>()
            //this rule will prevent more than 1 instance of a date being closed. why list that we are closed christmas, twice, essentially.
            builder.Entity<ClosedDate>()
                //each closed date will have an index value
                .HasIndex(c => c.Date)
                //and that value is unique, it cannot have duplicates
                .IsUnique();


            //prevents a service category from being deleted, if appointments are still tied to it. 
            //i guess if you delete that category, it cascades and deletes all appointments tied to the category.
            builder.Entity<Appointment>()
                .HasOne(a => a.ServiceCategory)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //prevents SQL from running a loop on self referancing appointments that extend beyond a single day.
            builder.Entity<Appointment>()
                .HasOne(a => a.ParentAppointment)
                .WithMany()
                .HasForeignKey(a => a.ParentAppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            //seed the two specific service categories requested by the client
            builder.Entity<ServiceCategory>().HasData(
                //defines the "Oil" category of service
                new ServiceCategory
                {
                    ServiceCategoryId = 1,
                    Name = "Oil",
                    DefaultDurationHours = 1,
                    BaseEstimate = 0.00m,
                    IsActive = true
                },

                //defines the "Repair" category of service
                new ServiceCategory
                {
                    ServiceCategoryId = 2,
                    Name = "Repair",
                    DefaultDurationHours = 1,
                    BaseEstimate = 0.00m,
                    IsActive = true
                }
            );
        }
    }
    
}
