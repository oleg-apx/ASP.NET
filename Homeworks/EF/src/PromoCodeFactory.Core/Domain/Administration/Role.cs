using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.Administration
{
    [Table("Role")]
    public class Role
        : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Навигационное свойство для сотрудников
        /// </summary>
        public ICollection<Employee> Employees { get; set; }
    }
}