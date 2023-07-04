using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1062

namespace DeviceProfiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    HotKey_Modifiers = table.Column<int>(type: "INTEGER", nullable: true),
                    HotKey_Key = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisplaySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayId = table.Column<uint>(type: "INTEGER", nullable: false),
                    PrimaryDisplay = table.Column<bool>(type: "INTEGER", nullable: true),
                    EnableHdr = table.Column<bool>(type: "INTEGER", nullable: true),
                    RefreshRate = table.Column<int>(type: "INTEGER", nullable: true),
                    DeviceProfileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplaySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisplaySettings_DeviceProfiles_DeviceProfileId",
                        column: x => x.DeviceProfileId,
                        principalTable: "DeviceProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisplaySettings_DeviceProfileId",
                table: "DisplaySettings",
                column: "DeviceProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplaySettings");

            migrationBuilder.DropTable(
                name: "DeviceProfiles");
        }
    }
}
#pragma warning restore CA1062