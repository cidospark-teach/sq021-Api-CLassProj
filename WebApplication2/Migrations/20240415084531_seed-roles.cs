using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class seedroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                                    INSERT INTO AspNetRoles(Id, Name, NormalizedName, ConcurrencyStamp)
                                    VALUES('1', 'regular', 'REGULAR', '{DateTime.Now}' ),
                                    ('2', 'admin', 'ADMIN', '{DateTime.Now}' )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
