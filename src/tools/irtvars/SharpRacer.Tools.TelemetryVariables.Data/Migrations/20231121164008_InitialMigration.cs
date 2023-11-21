using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpRacer.Tools.TelemetryVariables.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.Id);
                    table.UniqueConstraint("AK_Car_NormalizedPath", x => x.NormalizedPath);
                });

            migrationBuilder.CreateTable(
                name: "Variable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ValueType = table.Column<int>(type: "INTEGER", nullable: false),
                    ValueUnit = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    ValueCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsTimeSliceArray = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    IsDeprecated = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeprecatingVariableKey = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variable", x => x.Id);
                    table.UniqueConstraint("AK_Variable_NormalizedName", x => x.NormalizedName);
                    table.ForeignKey(
                        name: "FK_Variable_DeprecatingVariable",
                        column: x => x.DeprecatingVariableKey,
                        principalTable: "Variable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CarVariable",
                columns: table => new
                {
                    CarKey = table.Column<int>(type: "INTEGER", nullable: false),
                    VariableKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarVariable", x => new { x.CarKey, x.VariableKey });
                    table.ForeignKey(
                        name: "FK_CarVariable_Car",
                        column: x => x.CarKey,
                        principalTable: "Car",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarVariable_Variable",
                        column: x => x.VariableKey,
                        principalTable: "Variable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "AK_Car_NormalizedPath",
                table: "Car",
                column: "NormalizedPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarVariable_VariableKey",
                table: "CarVariable",
                column: "VariableKey");

            migrationBuilder.CreateIndex(
                name: "AK_Variable_NormalizedName",
                table: "Variable",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variable_DeprecatingVariableKey",
                table: "Variable",
                column: "DeprecatingVariableKey");

            // Views
            migrationBuilder.Sql(@"CREATE VIEW [SessionVariables] AS
SELECT
	v.[Id] AS [VariableKey]
FROM [CarVariable] cv
JOIN [Variable] v ON cv.[VariableKey] = v.[Id]
GROUP BY cv.[VariableKey]
HAVING COUNT(cv.[CarKey]) = (SELECT COUNT(*) FROM [Car])
ORDER BY v.[Id]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarVariable");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Variable");

            migrationBuilder.Sql(@"DROP VIEW [SessionVariables];");
        }
    }
}
