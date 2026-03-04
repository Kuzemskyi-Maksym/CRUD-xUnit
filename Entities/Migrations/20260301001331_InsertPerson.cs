using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_AddPerson = @"
                CREATE PROCEDURE [dbo].[InsertPerson]
                (@PersonID uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40), @DateOfBirth datetime2(7),
                @Gender int, @CountryID uniqueidentifier, @Address nvarchar(200), @ReceiveNewsLetters bit)
                AS BEGIN

                    INSERT INTO [dbo].[Persons](PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters)
                    VALUES (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters)          
    
                END
                ";
            migrationBuilder.Sql(sp_AddPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_AddPerson = @"
                DROP PROCEDURE [dbo].[InsertPerson]";
            migrationBuilder.Sql(sp_AddPerson);
        }
    }
}
