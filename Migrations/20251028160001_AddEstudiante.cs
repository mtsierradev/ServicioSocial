using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicioSocial.Migrations
{
    /// <inheritdoc />
    public partial class AddEstudiante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_AspNetUsers_ApplicationUserId",
                table: "Estudiantes");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Estudiantes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Estudiantes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Estudiantes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_AspNetUsers_ApplicationUserId",
                table: "Estudiantes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_AspNetUsers_ApplicationUserId",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Estudiantes");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Estudiantes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_AspNetUsers_ApplicationUserId",
                table: "Estudiantes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
