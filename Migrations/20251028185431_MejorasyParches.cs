using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicioSocial.Migrations
{
    /// <inheritdoc />
    public partial class MejorasyParches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actividades_Estudiantes_EstudianteId",
                table: "Actividades");

            migrationBuilder.DropColumn(
                name: "MinutosDedicados",
                table: "Actividades");

            migrationBuilder.AlterColumn<int>(
                name: "EstudianteId",
                table: "Actividades",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DocenteAprobadorId",
                table: "Actividades",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Actividades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ComentarioDocente",
                table: "Actividades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "HorasDedicadas",
                table: "Actividades",
                type: "decimal(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Actividades",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_UserId",
                table: "Actividades",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actividades_AspNetUsers_UserId",
                table: "Actividades",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Actividades_Estudiantes_EstudianteId",
                table: "Actividades",
                column: "EstudianteId",
                principalTable: "Estudiantes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actividades_AspNetUsers_UserId",
                table: "Actividades");

            migrationBuilder.DropForeignKey(
                name: "FK_Actividades_Estudiantes_EstudianteId",
                table: "Actividades");

            migrationBuilder.DropIndex(
                name: "IX_Actividades_UserId",
                table: "Actividades");

            migrationBuilder.DropColumn(
                name: "HorasDedicadas",
                table: "Actividades");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Actividades");

            migrationBuilder.AlterColumn<int>(
                name: "EstudianteId",
                table: "Actividades",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocenteAprobadorId",
                table: "Actividades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Actividades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "ComentarioDocente",
                table: "Actividades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinutosDedicados",
                table: "Actividades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Actividades_Estudiantes_EstudianteId",
                table: "Actividades",
                column: "EstudianteId",
                principalTable: "Estudiantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
