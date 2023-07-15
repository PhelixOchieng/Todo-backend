using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserToTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "todo_items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_todo_items_user_id",
                table: "todo_items",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_todo_items_users_user_id",
                table: "todo_items",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_users_user_id",
                table: "todo_items");

            migrationBuilder.DropIndex(
                name: "ix_todo_items_user_id",
                table: "todo_items");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "todo_items");
        }
    }
}
