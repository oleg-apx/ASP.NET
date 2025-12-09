using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    [Table("Customer")]
    public class Customer
        : BaseEntity
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Многие-ко-многим связь с Preference через CustomerPreference
        /// </summary>
        public ICollection<Preference> Preferences { get; set; }
        public ICollection<CustomerPreference> CustomerPreferences { get; set; }

        /// <summary>
        /// один-ко-многим связь с PromoCode
        /// </summary>
        public ICollection<PromoCode> PromoCodes { get; set; }
    }
}