using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorii",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorii", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preparate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantitatePortie = table.Column<float>(type: "real", nullable: false),
                    CantitateTotala = table.Column<float>(type: "real", nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preparate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Preparate_Categorii_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categorii",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImaginiPreparat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathImagine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreparatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImaginiPreparat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImaginiPreparat_Preparate_PreparatId",
                        column: x => x.PreparatId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImaginiPreparat_PreparatId",
                table: "ImaginiPreparat",
                column: "PreparatId");

            migrationBuilder.CreateIndex(
                name: "IX_Preparate_CategorieId",
                table: "Preparate",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImaginiPreparat");

            migrationBuilder.DropTable(
                name: "Preparate");

            migrationBuilder.DropTable(
                name: "Categorii");
        }
    }
}
