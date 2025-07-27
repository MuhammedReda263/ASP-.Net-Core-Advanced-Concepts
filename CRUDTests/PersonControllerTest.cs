﻿using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTests
{
    public class PersonControllerTest
    {
        private readonly IPersonsAdderService _personsAdderService;
		private readonly IPersonsGetterService _personsGetterService;
		private readonly IPersonsDeleterService _personsDeleterService;
		private readonly IPersonsUpdaterService _personsUpdaterService;
		private readonly IPersonsSorterService _personsSorterService;

		private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
    
		private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
		private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
		private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
		private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
		private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;

		private readonly Mock <ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly IFixture _fixture;
        
        public PersonControllerTest()
        {
			_personsGetterServiceMock = new Mock<IPersonsGetterService>();
			_personsAdderServiceMock = new Mock<IPersonsAdderService>();
			_personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
			_personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
			_personsSorterServiceMock = new Mock<IPersonsSorterService>();

			_personsGetterService = _personsGetterServiceMock.Object;
			_personsAdderService = _personsAdderServiceMock.Object;
			_personsUpdaterService = _personsUpdaterServiceMock.Object;
			_personsDeleterService = _personsDeleterServiceMock.Object;
			_personsSorterService = _personsSorterServiceMock.Object;

			_loggerMock = new Mock<ILogger<PersonsController>>();
            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
            _logger = _loggerMock.Object;
            _fixture = new Fixture();
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsAdderService,_personsGetterService, _personsSorterService,_personsDeleterService,_personsUpdaterService, _countriesService, _logger);

            _personsGetterServiceMock
             .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(persons_response_list);

            _personsSorterServiceMock
             .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
             .ReturnsAsync(persons_response_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }
        #endregion

        #region Create

       
       

        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countries);

			_personsAdderServiceMock
			 .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsAdderService, _personsGetterService, _personsSorterService, _personsDeleterService, _personsUpdaterService, _countriesService, _logger);


			//Act
			IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
