using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System.Linq;

public static class DbInitializer
{
    public static void Initialize(DataContext context)
    {
        // Удаляем базу, если она существует, и создаём заново
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Заполнение тестовыми данными
        if (!context.Set<Employee>().Any())
        {
            context.Set<Employee>().AddRange(FakeDataFactory.Employees);
        }
        if (!context.Set<Role>().Any())
        {
            context.Set<Role>().AddRange(FakeDataFactory.Roles);
        }
        if (!context.Set<Preference>().Any())
        {
            context.Set<Preference>().AddRange(FakeDataFactory.Preferences);
        }
        if (!context.Set<Customer>().Any())
        {
            context.Set<Customer>().AddRange(FakeDataFactory.Customers);
        }

        context.SaveChanges();
    }
}