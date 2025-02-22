using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Customers_CustomerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Customers_CustomerId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CustomerId1",
                table: "Projects");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1001);

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Customers_CustomerId",
                table: "Projects",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Customers_CustomerId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "ContactPerson", "Name" },
                values: new object[] { 1001, "John Doe", "ABC Corp" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CustomerId1",
                table: "Projects",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Customers_CustomerId",
                table: "Projects",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Customers_CustomerId1",
                table: "Projects",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }
    }
}
