using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создание сотрудника
        /// </summary>
        /// <returns>Созданного сотрудника или ошибку</returns>
        [HttpPost("{employee:Employee}")]
        public async Task<ActionResult<Employee>> CreateEmployeeAsync(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEmployee = await _employeeRepository.CreateAsync(employee);

            return employee;
        }

        /// <summary>
        /// Обновление сотрудника
        /// </summary>
        /// <returns>Обновленного сотрудника или ошибку</returns>
        [HttpPost("{empl:Employee}")]

        public async Task<ActionResult<Employee>> UpdateEmployeeAsync(Employee empl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var e = await _employeeRepository.GetByIdAsync(empl.Id);
            if (e == null)
                return NotFound();

            await _employeeRepository.UpdateAsync(empl);
            return empl;
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <returns>список без удаленного сотрудника </returns>
        [HttpPost("{id:guid}")]
        public async Task<List<EmployeeShortResponse>> DeleteEmployeeAsync(Guid id)
        {

            var employees = await _employeeRepository.DeleteAsync(id);
            var result = employees.Select(x=>
            new EmployeeShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
            }).ToList();
            return result;
        }






    }
}