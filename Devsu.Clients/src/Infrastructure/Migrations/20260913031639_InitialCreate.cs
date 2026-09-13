using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    person_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    identification_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.person_id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.client_id);
                    table.ForeignKey(
                        name: "fk_clients_persons_person_id",
                        column: x => x.client_id,
                        principalTable: "persons",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_persons_identification_number",
                table: "persons",
                column: "identification_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "persons");
        }
    }
}
