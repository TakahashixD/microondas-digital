using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace microondas_backend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramasAquecimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Alimento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tempo = table.Column<int>(type: "int", nullable: false),
                    Potencia = table.Column<int>(type: "int", nullable: false),
                    CaractereAquecimento = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Instrucoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasAquecimento", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProgramasAquecimento",
                columns: new[] { "Id", "Alimento", "CaractereAquecimento", "Instrucoes", "Nome", "Potencia", "Tempo" },
                values: new object[,]
                {
                    { 1, "Pipoca (de micro-ondas)", "*", "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.", "Pipoca", 7, 180 },
                    { 2, "Leite", "~", "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.", "Leite", 5, 300 },
                    { 3, "Carne em pedaço ou fatias", "#", "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.", "Carnes de boi", 4, 840 },
                    { 4, "Frango (qualquer corte)", "+", "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.", "Frango", 7, 480 },
                    { 5, "Feijão congelado", "@", "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.", "Feijão", 9, 480 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAquecimento_CaractereAquecimento",
                table: "ProgramasAquecimento",
                column: "CaractereAquecimento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAquecimento_Nome",
                table: "ProgramasAquecimento",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramasAquecimento");
        }
    }
}
