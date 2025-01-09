﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentSummatyCorrectionchangedYearTOExpYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentSummary_Year",
                table: "Orders",
                newName: "PaymentSummary_ExpYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentSummary_ExpYear",
                table: "Orders",
                newName: "PaymentSummary_Year");
        }
    }
}