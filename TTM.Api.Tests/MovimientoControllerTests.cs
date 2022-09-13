using AutoMapper;

using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TTM.Api.Controllers;
using TTM.Api.DTOs;
using TTM.Api.Models;
using TTM.Api.Profiles;
using TTM.Api.UnitOfWork;

namespace TTM.Api.Tests
{
    public class MovimientoControllerTests
    {
        private static IMapper? _mapper;
        private static IConfiguration? _configRoot;

        public MovimientoControllerTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MoviminetoDtoProfile());
                });

                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;

            }
            _configRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        }
        [Fact]
        public async void Get_Returns_Ok_Response_List_of_cuentas_When_Data_Exist()
        {
            //Arrange
            int count = 5;
            var fakeClients = A.CollectionOfDummy<Movimiento>(count).AsEnumerable();
            var unitOfWork = A.Fake<IUnitOfWork>();



            A.CallTo(() => unitOfWork.Movimientos.GetAll()).Returns(Task.FromResult(fakeClients));
            var controller = new MovimientosController(unitOfWork, _configRoot!, _mapper!);

            //Act
            var actionResult = await controller.Get();

            //Assert
            var result = actionResult.Result as OkObjectResult;
            var returnClientes = result != null ? result.Value as IEnumerable<MovimientoDto> : null;
            Assert.Equal(count, returnClientes is not null ? returnClientes.Count() : 0);


        }

        [Fact]
        public async void Get_Returns_NoContent_Response_When_Data_Not_Exist()
        {
            //Arrange
            int count = 0;
            var fakeClients = A.CollectionOfDummy<Movimiento>(count).AsEnumerable();
            var unitOfWork = A.Fake<IUnitOfWork>();
            A.CallTo(() => unitOfWork.Movimientos.GetAll()).Returns(Task.FromResult(fakeClients));


            var controller = new MovimientosController(unitOfWork, _configRoot!, _mapper!);

            //Act
            var actionResult = await controller.Get();

            //Assert
            var result = actionResult.Result;
            Assert.IsType<NoContentResult>(result);


        }
    }
}