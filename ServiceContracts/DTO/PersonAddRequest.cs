using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Entities.Enums;

namespace Entities.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person's Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Person's Email can't be blank")]
        public string? Email {  get; set; }

        [Required(ErrorMessage = "Date of birth can't be blank")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender can't be blank")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Country can't be blank")]
        public Guid? CountryID { get; set; }

        [Required(ErrorMessage = "Address can't be blank")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Choose truy or false")]
        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person()
            {
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
}
