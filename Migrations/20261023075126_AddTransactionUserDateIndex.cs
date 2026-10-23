using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionUserDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
            name: "IX_Transactions_UserId_Date",
            table: "Transactions",
            columns: new[] { "UserId", "Date" });

            migrationBuilder.DropIndex(
            name: "IX_Transactions_UserId",
            table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
            name: "IX_Transactions_UserId",
            table: "Transactions",
            column: "UserId");

            migrationBuilder.DropIndex(
            name: "IX_Transactions_UserId_Date",
            table: "Transactions");
        }
    }
}
