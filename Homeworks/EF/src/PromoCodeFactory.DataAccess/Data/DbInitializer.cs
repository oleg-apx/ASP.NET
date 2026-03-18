using System.Linq;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Data;

public static class DbInitializer
{
    public static void Initialize(DataContext context)
    {
        context.Database.Migrate();

        if (!context.Roles.Any())
            context.Roles.AddRange(FakeDataFactory.Roles);
        if (!context.Preferences.Any())
            context.Preferences.AddRange(FakeDataFactory.Preferences);
        if (!context.Employees.Any())
            context.Employees.AddRange(FakeDataFactory.Employees);
        if (!context.Customers.Any())
            context.Customers.AddRange(FakeDataFactory.Customers);

        context.SaveChanges();
    }
}