using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace microondas_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCustomizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Customizado",
                table: "ProgramasAquecimento",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ProgramasAquecimento",
                keyColumn: "Id",
                keyValue: 1,
                column: "Customizado",
                value: false);

            migrationBuilder.UpdateData(
                table: "ProgramasAquecimento",
                keyColumn: "Id",
                keyValue: 2,
                column: "Customizado",
                value: false);

            migrationBuilder.UpdateData(
                table: "ProgramasAquecimento",
                keyColumn: "Id",
                keyValue: 3,
                column: "Customizado",
                value: false);

            migrationBuilder.UpdateData(
                table: "ProgramasAquecimento",
                keyColumn: "Id",
                keyValue: 4,
                column: "Customizado",
                value: false);

            migrationBuilder.UpdateData(
                table: "ProgramasAquecimento",
                keyColumn: "Id",
                keyValue: 5,
                column: "Customizado",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customizado",
                table: "ProgramasAquecimento");
        }
    }
}
