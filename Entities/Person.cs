using Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(40)]
        public string? PersonName { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public GenderOptions? Gender { get; set; }

        public Guid? CountryID { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        [ForeignKey("CountryID")]
        public Country? Country { get; set; }
    }
}
