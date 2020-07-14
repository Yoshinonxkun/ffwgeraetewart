using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Member.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Psa",
                columns: table => new
                {
                    PsaId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EinsatzJacke = table.Column<int>(nullable: false),
                    EinsatzHose = table.Column<int>(nullable: false),
                    ArbeitsJacke = table.Column<int>(nullable: false),
                    ArbeitsHose = table.Column<int>(nullable: false),
                    Helm = table.Column<int>(nullable: false),
                    HelmDate = table.Column<DateTime>(nullable: false),
                    Handschuhe = table.Column<int>(nullable: false),
                    Schuhe = table.Column<int>(nullable: false),
                    Kopfschutzhaube = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Psa", x => x.PsaId);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Surname = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MemberFK = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Members_Psa_MemberFK",
                        column: x => x.MemberFK,
                        principalTable: "Psa",
                        principalColumn: "PsaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberFK",
                table: "Members",
                column: "MemberFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Psa");
        }
    }
}
