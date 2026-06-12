using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Walmart.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSaleColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SaleEndDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalePercentage",
                table: "Products");
        }
    }
}
