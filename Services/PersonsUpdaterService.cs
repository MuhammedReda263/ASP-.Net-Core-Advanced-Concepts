﻿using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using Services.Helpers;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
 public class PersonsUpdaterService : IPersonsUpdaterService
 {
  //private field
  private readonly IPersonsRepository _personsRepository;
  private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
 

  //constructor
  public PersonsUpdaterService(IPersonsRepository personRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
  {
   _personsRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;

   
  }
  public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
  {
   if (personUpdateRequest == null)
    throw new ArgumentNullException(nameof(Person));

   //validation
   ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object to update
            Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonID);
   if (matchingPerson == null)
   {
    throw new InvalidPersonIDException("Given person id doesn't exist");
   }

   //update all details
   matchingPerson.PersonName = personUpdateRequest.PersonName;
   matchingPerson.Email = personUpdateRequest.Email;
   matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
   matchingPerson.Gender = personUpdateRequest.Gender.ToString();
   matchingPerson.CountryID = personUpdateRequest.CountryID;
   matchingPerson.Address = personUpdateRequest.Address;
   matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

   await _personsRepository.UpdatePerson(matchingPerson); //UPDATE

   return matchingPerson.ToPersonResponse();
  }

 
 }
}
