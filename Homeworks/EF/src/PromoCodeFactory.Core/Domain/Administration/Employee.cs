using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;

namespace PromoCodeFactory.Core.Domain.Administration
{

    [Table("Employee")]
    public class Employee
        : BaseEntity
    {   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime HireDate { get; set; }

        public string FullName => $"{LastName} {FirstName}";

        /// <summary>
        /// Связь с Role
        /// </summary>
        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        /// <summary>
        /// Навигационное свойство для промокодов, созданных сотрудником
        /// </summary>
        public ICollection<PromoCode> CreatedPromoCodes { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }
}