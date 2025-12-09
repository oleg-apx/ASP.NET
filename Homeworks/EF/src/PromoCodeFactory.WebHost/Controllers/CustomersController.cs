using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Abstractions.Interfaces;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly DataContext _context;

        public CustomersController(
            ICustomerRepository customerRepository,
            IRepository<Preference> preferenceRepository,
            DataContext context)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _context = context;
        }



        /// <summary>
        /// Получить список всех клиентов (краткая информация)
        /// </summary>
        /// <returns>Список клиентов с краткой информацией</returns>
        /// <response code="200">Успешно возвращен список клиентов</response>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersWithDetailsAsync();

            var response = customers.Select(c => new CustomerShortResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,               
            }).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Получить клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Полная информация о клиенте с промокодами</returns>
        /// <response code="200">Клиент найден</response>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetCustomerWithDetailsAsync(id);

            if (customer == null)
                return NotFound($"Клиент с ID {id} не найден");

            var response = MapToCustomerResponse(customer);
            return Ok(response);
        }


        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request">Данные нового клиента</param>
        /// <returns>Созданный клиент</returns>
        /// <response code="201">Клиент успешно создан</response>
        /// <response code="400">Некорректные данные клиента</response>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Проверяем уникальность email
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (existingCustomer != null)
                return BadRequest($"Клиент с email {request.Email} уже существует");

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = string.Empty,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true,
                Preferences = new List<Preference>()
            };

            // Добавляем предпочтения если они указаны
            if (request.PreferenceIds != null && request.PreferenceIds.Any())
            {
                var preferences = await _context.Preferences
                    .Where(p => request.PreferenceIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var preference in preferences)
                {
                    customer.Preferences.Add(preference);
                }
            }

            var createdCustomer = await _customerRepository.AddAsync(customer);

            // Загружаем связанные данные для ответа
            await _context.Entry(createdCustomer)
                .Collection(c => c.Preferences)
                .LoadAsync();

            await _context.Entry(createdCustomer)
                .Collection(c => c.PromoCodes)
                .LoadAsync();

            var response = MapToCustomerResponse(createdCustomer);
            return CreatedAtAction(nameof(GetCustomerAsync), new { id = createdCustomer.Id }, response);
        }


        /// <summary>
        /// Обновить данные клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Обновленные данные клиента</param>
        /// <response code="204">Клиент успешно обновлен</response>
        /// <response code="400">Некорректные данные клиента</response>
        /// <response code="404">Клиент не найден</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _customerRepository.GetCustomerWithDetailsAsync(id);

            if (customer == null)
                return NotFound($"Клиент с ID {id} не найден");

            // Проверяем уникальность email (если изменился)
            if (customer.Email != request.Email)
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == request.Email && c.Id != id);

                if (existingCustomer != null)
                    return BadRequest($"Клиент с email {request.Email} уже существует");
            }

            // Обновляем основные данные
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.UpdatedDate = DateTime.UtcNow;

            // Обновляем предпочтения
            if (request.PreferenceIds != null)
            {
                // Получаем текущие предпочтения клиента
                await _context.Entry(customer)
                    .Collection(c => c.Preferences)
                    .LoadAsync();

                // Получаем новые предпочтения
                var newPreferences = await _context.Preferences
                    .Where(p => request.PreferenceIds.Contains(p.Id))
                    .ToListAsync();

                // Очищаем текущие предпочтения и добавляем новые
                customer.Preferences.Clear();
                foreach (var preference in newPreferences)
                {
                    customer.Preferences.Add(preference);
                }
            }

            await _customerRepository.UpdateAsync(customer);
            return NoContent();
        }



        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <remarks>При удалении клиента также удаляются все связанные с ним промокоды</remarks>
        /// <response code="204">Клиент успешно удален</response>
        /// <response code="404">Клиент не найден</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound($"Клиент с ID {id} не найден");

            // При использовании каскадного удаления в EF Core, 
            // все связанные промокоды удалятся автоматически
            await _customerRepository.DeleteAsync(id);
            return NoContent();
        }



        private CustomerResponse MapToCustomerResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
               
                PromoCodes = customer.PromoCodes?.Select(pc => new PromoCodeShortResponse
                {
                    Id = pc.Id,
                    Code = pc.Code,
                    BeginDate = pc.StartDate.ToShortDateString(),
                    
                }).ToList() ?? new List<PromoCodeShortResponse>()
            };
        }
    }
}