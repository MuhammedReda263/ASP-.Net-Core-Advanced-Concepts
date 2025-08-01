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
 public class PersonsDeleterService : IPersonsDeleterService
 {
  //private field
  private readonly IPersonsRepository _personsRepository;
  private readonly ILogger<PersonsDeleterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
 

  //constructor
  public PersonsDeleterService(IPersonsRepository personRepository, ILogger<PersonsDeleterService> logger, IDiagnosticContext diagnosticContext)
  {
   _personsRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;

   
  }


  public async Task<bool> DeletePerson(Guid? personID)
  {
   if (personID == null)
   {
    throw new ArgumentNullException(nameof(personID));
   }

            Person? person = await _personsRepository.GetPersonByPersonId(personID.Value);
   if (person == null)
    return false;

    await _personsRepository.DeletePersonByPersonID(personID.Value);

   return true;
  }


 }
}
