using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WOVSIM.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class userupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_players_game_sessions_game_session_game_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "ix_players_user_id",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_sessions",
                table: "game_sessions");

            migrationBuilder.DropIndex(
                name: "ix_game_sessions_game_id",
                table: "game_sessions");

            migrationBuilder.RenameColumn(
                name: "game_session_game_id",
                table: "players",
                newName: "game_session_id");

            migrationBuilder.RenameIndex(
                name: "ix_players_game_session_game_id",
                table: "players",
                newName: "ix_players_game_session_id");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "game_sessions",
                newName: "starter_user_id");

            migrationBuilder.AlterColumn<int>(
                name: "starter_user_id",
                table: "game_sessions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "game_session_id",
                table: "game_sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "discord_guild_id",
                table: "game_sessions",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discord_id",
                table: "game_sessions",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "game_information_response_id",
                table: "game_sessions",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_sessions",
                table: "game_sessions",
                column: "game_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_user_id",
                table: "players",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_sessions_game_session_id",
                table: "game_sessions",
                column: "game_session_id");

            migrationBuilder.AddForeignKey(
                name: "fk_players_game_sessions_game_session_id",
                table: "players",
                column: "game_session_id",
                principalTable: "game_sessions",
                principalColumn: "game_session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_players_users_user_id",
                table: "players",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_players_game_sessions_game_session_id",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "fk_players_users_user_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "ix_players_user_id",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_sessions",
                table: "game_sessions");

            migrationBuilder.DropIndex(
                name: "ix_game_sessions_game_session_id",
                table: "game_sessions");

            migrationBuilder.DropColumn(
                name: "game_session_id",
                table: "game_sessions");

            migrationBuilder.DropColumn(
                name: "discord_guild_id",
                table: "game_sessions");

            migrationBuilder.DropColumn(
                name: "discord_id",
                table: "game_sessions");

            migrationBuilder.DropColumn(
                name: "game_information_response_id",
                table: "game_sessions");

            migrationBuilder.RenameColumn(
                name: "game_session_id",
                table: "players",
                newName: "game_session_game_id");

            migrationBuilder.RenameIndex(
                name: "ix_players_game_session_id",
                table: "players",
                newName: "ix_players_game_session_game_id");

            migrationBuilder.RenameColumn(
                name: "starter_user_id",
                table: "game_sessions",
                newName: "game_id");

            migrationBuilder.AlterColumn<int>(
                name: "game_id",
                table: "game_sessions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_sessions",
                table: "game_sessions",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_user_id",
                table: "players",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_sessions_game_id",
                table: "game_sessions",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "fk_players_game_sessions_game_session_game_id",
                table: "players",
                column: "game_session_game_id",
                principalTable: "game_sessions",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
