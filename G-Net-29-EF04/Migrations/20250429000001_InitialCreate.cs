using System;
using G_Net_29_EF04.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace G_Net_29_EF04.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(BankDbContext))]
    [Migration("20250429000001_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managers_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountNumber);
                    table.ForeignKey(
                        name: "FK_Accounts_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAccounts",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OwnershipType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnershipStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAccounts", x => new { x.CustomerId, x.AccountNumber });
                    table.ForeignKey(
                        name: "FK_CustomerAccounts_Accounts_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAccounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionNumber);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            // Unique Indexes
            migrationBuilder.CreateIndex(
                name: "IX_Branches_Code",
                table: "Branches",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NationalId",
                table: "Customers",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Managers_BranchId",
                table: "Managers",
                column: "BranchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BranchId",
                table: "Accounts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_AccountNumber",
                table: "CustomerAccounts",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountNumber",
                table: "Transactions",
                column: "AccountNumber");

            // ── Seed Branches ──
            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Branches] ON;

                INSERT INTO [Branches] ([Id], [Code], [Name], [Address], [PhoneNumber])
                VALUES
                    (1, N'CAI-01', N'Cairo Main Branch', N'10 Tahrir Square, Cairo', N'0221234567'),
                    (2, N'ALX-01', N'Alexandria Branch', N'5 Corniche Road, Alexandria', N'0331234567'),
                    (3, N'GIZ-01', N'Giza Branch', N'15 Haram Street, Giza', N'0381234567');

                SET IDENTITY_INSERT [Branches] OFF;
                """);

            // ── Seed Managers ──
            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Managers] ON;

                INSERT INTO [Managers] ([Id], [FullName], [Email], [PhoneNumber], [HireDate], [BranchId])
                VALUES
                    (1, N'Ahmed Hassan', N'ahmed.hassan@bank.com', N'01001111111', '2018-03-15T00:00:00', 1),
                    (2, N'Sara Mohamed', N'sara.mohamed@bank.com', N'01002222222', '2019-07-01T00:00:00', 2),
                    (3, N'Omar Khaled', N'omar.khaled@bank.com', N'01003333333', '2020-01-10T00:00:00', 3);

                SET IDENTITY_INSERT [Managers] OFF;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Transactions");
            migrationBuilder.DropTable(name: "CustomerAccounts");
            migrationBuilder.DropTable(name: "Accounts");
            migrationBuilder.DropTable(name: "Managers");
            migrationBuilder.DropTable(name: "Customers");
            migrationBuilder.DropTable(name: "Branches");
        }
    }
}
