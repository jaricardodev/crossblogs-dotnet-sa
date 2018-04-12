using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace crossblog.Migrations
{
    public partial class AddingIndexForSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Articles",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Title_Content",
                table: "Articles",
                columns: new[] { "Title", "Content" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_Title_Content",
                table: "Articles");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Articles",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
