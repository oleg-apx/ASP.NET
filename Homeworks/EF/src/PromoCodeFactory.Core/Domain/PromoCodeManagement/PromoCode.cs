using System;
using System.ComponentModel.DataAnnotations.Schema;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{

    [Table("PromoCode")]
    public class PromoCode
        : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public int MaxUsageCount { get; set; }

        /// <summary>
        /// Связь с Preference
        /// </summary>
        public int? PreferenceId { get; set; }
        public Preference Preference { get; set; }

        /// <summary>
        /// Связь с Customer (один-ко-многим)
        /// </summary>
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        /// <summary>
        /// Связь с Employee (кто создал промокод)
        /// </summary>
        public int? CreatedByEmployeeId { get; set; }
        public Employee CreatedByEmployee { get; set; }
    }
}