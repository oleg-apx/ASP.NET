using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        /// <summary>
        /// Фабричный метод для создания базового партнёра (Arrange).
        /// </summary>
        private Partner CreateBasePartner()
        {
            return new PartnerBuilder().Build();
        }

        /// <summary>
        /// Фабричный метод для создания запроса на установку лимита (Arrange).
        /// </summary>
        private SetPartnerPromoCodeLimitRequest CreateValidRequest()
        {
            return new SetPartnerPromoCodeLimitRequestBuilder().Build();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ПартнерНеНайден_ВозвращаетNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            Partner partner = null;
            var request = CreateValidRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ПартнерЗаблокирован_ВозвращаетBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = CreateBasePartner();
            partner.IsActive = false;
            var request = CreateValidRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("Данный партнер не активен");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ЛимитЗакончился_КоличествоПромокодовНеОбнуляется()
        {
            // Arrange: предыдущий лимит с EndDate в прошлом
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithNumberIssuedPromoCodes(50)
                .WithActiveLimitEndDate(DateTime.Now.AddDays(-1))
                .Build();
            var request = CreateValidRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(50);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ПредыдущийЛимитАктивен_ОбнуляетКоличествоПромокодов()
        {
            // Arrange: предыдущий лимит с EndDate в будущем
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithNumberIssuedPromoCodes(30)
                .WithActiveLimitEndDate(DateTime.Now.AddDays(10))
                .Build();
            var request = CreateValidRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ПриУстановкеЛимита_ОтключаетПредыдущийЛимит()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var previousLimitId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithActiveLimit(previousLimitId, DateTime.Now.AddDays(5))
                .Build();
            var request = CreateValidRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var previousLimit = partner.PartnerLimits.FirstOrDefault(x => x.Id == previousLimitId);
            previousLimit.Should().NotBeNull();
            previousLimit!.CancelDate.Should().BeNull();

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            previousLimit.CancelDate.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ЛимитРавенНулю_ВозвращаетBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = CreateBasePartner();
            var request = new SetPartnerPromoCodeLimitRequestBuilder().WithLimit(0).Build();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            (result as BadRequestObjectResult)!.Value.Should().Be("Лимит должен быть больше 0");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ЛимитОтрицательный_ВозвращаетBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = CreateBasePartner();
            var request = new SetPartnerPromoCodeLimitRequestBuilder().WithLimit(-5).Build();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_КорректныйЗапрос_СохраняетНовыйЛимитВБазеДанных()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).Build();
            var request = new SetPartnerPromoCodeLimitRequestBuilder()
                .WithLimit(100)
                .WithEndDate(DateTime.Now.AddMonths(1))
                .Build();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask);

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            _partnersRepositoryMock.Verify(
                repo => repo.UpdateAsync(It.Is<Partner>(p =>
                    p.Id == partnerId &&
                    p.PartnerLimits.Any(l =>
                        l.Limit == request.Limit &&
                        l.EndDate == request.EndDate &&
                        l.PartnerId == partnerId))),
                Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_КорректныйЗапрос_ВозвращаетCreatedAtAction()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).Build();
            var request = CreateValidRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAt = result as CreatedAtActionResult;
            createdAt!.ActionName.Should().Be(nameof(PartnersController.GetPartnerLimitAsync));
            createdAt.RouteValues!["id"].Should().Be(partnerId);
            createdAt.RouteValues.Should().ContainKey("limitId");
        }
    }

    #region Builders (тестовые данные через Builder)

    public class PartnerBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "Тестовый партнёр";
        private int _numberIssuedPromoCodes = 0;
        private bool _isActive = true;
        private List<PartnerPromoCodeLimit> _limits = new();

        public PartnerBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public PartnerBuilder WithNumberIssuedPromoCodes(int count)
        {
            _numberIssuedPromoCodes = count;
            return this;
        }

        public PartnerBuilder WithActiveLimitEndDate(DateTime endDate)
        {
            _limits = new List<PartnerPromoCodeLimit>
            {
                new PartnerPromoCodeLimit
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.Now.AddDays(-10),
                    EndDate = endDate,
                    Limit = 100
                }
            };
            return this;
        }

        public PartnerBuilder WithActiveLimit(Guid limitId, DateTime endDate)
        {
            _limits = new List<PartnerPromoCodeLimit>
            {
                new PartnerPromoCodeLimit
                {
                    Id = limitId,
                    CreateDate = DateTime.Now.AddDays(-5),
                    EndDate = endDate,
                    Limit = 50
                }
            };
            return this;
        }

        public Partner Build()
        {
            var partner = new Partner
            {
                Id = _id,
                Name = _name,
                NumberIssuedPromoCodes = _numberIssuedPromoCodes,
                IsActive = _isActive,
                PartnerLimits = _limits
            };
            foreach (var limit in _limits)
            {
                limit.PartnerId = partner.Id;
                limit.Partner = partner;
            }
            return partner;
        }
    }

    public class SetPartnerPromoCodeLimitRequestBuilder
    {
        private int _limit = 100;
        private DateTime _endDate = DateTime.Now.AddMonths(1);

        public SetPartnerPromoCodeLimitRequestBuilder WithLimit(int limit)
        {
            _limit = limit;
            return this;
        }

        public SetPartnerPromoCodeLimitRequestBuilder WithEndDate(DateTime endDate)
        {
            _endDate = endDate;
            return this;
        }

        public SetPartnerPromoCodeLimitRequest Build()
        {
            return new SetPartnerPromoCodeLimitRequest
            {
                Limit = _limit,
                EndDate = _endDate
            };
        }
    }

    #endregion
}
