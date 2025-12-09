using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class CustomerPreference
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid PreferenceId { get; set; }
        public Preference Preference { get; set; }

        /// <summary>
        /// Дополнительные поля для связи
        /// </summary>
        public DateTime AssignedDate { get; set; }

        /// <summary>
        /// Имя сотрудника или система
        /// </summary>
        public string AssignedBy { get; set; } 

        public DateTime CreatedDate { get; set; }

    }
}
