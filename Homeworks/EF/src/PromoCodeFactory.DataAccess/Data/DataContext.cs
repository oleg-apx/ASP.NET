using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настраиваем классы 
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

              
                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(r => r.Description)
                    .HasMaxLength(500);
            });

           
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(c => c.LastName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(c => c.Phone)
                    .HasMaxLength(20);

                
                entity.HasMany(c => c.Preferences)
                    .WithMany(p => p.Customers)
                    .UsingEntity<CustomerPreference>(
                        j => j.HasOne(cp => cp.Preference)
                            .WithMany()
                            .HasForeignKey(cp => cp.PreferenceId),
                        j => j.HasOne(cp => cp.Customer)
                            .WithMany()
                            .HasForeignKey(cp => cp.CustomerId),
                        j =>
                        {
                            j.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });
                        });

                
                entity.HasMany(c => c.PromoCodes)
                    .WithOne(p => p.Customer)
                    .HasForeignKey(p => p.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

           
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(p => p.Description)
                    .HasMaxLength(500);
            });

            
            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

                entity.HasOne(cp => cp.Customer)
                    .WithMany(c => c.CustomerPreferences)
                    .HasForeignKey(cp => cp.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Preference)
                    .WithMany(p => p.CustomerPreferences)
                    .HasForeignKey(cp => cp.PreferenceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

           
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Code)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(p => p.Description)
                    .HasMaxLength(500);

                
                entity.HasOne(p => p.Preference)
                    .WithMany()
                    .HasForeignKey(p => p.PreferenceId)
                    .OnDelete(DeleteBehavior.Restrict);

               
                entity.HasOne(p => p.Customer)
                    .WithMany(c => c.PromoCodes)
                    .HasForeignKey(p => p.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                //Индексы для связанных объектов
                entity.HasIndex(p => p.Code).IsUnique();
                entity.HasIndex(p => p.ExpiryDate);
            });
        }
    }
}
