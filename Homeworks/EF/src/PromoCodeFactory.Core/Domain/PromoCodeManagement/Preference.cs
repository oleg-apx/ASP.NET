using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{

    [Table("Preference")]
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Один-ко-многим связь с Customer через CustomerPreference
        /// </summary>
        public ICollection<Customer> Customers { get; set; }
        public ICollection<CustomerPreference> CustomerPreferences { get; set; }

        /// <summary>
        /// Один-ко-многим связь с PromoCode
        /// </summary>
        public ICollection<PromoCode> PromoCodes { get; set; }
        public bool IsActive { get; set; }
    }
}