using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoChallenger.Migrations
{
    /// <inheritdoc />
    public partial class Challengess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxProgress = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    WasConcluded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChallenges_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChallenges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Challenges",
                columns: new[] { "Id", "Description", "MaxProgress", "Points", "Title", "Type" },
                values: new object[,]
                {
                    { 1, "Description 1", 0, 10, "Daily Challenge 1", "Daily" },
                    { 2, "Description 2", 0, 10, "Daily Challenge 2", "Daily" },
                    { 3, "Description 3", 0, 10, "Daily Challenge 3", "Daily" },
                    { 4, "Description 4", 0, 10, "Daily Challenge 4", "Daily" },
                    { 5, "Description 5", 0, 10, "Daily Challenge 5", "Daily" },
                    { 6, "Description 6", 0, 10, "Daily Challenge 6", "Daily" },
                    { 7, "Description 7", 0, 10, "Daily Challenge 7", "Daily" },
                    { 8, "Description 8", 0, 10, "Daily Challenge 8", "Daily" },
                    { 9, "Description 9", 0, 10, "Daily Challenge 9", "Daily" },
                    { 10, "Description 10", 0, 10, "Daily Challenge 10", "Daily" },
                    { 11, "Description 1", 5, 100, "Weekly Challenge 1", "Weekly" },
                    { 12, "Description 2", 7, 160, "Weekly Challenge 2", "Weekly" },
                    { 13, "Description 3", 3, 60, "Weekly Challenge 3", "Weekly" },
                    { 14, "Description 4", 5, 100, "Weekly Challenge 4", "Weekly" },
                    { 15, "Description 5", 4, 80, "Weekly Challenge 5", "Weekly" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_ChallengeId",
                table: "UserChallenges",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_UserId",
                table: "UserChallenges",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChallenges");

            migrationBuilder.DropTable(
                name: "Challenges");
        }
    }
}
