using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Abstractions.Interfaces;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerRepository : EfRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DataContext context) : base(context)
        {
        }

        public async Task<Customer> GetCustomerWithDetailsAsync(Guid id)
        {
            return await _context.Customers
                .Include(c => c.Preferences)
                .Include(c => c.PromoCodes)
                    .ThenInclude(p => p.Preference)
                .Include(c => c.PromoCodes)
                    .ThenInclude(p => p.CreatedByEmployee)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersWithDetailsAsync()
        {
            return await _context.Customers
                .Include(c => c.Preferences)
                .Include(c => c.PromoCodes)
                .ToListAsync();
        }

        public override async Task DeleteAsync(Guid id)
        {
            // При удалении клиента каскадно удаляются его промокоды
            var customer = await GetCustomerWithDetailsAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPreferenceToCustomerAsync(Guid customerId, Guid preferenceId)
        {
            var customer = await _context.Customers
                .Include(c => c.Preferences)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            var preference = await _context.Preferences.FindAsync(preferenceId);

            if (customer != null && preference != null &&
                !customer.Preferences.Any(p => p.Id == preferenceId))
            {
                customer.Preferences.Add(preference);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePreferenceFromCustomerAsync(Guid customerId, Guid preferenceId)
        {
            var customerPreference = await _context.CustomerPreferences
                .FirstOrDefaultAsync(cp => cp.CustomerId == customerId && cp.PreferenceId == preferenceId);

            if (customerPreference != null)
            {
                _context.CustomerPreferences.Remove(customerPreference);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByPreferenceAsync(Guid preferenceId)
        {
            return await _context.Customers
                .Include(c => c.Preferences)
                .Where(c => c.Preferences.Any(p => p.Id == preferenceId))
                .ToListAsync();
        }
    }
}
