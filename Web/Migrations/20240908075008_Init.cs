using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Amplifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amplifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bitnesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitnesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cartriges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartriges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Codecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reissues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reissues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samplings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samplings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Years",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Years", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<double>(type: "float", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    ReissueId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    LabelId = table.Column<int>(type: "int", nullable: true),
                    StorageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Albums_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Albums_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Albums_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Albums_Reissues_ReissueId",
                        column: x => x.ReissueId,
                        principalTable: "Reissues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Albums_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Albums_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlbumId = table.Column<int>(type: "int", nullable: true),
                    AmplifierId = table.Column<int>(type: "int", nullable: true),
                    BitnessId = table.Column<int>(type: "int", nullable: true),
                    CartrigeId = table.Column<int>(type: "int", nullable: true),
                    CodecId = table.Column<int>(type: "int", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    FormatId = table.Column<int>(type: "int", nullable: true),
                    ProcessingId = table.Column<int>(type: "int", nullable: true),
                    AdcId = table.Column<int>(type: "int", nullable: true),
                    SamplingId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Adces_AdcId",
                        column: x => x.AdcId,
                        principalTable: "Adces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Amplifiers_AmplifierId",
                        column: x => x.AmplifierId,
                        principalTable: "Amplifiers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Bitnesses_BitnessId",
                        column: x => x.BitnessId,
                        principalTable: "Bitnesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Cartriges_CartrigeId",
                        column: x => x.CartrigeId,
                        principalTable: "Cartriges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Codecs_CodecId",
                        column: x => x.CodecId,
                        principalTable: "Codecs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Formats_FormatId",
                        column: x => x.FormatId,
                        principalTable: "Formats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Processings_ProcessingId",
                        column: x => x.ProcessingId,
                        principalTable: "Processings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_Samplings_SamplingId",
                        column: x => x.SamplingId,
                        principalTable: "Samplings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfos_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Bitnesses",
                columns: new[] { "Id", "Data" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 24 },
                    { 3, 32 },
                    { 4, 64 }
                });

            migrationBuilder.InsertData(
                table: "Codecs",
                columns: new[] { "Id", "Data" },
                values: new object[,]
                {
                    { 1, "FLAC" },
                    { 2, "DSD64" },
                    { 3, "DSD128" },
                    { 4, "DSD256" },
                    { 5, "DSD512" },
                    { 6, "WV" }
                });

            migrationBuilder.InsertData(
                table: "Formats",
                columns: new[] { "Id", "Data" },
                values: new object[,]
                {
                    { 1, "LP 12'' 33RPM" },
                    { 2, "EP 10'' 45RPM" },
                    { 3, "EP 12'' 45RPM" },
                    { 4, "SINGLE 7'' 45RPM" },
                    { 5, "SINGLE 12'' 45RPM" },
                    { 6, "SHELLAC 10'' 78RPM" }
                });

            migrationBuilder.InsertData(
                table: "Samplings",
                columns: new[] { "Id", "Data" },
                values: new object[,]
                {
                    { 1, 96.0 },
                    { 2, 192.0 },
                    { 3, 384.0 },
                    { 4, 2.7999999999999998 },
                    { 5, 5.5999999999999996 },
                    { 6, 11.199999999999999 },
                    { 7, 22.5 }
                });

            migrationBuilder.InsertData(
                table: "States",
                columns: new[] { "Id", "Data" },
                values: new object[,]
                {
                    { 1, "Mint" },
                    { 2, "Near Mint" },
                    { 3, "Very Good+" },
                    { 4, "Very Good" },
                    { 5, "Good" },
                    { 6, "Unknown" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CountryId",
                table: "Albums",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_GenreId",
                table: "Albums",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_LabelId",
                table: "Albums",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ReissueId",
                table: "Albums",
                column: "ReissueId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_StorageId",
                table: "Albums",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_YearId",
                table: "Albums",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_AdcId",
                table: "TechnicalInfos",
                column: "AdcId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_AlbumId",
                table: "TechnicalInfos",
                column: "AlbumId",
                unique: true,
                filter: "[AlbumId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_AmplifierId",
                table: "TechnicalInfos",
                column: "AmplifierId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_BitnessId",
                table: "TechnicalInfos",
                column: "BitnessId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_CartrigeId",
                table: "TechnicalInfos",
                column: "CartrigeId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_CodecId",
                table: "TechnicalInfos",
                column: "CodecId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_DeviceId",
                table: "TechnicalInfos",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_FormatId",
                table: "TechnicalInfos",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_ProcessingId",
                table: "TechnicalInfos",
                column: "ProcessingId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_SamplingId",
                table: "TechnicalInfos",
                column: "SamplingId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfos_StateId",
                table: "TechnicalInfos",
                column: "StateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicalInfos");

            migrationBuilder.DropTable(
                name: "Adces");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Amplifiers");

            migrationBuilder.DropTable(
                name: "Bitnesses");

            migrationBuilder.DropTable(
                name: "Cartriges");

            migrationBuilder.DropTable(
                name: "Codecs");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Formats");

            migrationBuilder.DropTable(
                name: "Processings");

            migrationBuilder.DropTable(
                name: "Samplings");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Reissues");

            migrationBuilder.DropTable(
                name: "Storages");

            migrationBuilder.DropTable(
                name: "Years");
        }
    }
}
