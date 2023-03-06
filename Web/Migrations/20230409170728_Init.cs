using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<int>(type: "int", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<int>(type: "int", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<int>(type: "int", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechicalInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Years",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<int>(type: "int", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
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
                    Size = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    ReissueId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    LabelId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Albums_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalInfo",
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
                    table.PrimaryKey("PK_TechnicalInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Adces_AdcId",
                        column: x => x.AdcId,
                        principalTable: "Adces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Amplifiers_AmplifierId",
                        column: x => x.AmplifierId,
                        principalTable: "Amplifiers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Bitnesses_BitnessId",
                        column: x => x.BitnessId,
                        principalTable: "Bitnesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Cartriges_CartrigeId",
                        column: x => x.CartrigeId,
                        principalTable: "Cartriges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Codecs_CodecId",
                        column: x => x.CodecId,
                        principalTable: "Codecs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Formats_FormatId",
                        column: x => x.FormatId,
                        principalTable: "Formats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Processings_ProcessingId",
                        column: x => x.ProcessingId,
                        principalTable: "Processings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_Samplings_SamplingId",
                        column: x => x.SamplingId,
                        principalTable: "Samplings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalInfo_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id");
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
                name: "IX_Albums_YearId",
                table: "Albums",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_AdcId",
                table: "TechnicalInfo",
                column: "AdcId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_AlbumId",
                table: "TechnicalInfo",
                column: "AlbumId",
                unique: true,
                filter: "[AlbumId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_AmplifierId",
                table: "TechnicalInfo",
                column: "AmplifierId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_BitnessId",
                table: "TechnicalInfo",
                column: "BitnessId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_CartrigeId",
                table: "TechnicalInfo",
                column: "CartrigeId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_CodecId",
                table: "TechnicalInfo",
                column: "CodecId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_DeviceId",
                table: "TechnicalInfo",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_FormatId",
                table: "TechnicalInfo",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_ProcessingId",
                table: "TechnicalInfo",
                column: "ProcessingId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_SamplingId",
                table: "TechnicalInfo",
                column: "SamplingId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalInfo_StateId",
                table: "TechnicalInfo",
                column: "StateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicalInfo");

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
                name: "Years");
        }
    }
}
