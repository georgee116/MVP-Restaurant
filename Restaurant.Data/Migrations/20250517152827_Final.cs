using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Data.Migrations
{
    /// <inheritdoc />
    public partial class Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergeni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergeni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meniuri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meniuri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meniuri_Categorii_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categorii",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilizatori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresaLivrare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParolaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizatori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreparatAlergeni",
                columns: table => new
                {
                    PreparatId = table.Column<int>(type: "int", nullable: false),
                    AlergenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparatAlergeni", x => new { x.PreparatId, x.AlergenId });
                    table.ForeignKey(
                        name: "FK_PreparatAlergeni_Alergeni_AlergenId",
                        column: x => x.AlergenId,
                        principalTable: "Alergeni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreparatAlergeni_Preparate_PreparatId",
                        column: x => x.PreparatId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreparatMeniuri",
                columns: table => new
                {
                    MeniuId = table.Column<int>(type: "int", nullable: false),
                    PreparatId = table.Column<int>(type: "int", nullable: false),
                    CantitatePortieMeniu = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparatMeniuri", x => new { x.MeniuId, x.PreparatId });
                    table.ForeignKey(
                        name: "FK_PreparatMeniuri_Meniuri_MeniuId",
                        column: x => x.MeniuId,
                        principalTable: "Meniuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreparatMeniuri_Preparate_PreparatId",
                        column: x => x.PreparatId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comenzi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodUnic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataComenzii = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DiscountAplicat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostTransport = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OraEstimativaLivrare = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtilizatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comenzi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comenzi_Utilizatori_UtilizatorId",
                        column: x => x.UtilizatorId,
                        principalTable: "Utilizatori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComandaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComandaId = table.Column<int>(type: "int", nullable: false),
                    PreparatId = table.Column<int>(type: "int", nullable: true),
                    MeniuId = table.Column<int>(type: "int", nullable: true),
                    Cantitate = table.Column<int>(type: "int", nullable: false),
                    CantitatePortie = table.Column<float>(type: "real", nullable: false),
                    PretUnitate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComandaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComandaItems_Comenzi_ComandaId",
                        column: x => x.ComandaId,
                        principalTable: "Comenzi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComandaItems_Meniuri_MeniuId",
                        column: x => x.MeniuId,
                        principalTable: "Meniuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComandaItems_Preparate_PreparatId",
                        column: x => x.PreparatId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItems_ComandaId",
                table: "ComandaItems",
                column: "ComandaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItems_MeniuId",
                table: "ComandaItems",
                column: "MeniuId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItems_PreparatId",
                table: "ComandaItems",
                column: "PreparatId");

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_UtilizatorId",
                table: "Comenzi",
                column: "UtilizatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Meniuri_CategorieId",
                table: "Meniuri",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparatAlergeni_AlergenId",
                table: "PreparatAlergeni",
                column: "AlergenId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparatMeniuri_PreparatId",
                table: "PreparatMeniuri",
                column: "PreparatId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizatori_Email",
                table: "Utilizatori",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComandaItems");

            migrationBuilder.DropTable(
                name: "PreparatAlergeni");

            migrationBuilder.DropTable(
                name: "PreparatMeniuri");

            migrationBuilder.DropTable(
                name: "Comenzi");

            migrationBuilder.DropTable(
                name: "Alergeni");

            migrationBuilder.DropTable(
                name: "Meniuri");

            migrationBuilder.DropTable(
                name: "Utilizatori");
        }
    }
}
