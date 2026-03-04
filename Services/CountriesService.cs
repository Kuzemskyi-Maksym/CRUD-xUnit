using Entities;
using Entities.DTO;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;

        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation
            if (countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if (await _db.Countries.CountAsync(country => country.CountryName == countryAddRequest.CountryName) > 0)
                throw new ArgumentException("Given county name already exists");

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByID(Guid? ID)
        {
            if (ID == null) return null;

            Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == ID);

            if (country_response_from_list == null) return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}
