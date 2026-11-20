using System.ComponentModel.DataAnnotations;

namespace OSAutoRepair.Models
{
    /// <summary>
    /// builds the seasonal tires that the legacy customers have stored at the shop. The practice has been discontinued.
    /// </summary>
    public class TireStorageItem
    {
        
        /// <summary>
        /// give the tire an Id that should be associated to a specific customer
        /// </summary>
        [Key]
        public int TireStorageItemId { get; set; }

        /// <summary>
        /// gets/sets the customers name who owns the tire
        /// </summary>
        [StringLength(100)]
        public string? CustomerName { get; set; }

        /// <summary>
        /// defines the customers phone number
        /// </summary>
        [Required, Phone, StringLength(25)]
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// optionally allows for the defintion of the customers email
        /// </summary>
        [EmailAddress, StringLength(100)]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// defines an optional description of the tires
        /// </summary>
        [StringLength(250)]
        public string? TireDescription { get; set; }

        /// <summary>
        /// defines how many tires are in this set that belong to the customer
        /// </summary>
        [Range(1, 8)]
        public int Quantity { get; set; } = 1;
    }
}
