using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevPodcast.Data.EntityFramework.Migrations
{
    public partial class Categories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "BasePodcast",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Description = table.Column<string>(nullable: false),
            //        ItunesId = table.Column<string>(maxLength: 50, nullable: true),
            //        ItunesSubscriberUrl = table.Column<string>(nullable: true),
            //        PodcastSite = table.Column<string>(nullable: true),
            //        Title = table.Column<string>(maxLength: 200, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BasePodcast", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                "Category",
                table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(unicode: false, maxLength: 50)
                },
                constraints: table => { table.PrimaryKey("PK_Category", x => x.Id); });

            //migrationBuilder.CreateTable(
            //    name: "Podcast",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Artists = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
            //        Country = table.Column<string>(maxLength: 50, nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Description = table.Column<string>(nullable: false),
            //        EpisodeCount = table.Column<int>(nullable: false, defaultValueSql: "((0))"),
            //        FeedUrl = table.Column<string>(nullable: true),
            //        ImageUrl = table.Column<string>(nullable: true),
            //        ItunesId = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "(N'')"),
            //        LatestReleaseDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        ShowUrl = table.Column<string>(nullable: true),
            //        Title = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Podcast", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Tag",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Description = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Tag", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Episode",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        AudioDuration = table.Column<string>(maxLength: 10, nullable: true),
            //        AudioType = table.Column<string>(maxLength: 10, nullable: true),
            //        AudioUrl = table.Column<string>(nullable: true),
            //        Author = table.Column<string>(maxLength: 250, nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Description = table.Column<string>(nullable: true),
            //        ImageUrl = table.Column<string>(nullable: true),
            //        PodcastId = table.Column<int>(nullable: false),
            //        PublishedDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        SourceUrl = table.Column<string>(nullable: true),
            //        Title = table.Column<string>(maxLength: 250, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Episode", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Episode_Podcast",
            //            column: x => x.PodcastId,
            //            principalTable: "Podcast",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                "PodcastCategory",
                table => new
                {
                    CategoryId = table.Column<int>(),
                    PodcastId = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PodcastCategory", x => new {x.CategoryId, x.PodcastId});
                    table.ForeignKey(
                        "FK_PodcastCategory_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_PodcastCategory_Podcast_PodcastId",
                        x => x.PodcastId,
                        "Podcast",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "PodcastTag",
            //    columns: table => new
            //    {
            //        TagId = table.Column<int>(nullable: false),
            //        PodcastId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PodcastTag", x => new { x.TagId, x.PodcastId });
            //        table.ForeignKey(
            //            name: "FK_PodcastTag_Podcast_PodcastId",
            //            column: x => x.PodcastId,
            //            principalTable: "Podcast",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PodcastTag_Tag_TagId",
            //            column: x => x.TagId,
            //            principalTable: "Tag",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                "EpisodeCategory",
                table => new
                {
                    CategoryId = table.Column<int>(),
                    EpisodeId = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeCategory", x => new {x.CategoryId, x.EpisodeId});
                    table.ForeignKey(
                        "FK_EpisodeCategory_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_EpisodeCategory_Episode_EpisodeId",
                        x => x.EpisodeId,
                        "Episode",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "EpisodeTag",
            //    columns: table => new
            //    {
            //        TagId = table.Column<int>(nullable: false),
            //        EpisodeId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EpisodeTag", x => new { x.TagId, x.EpisodeId });
            //        table.ForeignKey(
            //            name: "FK_EpisodeTag_Episode_EpisodeId",
            //            column: x => x.EpisodeId,
            //            principalTable: "Episode",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_EpisodeTag_Tag_TagId",
            //            column: x => x.TagId,
            //            principalTable: "Tag",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateIndex(
                "IX_Episode_PodcastId",
                "Episode",
                "PodcastId");

            migrationBuilder.CreateIndex(
                "IX_EpisodeCategory_EpisodeId",
                "EpisodeCategory",
                "EpisodeId");

            migrationBuilder.CreateIndex(
                "IX_EpisodeTag_EpisodeId",
                "EpisodeTag",
                "EpisodeId");

            migrationBuilder.CreateIndex(
                "IX_PodcastCategory_PodcastId",
                "PodcastCategory",
                "PodcastId");

            migrationBuilder.CreateIndex(
                "IX_PodcastTag_PodcastId",
                "PodcastTag",
                "PodcastId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "BasePodcast");

            migrationBuilder.DropTable(
                "EpisodeCategory");

            migrationBuilder.DropTable(
                "EpisodeTag");

            migrationBuilder.DropTable(
                "PodcastCategory");

            migrationBuilder.DropTable(
                "PodcastTag");

            migrationBuilder.DropTable(
                "Episode");

            migrationBuilder.DropTable(
                "Category");

            migrationBuilder.DropTable(
                "Tag");

            migrationBuilder.DropTable(
                "Podcast");
        }
    }
}