using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmergencyManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "abrigos",
                columns: table => new
                {
                    id_abrigo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    endereco = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    capacidade_total = table.Column<int>(type: "INTEGER", nullable: false),
                    vagas_disponiveis = table.Column<int>(type: "INTEGER", nullable: false),
                    telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abrigos", x => x.id_abrigo);
                });

            migrationBuilder.CreateTable(
                name: "tipos_ocorrencia",
                columns: table => new
                {
                    id_tipo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_ocorrencia", x => x.id_tipo);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    tipo_usuario = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    telefone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    endereco = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "checkins_abrigos",
                columns: table => new
                {
                    id_checkin = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_usuario = table.Column<int>(type: "INTEGER", nullable: false),
                    id_abrigo = table.Column<int>(type: "INTEGER", nullable: false),
                    data_entrada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    data_saida = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkins_abrigos", x => x.id_checkin);
                    table.ForeignKey(
                        name: "FK_checkins_abrigos_abrigos_id_abrigo",
                        column: x => x.id_abrigo,
                        principalTable: "abrigos",
                        principalColumn: "id_abrigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checkins_abrigos_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ocorrencias",
                columns: table => new
                {
                    id_ocorrencia = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_usuario = table.Column<int>(type: "INTEGER", nullable: false),
                    id_tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    latitude = table.Column<decimal>(type: "TEXT", nullable: true),
                    longitude = table.Column<decimal>(type: "TEXT", nullable: true),
                    data_ocorrencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    nivel_risco = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ocorrencias", x => x.id_ocorrencia);
                    table.ForeignKey(
                        name: "FK_ocorrencias_tipos_ocorrencia_id_tipo",
                        column: x => x.id_tipo,
                        principalTable: "tipos_ocorrencia",
                        principalColumn: "id_tipo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ocorrencias_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alertas",
                columns: table => new
                {
                    id_alerta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    mensagem = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    nivel_urgencia = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    data_emissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    fonte = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    id_ocorrencia = table.Column<int>(type: "INTEGER", nullable: false),
                    latitude = table.Column<decimal>(type: "TEXT", nullable: true),
                    longitude = table.Column<decimal>(type: "TEXT", nullable: true),
                    raio_afetado = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alertas", x => x.id_alerta);
                    table.ForeignKey(
                        name: "FK_alertas_ocorrencias_id_ocorrencia",
                        column: x => x.id_ocorrencia,
                        principalTable: "ocorrencias",
                        principalColumn: "id_ocorrencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alertas_id_ocorrencia",
                table: "alertas",
                column: "id_ocorrencia");

            migrationBuilder.CreateIndex(
                name: "IX_checkins_abrigos_id_abrigo",
                table: "checkins_abrigos",
                column: "id_abrigo");

            migrationBuilder.CreateIndex(
                name: "IX_checkins_abrigos_id_usuario",
                table: "checkins_abrigos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_ocorrencias_id_tipo",
                table: "ocorrencias",
                column: "id_tipo");

            migrationBuilder.CreateIndex(
                name: "IX_ocorrencias_id_usuario",
                table: "ocorrencias",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alertas");

            migrationBuilder.DropTable(
                name: "checkins_abrigos");

            migrationBuilder.DropTable(
                name: "ocorrencias");

            migrationBuilder.DropTable(
                name: "abrigos");

            migrationBuilder.DropTable(
                name: "tipos_ocorrencia");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
