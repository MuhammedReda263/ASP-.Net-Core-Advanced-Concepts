﻿using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _context;
        public CountriesRepository(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<Country> AddCountry(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
          return await _context.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryId(Guid countryID)
        {
          return await _context.Countries.FirstOrDefaultAsync(temp=>temp.CountryID == countryID);

        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _context.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
