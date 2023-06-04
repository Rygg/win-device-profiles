using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1062

namespace Infrastructure.Migrations
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
                    DeviceProfileId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayId = table.Column<uint>(type: "INTEGER", nullable: false),
                    PrimaryDisplay = table.Column<bool>(type: "INTEGER", nullable: true),
                    EnableHdr = table.Column<bool>(type: "INTEGER", nullable: true),
                    RefreshRate = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplaySettings", x => new { x.DeviceProfileId, x.Id });
                    table.ForeignKey(
                        name: "FK_DisplaySettings_DeviceProfiles_DeviceProfileId",
                        column: x => x.DeviceProfileId,
                        principalTable: "DeviceProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
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