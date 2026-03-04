using Entities.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;

            return PersonID == person.PersonID 
                && PersonName == person.PersonName
                && Email == person.Email
                && DateOfBirth == person.DateOfBirth
                && Gender == person.Gender
                && CountryID == person.CountryID
                && Country == person.Country
                && Address == person.Address
                && ReceiveNewsLetters == person.ReceiveNewsLetters
                && Age == person.Age;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersoID: {PersonID}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth?.ToString()}, " +
                $"Gender: {Gender?.ToString()}, CountryID: {CountryID}, Country: {Country}, Address: {Address}, " +
                $"ReceiveNewsLetters: {ReceiveNewsLetters}, Age: {Age}";
            ;
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender,
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }

    public static class PersonExtension
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null)? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Country = person.Country?.CountryName,
            };
        }
    }
}
