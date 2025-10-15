using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WOVSIM.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_players_games_game_id",
                table: "players");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropIndex(
                name: "ix_players_discord_user_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "discord_user_id",
                table: "players");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "players",
                newName: "game_session_game_id");

            migrationBuilder.RenameIndex(
                name: "ix_players_game_id",
                table: "players",
                newName: "ix_players_game_session_game_id");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "game_sessions",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_sessions", x => x.game_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discord_user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_players_user_id",
                table: "players",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_sessions_game_id",
                table: "game_sessions",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_discord_user_id",
                table: "users",
                column: "discord_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_user_id",
                table: "users",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_players_game_sessions_game_session_game_id",
                table: "players",
                column: "game_session_game_id",
                principalTable: "game_sessions",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_players_game_sessions_game_session_game_id",
                table: "players");

            migrationBuilder.DropTable(
                name: "game_sessions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "ix_players_user_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "players");

            migrationBuilder.RenameColumn(
                name: "game_session_game_id",
                table: "players",
                newName: "game_id");

            migrationBuilder.RenameIndex(
                name: "ix_players_game_session_game_id",
                table: "players",
                newName: "ix_players_game_id");

            migrationBuilder.AddColumn<decimal>(
                name: "discord_user_id",
                table: "players",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.game_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_players_discord_user_id",
                table: "players",
                column: "discord_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_games_game_id",
                table: "games",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "fk_players_games_game_id",
                table: "players",
                column: "game_id",
                principalTable: "games",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
