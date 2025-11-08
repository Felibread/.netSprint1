using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    tipo_alerta = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    titulo = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    mensagem = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    data_envio = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    expira_em = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localizacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    latitude = table.Column<double>(type: "REAL", precision: 8, scale: 4, nullable: false),
                    longitude = table.Column<double>(type: "REAL", precision: 8, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clima",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    data_hora = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    temp = table.Column<double>(type: "REAL", precision: 8, scale: 2, nullable: false),
                    temp_unit = table.Column<string>(type: "TEXT", nullable: false),
                    humidity = table.Column<double>(type: "REAL", nullable: false),
                    wind_speed = table.Column<double>(type: "REAL", precision: 8, scale: 2, nullable: false),
                    condition = table.Column<string>(type: "TEXT", nullable: false),
                    precip_prob = table.Column<double>(type: "REAL", precision: 6, scale: 4, nullable: false),
                    LocationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    id_localizacao = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clima", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clima_Localizacao_id_localizacao",
                        column: x => x.id_localizacao,
                        principalTable: "Localizacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "Alerta__IDX",
                table: "Alerta",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Clima__IDX",
                table: "Clima",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Clima_id_localizacao",
                table: "Clima",
                column: "id_localizacao");

            migrationBuilder.CreateIndex(
                name: "Localizacao__IDX",
                table: "Localizacao",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerta");

            migrationBuilder.DropTable(
                name: "Clima");

            migrationBuilder.DropTable(
                name: "Localizacao");
        }
    }
}
