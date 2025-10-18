using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WOVSIM.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_sessions",
                columns: table => new
                {
                    game_session_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    starter_user_id = table.Column<int>(type: "integer", nullable: false),
                    discord_guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    discord_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    game_information_response_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_sessions", x => x.game_session_id);
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

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_session_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    alive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.player_id);
                    table.ForeignKey(
                        name: "fk_players_game_sessions_game_session_id",
                        column: x => x.game_session_id,
                        principalTable: "game_sessions",
                        principalColumn: "game_session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_players_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_players_game_session_id",
                table: "players",
                column: "game_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_user_id",
                table: "players",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "game_sessions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
