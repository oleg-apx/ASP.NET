using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Abstractions.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerWithDetailsAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllCustomersWithDetailsAsync();
        Task AddPreferenceToCustomerAsync(Guid customerId, Guid preferenceId);
        Task RemovePreferenceFromCustomerAsync(Guid customerId, Guid preferenceId);
    }
}
