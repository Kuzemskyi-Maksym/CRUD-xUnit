using Entities.DTO;

namespace Entities
{
    /// <summary>
    /// Represents the business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest"></param>
        /// <returns></returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries
        /// </summary>
        /// <returns></returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns country by its ID
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns></returns>
        Task<CountryResponse?> GetCountryByID(Guid? countryID);
    }
}
