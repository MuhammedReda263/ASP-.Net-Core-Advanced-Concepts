﻿using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using AutoFixture.Kernel;
using System.Linq.Expressions;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    //private fields
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;
    private readonly IPersonsRepository _personsRepository;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
            _fixture = new Fixture();
            var countriesInitialData = new List<Country>();
            var personsInitialData = new List<Person>();
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            ApplicationDbContext dbContext = dbContextMock.Object;
            _countriesService = new CountriesService(null);
            _personService = new PersonsService(_personsRepository);
      
      _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact] 
    public async Task AddPerson_NullPersonToBeArgumentNullException()
    {
      //Arrange
      PersonAddRequest? personAddRequest = null;

      //Act
      //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
      //{
      //  await _personService.AddPerson(personAddRequest);
      //});

     Func<Task> acutal = async () =>
      {
        await _personService.AddPerson(personAddRequest);
      };

           await acutal.Should().ThrowAsync<ArgumentNullException>();

    
    }


    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {
      //Arrange
      PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp=> temp.PersonName, null
      as string).Create();

            Person person = personAddRequest.ToPerson();

            //Act
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //  await _personService.AddPerson(personAddRequest);
            //});

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

           await action.Should().ThrowAsync<ArgumentException>();
        }

    //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp=> temp.Email , "Example@gmail.com").Create();
            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();
            _personsRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

            person_response_expected.PersonID = person_response_from_add.PersonID;

     

            //Assert
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

          
    }

    #endregion


    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
    {
      //Arrange
      Guid? personID = null;

      //Act
      PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

      //Assert
      //Assert.Null(person_response_from_get);
      person_response_from_get.Should().BeNull();
    }


    //If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
    {
      //Arange
     
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "email@sample.com").With(temp => temp.Country, null as Country).Create();
            PersonResponse person_responce_expected = person.ToPersonResponse();
           _personsRepositoryMock.Setup(temp=> temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person.PersonID);

            //Assert

            person_response_from_get.Should().Be(person_responce_expected);
    }

    #endregion


    #region GetAllPersons

    //The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_EmptyList_ToBeEmpty_ToBeSuccessful()
    {
            var persons = new List<Person>();
            _personsRepositoryMock
             .Setup(temp => temp.GetAllPersons())
             .ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }


    //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
      //Arrange
     

      Person person_1 = _fixture.Build<Person>().With(temp => temp.Email, "smith@example.com").With(temp=>temp.Country,null as Country).Create();     
      Person person_2 = _fixture.Build<Person>().With(temp => temp.Email, "mary@example.com").With(temp => temp.Country, null as Country).Create();
      Person person_3 = _fixture.Build<Person>().With(temp => temp.Email, "rahman@example.com").With(temp => temp.Country, null as Country).Create();

      List<Person> persons = new List<Person>() { person_1, person_2, person_3 };

      List<PersonResponse> person_response_list_expected = persons.Select(temp=>temp.ToPersonResponse()).ToList();


      //print person_response_list_from_add
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

      _personsRepositoryMock.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(persons);

      //Act
      List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_get)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //  Assert.Contains(person_response_from_add, persons_list_from_get);
            //}

            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
    }
    #endregion


    #region GetFilteredPersons

    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
    {
            Person person_1 = _fixture.Build<Person>().With(temp => temp.Email, "smith@example.com").With(temp => temp.Country, null as Country).Create();
            Person person_2 = _fixture.Build<Person>().With(temp => temp.Email, "mary@example.com").With(temp => temp.Country, null as Country).Create();
            Person person_3 = _fixture.Build<Person>().With(temp => temp.Email, "rahman@example.com").With(temp => temp.Country, null as Country).Create();

            List<Person> persons = new List<Person>() { person_1, person_2, person_3 };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_expected in person_response_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_from_expected.ToString());
      }

      _personsRepositoryMock.Setup(temp=>temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(persons);

      //Act
      List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      //{
      //  Assert.Contains(person_response_from_add, persons_list_from_search);
      //}

            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
    }


    //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
            Person person_1 = _fixture.Build<Person>().With(temp => temp.Email, "smith@example.com").With(temp => temp.Country, null as Country).Create();
            Person person_2 = _fixture.Build<Person>().With(temp => temp.Email, "mary@example.com").With(temp => temp.Country, null as Country).Create();
            Person person_3 = _fixture.Build<Person>().With(temp => temp.Email, "rahman@example.com").With(temp => temp.Country, null as Country).Create();

            List<Person> persons = new List<Person>() { person_1, person_2, person_3 };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_expected in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_expected.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //  Assert.Contains(person_response_from_add, persons_list_from_search);
            //}

            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

    #endregion


    #region GetSortedPersons

    //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
            Person person_1 = _fixture.Build<Person>().With(temp => temp.Email, "smith@example.com").With(temp => temp.Country, null as Country).Create();
            Person person_2 = _fixture.Build<Person>().With(temp => temp.Email, "mary@example.com").With(temp => temp.Country, null as Country).Create();
            Person person_3 = _fixture.Build<Person>().With(temp => temp.Email, "rahman@example.com").With(temp => temp.Country, null as Country).Create();

            List<Person> persons = new List<Person>() { person_1, person_2, person_3 };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

    
      //print person_response_list_from_expected
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
      List<PersonResponse> allPersons = await _personService.GetAllPersons();

      //Act
      List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_sort)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }
      //person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

      ////Assert
      //for (int i = 0; i < person_response_list_from_add.Count; i++)
      //{
      //  Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
      //}

            persons_list_from_sort.Should().BeInDescendingOrder(temp=>temp.PersonName);

    }
    #endregion


    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = null;

      //Assert
      //await Assert.ThrowsAsync<ArgumentNullException>(async () => {
      //  //Act
      //  await _personService.UpdatePerson(person_update_request);
      //});

      var action = async () => {
        //Act
        await _personService.UpdatePerson(person_update_request);
      };
          await action.Should().ThrowAsync<ArgumentNullException>();


    }


    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = _fixture.Create<PersonUpdateRequest>();

      //Assert
      //await Assert.ThrowsAsync<ArgumentException>(async() => {
      //  //Act
      //  await _personService.UpdatePerson(person_update_request);
      //});  

       var action = async() => {
        //Act
        await _personService.UpdatePerson(person_update_request);
      };
            await action.Should().ThrowAsync<ArgumentException>();
    }


    //When PersonName is null, it should throw ArgumentException
    [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.PersonName, null as string)
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();


            //Act
            var action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }



        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personsRepositoryMock
             .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);
        }

        #endregion


        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Female")
             .Create();


            _personsRepositoryMock
             .Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
             .ReturnsAsync(true);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            bool isDeleted = await _personService.DeletePerson(person.PersonID);

            //Assert
            isDeleted.Should().BeTrue();
        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}
