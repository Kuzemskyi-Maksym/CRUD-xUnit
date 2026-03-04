using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Persons",
                newName: "ActualAddress");

            migrationBuilder.AlterColumn<string>(
                name: "ActualAddress",
                table: "Persons",
                type: "varchar(60)",
                maxLength: 200,
                nullable: true,
                defaultValue: "Academian street",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActualAddress",
                table: "Persons",
                newName: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Persons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(60)",
                oldMaxLength: 200,
                oldNullable: true,
                oldDefaultValue: "Academian street");
        }
    }
}
