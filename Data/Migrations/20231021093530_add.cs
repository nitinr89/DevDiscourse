using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devdiscourse.Data.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DigitCode",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationType",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePic",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPRManager",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LogTitle = table.Column<string>(type: "TEXT", nullable: false),
                    LogDescription = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: false),
                    Activity = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityUrl = table.Column<string>(type: "TEXT", nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_AspNetUsers_ActivityUserId",
                        column: x => x.ActivityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdoptSDGTools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Institution = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Contact = table.Column<string>(type: "TEXT", nullable: false),
                    NeededData = table.Column<string>(type: "TEXT", nullable: false),
                    GeographicalData = table.Column<string>(type: "TEXT", nullable: false),
                    ThemeticArea = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptSDGTools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Advertisor = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryPanels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Member = table.Column<string>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", nullable: false),
                    AboutMe = table.Column<string>(type: "TEXT", nullable: false),
                    ProfilePic = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryPanels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    AdminCheck = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignNews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignNews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CampaignPetitions",
                columns: table => new
                {
                    CampaignPetitionId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    PledgeType = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPetitions", x => x.CampaignPetitionId);
                });

            migrationBuilder.CreateTable(
                name: "Careers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", nullable: false),
                    Qualification = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentEmployment = table.Column<string>(type: "TEXT", nullable: false),
                    AreaOfExpertise = table.Column<string>(type: "TEXT", nullable: false),
                    UploadCV = table.Column<string>(type: "TEXT", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    JobId = table.Column<long>(type: "INTEGER", nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Careers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PageTitle = table.Column<string>(type: "TEXT", nullable: false),
                    PageImage = table.Column<string>(type: "TEXT", nullable: false),
                    BannerImage = table.Column<string>(type: "TEXT", nullable: false),
                    PageDescription = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LogTitle = table.Column<string>(type: "TEXT", nullable: false),
                    LogDescription = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityUrl = table.Column<string>(type: "TEXT", nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EventVenue = table.Column<string>(type: "TEXT", nullable: false),
                    EventTagline = table.Column<string>(type: "TEXT", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundBannerUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    RegistrationUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: false),
                    PageKeywords = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StreetAddress = table.Column<string>(type: "TEXT", nullable: false),
                    AddressLocality = table.Column<string>(type: "TEXT", nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    AddressRegion = table.Column<string>(type: "TEXT", nullable: false),
                    AddressCountry = table.Column<string>(type: "TEXT", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    AvailabilityUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    CardUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Featured = table.Column<bool>(type: "INTEGER", nullable: false),
                    EventTheme = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceInfo = table.Column<string>(type: "TEXT", nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", nullable: false),
                    UserRegion = table.Column<string>(type: "TEXT", nullable: false),
                    NewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevNews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    AlternateHeadline = table.Column<string>(type: "TEXT", nullable: false),
                    SubTitle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    SubType = table.Column<string>(type: "TEXT", nullable: false),
                    NewsLabels = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    Themes = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCopyright = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCaption = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    AdminCheck = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSponsored = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditorPick = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInfocus = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGlobal = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsStandout = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsIndexed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalSource = table.Column<string>(type: "TEXT", nullable: false),
                    SourceUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    WorkStage = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReferenceId = table.Column<long>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevNews_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DevResearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    AdminCheck = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevResearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevResearches_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DevSectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevSectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevThemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscourseComments",
                columns: table => new
                {
                    CommentId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentText = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<long>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    RootParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DislikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    EndorseCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RejectCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentBy = table.Column<string>(type: "TEXT", nullable: false),
                    ReplyText = table.Column<string>(type: "TEXT", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    ChildCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscourseComments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_DiscourseComments_AspNetUsers_CommentBy",
                        column: x => x.CommentBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscourseTopics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    DislikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscourseTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscourseTopics_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SubTitle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    Themes = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AdminCheck = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInfocus = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGlobal = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageGallery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCopyright = table.Column<string>(type: "TEXT", nullable: false),
                    Caption = table.Column<string>(type: "TEXT", nullable: false),
                    UseCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageGallery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infocus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Edition = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infocus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infocus_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationName = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    AboutPosition = table.Column<string>(type: "TEXT", nullable: false),
                    EmailId = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    PostedByUser = table.Column<string>(type: "TEXT", nullable: false),
                    OpeningDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false),
                    MinCTC = table.Column<long>(type: "INTEGER", nullable: false),
                    MaxCTC = table.Column<long>(type: "INTEGER", nullable: false),
                    JobCTCType = table.Column<string>(type: "TEXT", nullable: false),
                    CTCCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    Vacancy = table.Column<int>(type: "INTEGER", nullable: false),
                    MinExperience = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxExperience = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiveBlogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCopyright = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveBlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Livediscourse",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SubTitle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCopyright = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCaption = table.Column<string>(type: "TEXT", nullable: false),
                    ParentStoryLink = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    AdminCheck = table.Column<bool>(type: "INTEGER", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DislikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FollowCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: false),
                    Close_Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    LivediscourseIndex = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublishedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livediscourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Livediscourse_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaInternships",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sectors = table.Column<string>(type: "TEXT", nullable: false),
                    Editions = table.Column<string>(type: "TEXT", nullable: false),
                    CVUrl = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaInternships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaInternships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsWeekMeme = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsContents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SubTitle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    NewsLabels = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCopyright = table.Column<string>(type: "TEXT", nullable: false),
                    VideoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    ContentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ReasonofReject = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsContents_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsWire",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SubTitle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    NewsLabels = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorImage = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    ImageCopyright = table.Column<string>(type: "TEXT", nullable: false),
                    VideoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReasonofReject = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsWire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsWire_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SubType = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentMode = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceNo = table.Column<string>(type: "TEXT", nullable: false),
                    WithdrawAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistory_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Pincode = table.Column<string>(type: "TEXT", nullable: false),
                    Contact = table.Column<string>(type: "TEXT", nullable: false),
                    EmailId = table.Column<string>(type: "TEXT", nullable: false),
                    AccountHolderName = table.Column<string>(type: "TEXT", nullable: false),
                    BankName = table.Column<string>(type: "TEXT", nullable: false),
                    BankAddress = table.Column<string>(type: "TEXT", nullable: false),
                    BankCity = table.Column<string>(type: "TEXT", nullable: false),
                    BankState = table.Column<string>(type: "TEXT", nullable: false),
                    BankCountry = table.Column<string>(type: "TEXT", nullable: false),
                    BankPincode = table.Column<string>(type: "TEXT", nullable: false),
                    AccountNo = table.Column<string>(type: "TEXT", nullable: false),
                    AccountType = table.Column<string>(type: "TEXT", nullable: false),
                    IFSCCode = table.Column<string>(type: "TEXT", nullable: false),
                    PanCardNo = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reacts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReactBy = table.Column<string>(type: "TEXT", nullable: false),
                    ReactOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReactType = table.Column<int>(type: "INTEGER", nullable: false),
                    ReactItemType = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reacts_AspNetUsers_ReactBy",
                        column: x => x.ReactBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResearchTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamMember = table.Column<string>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", nullable: false),
                    AboutMe = table.Column<string>(type: "TEXT", nullable: false),
                    ProfilePic = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Response",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    IsLike = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Response", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Response_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReportText = table.Column<string>(type: "TEXT", nullable: false),
                    ResponseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResponseTitle = table.Column<string>(type: "TEXT", nullable: false),
                    ReportBy = table.Column<string>(type: "TEXT", nullable: false),
                    ReportedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseReports_AspNetUsers_ReportBy",
                        column: x => x.ReportBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscribeNews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribeNews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tagstb",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagTitle = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tagstb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamMember = table.Column<string>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    AboutMe = table.Column<string>(type: "TEXT", nullable: false),
                    ProfilePic = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBehaviours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BehaviourType = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResponseTitle = table.Column<string>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBehaviours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBehaviours_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserComments",
                columns: table => new
                {
                    CommentId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentText = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemTitle = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    CommentBy = table.Column<string>(type: "TEXT", nullable: false),
                    CommentOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserComments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_UserComments_AspNetUsers_CommentBy",
                        column: x => x.CommentBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    FileFor = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Sector = table.Column<string>(type: "TEXT", nullable: false),
                    InterestType = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInterests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LogTitle = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityUrl = table.Column<string>(type: "TEXT", nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNewsLabels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    NewsLabel = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNewsLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNewsLabels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkStage = table.Column<string>(type: "TEXT", nullable: false),
                    ColorCode = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoNews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    AlternateHeadline = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    AdminCheck = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditorPick = table.Column<bool>(type: "INTEGER", nullable: false),
                    VideoName = table.Column<string>(type: "TEXT", nullable: false),
                    BlobName = table.Column<string>(type: "TEXT", nullable: false),
                    VideoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    Size = table.Column<string>(type: "TEXT", nullable: false),
                    Caption = table.Column<string>(type: "TEXT", nullable: false),
                    Copyright = table.Column<string>(type: "TEXT", nullable: false),
                    VideoThumbUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorImage = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoNews_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventNavLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    Href = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    EventId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNavLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNavLinks_CommonEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "CommonEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNewsFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    FileType = table.Column<string>(type: "TEXT", nullable: false),
                    FileCaption = table.Column<string>(type: "TEXT", nullable: false),
                    FileThumbUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    NewsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNewsFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNewsFiles_DevNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "DevNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscourseTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SectorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscourseTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscourseTags_DevSectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "DevSectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscourseIndex",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    LivediscourseId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscourseIndex", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscourseIndex_Livediscourse_LivediscourseId",
                        column: x => x.LivediscourseId,
                        principalTable: "Livediscourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowLivediscourses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FollowBy = table.Column<string>(type: "TEXT", nullable: false),
                    FollowOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsModerator = table.Column<bool>(type: "INTEGER", nullable: false),
                    LivediscourseId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowLivediscourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowLivediscourses_AspNetUsers_FollowBy",
                        column: x => x.FollowBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowLivediscourses_Livediscourse_LivediscourseId",
                        column: x => x.LivediscourseId,
                        principalTable: "Livediscourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiveDiscourseInfocuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Edition = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    LivediscourseId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveDiscourseInfocuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveDiscourseInfocuses_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveDiscourseInfocuses_Livediscourse_LivediscourseId",
                        column: x => x.LivediscourseId,
                        principalTable: "Livediscourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LivediscourseVideos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileMimeType = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    FileType = table.Column<string>(type: "TEXT", nullable: false),
                    FileCaption = table.Column<string>(type: "TEXT", nullable: false),
                    FileThumbUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LivediscourseId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivediscourseVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LivediscourseVideos_Livediscourse_LivediscourseId",
                        column: x => x.LivediscourseId,
                        principalTable: "Livediscourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Earnings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Earnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Earnings_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Earnings_NewsContents_NewsId",
                        column: x => x.NewsId,
                        principalTable: "NewsContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SDGSamurais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Profession = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    SDGCode = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: false),
                    SDGPosition = table.Column<int>(type: "INTEGER", nullable: false),
                    SDGPoints = table.Column<double>(type: "REAL", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SDGSamurais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SDGSamurais_AspNetUsers_Creator",
                        column: x => x.Creator,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SDGSamurais_Ranks_SDGPosition",
                        column: x => x.SDGPosition,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegionNewsRankings",
                columns: table => new
                {
                    RegionNewsId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NewsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Ranking = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionNewsRankings", x => x.RegionNewsId);
                    table.ForeignKey(
                        name: "FK_RegionNewsRankings_DevNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "DevNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegionNewsRankings_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Websites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    SiteUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    PressRelease = table.Column<string>(type: "TEXT", nullable: false),
                    PressReleaseUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Websites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Websites_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsTagstb",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagId = table.Column<long>(type: "INTEGER", nullable: false),
                    DevNewsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTagstb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsTagstb_DevNews_DevNewsId",
                        column: x => x.DevNewsId,
                        principalTable: "DevNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsTagstb_Tagstb_TagId",
                        column: x => x.TagId,
                        principalTable: "Tagstb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoNewsRegions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoNewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    EditionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoNewsRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoNewsRegions_Regions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoNewsRegions_VideoNews_VideoNewsId",
                        column: x => x.VideoNewsId,
                        principalTable: "VideoNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoNewsSectors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoNewsId = table.Column<long>(type: "INTEGER", nullable: false),
                    SectorId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoNewsSectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoNewsSectors_DevSectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "DevSectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoNewsSectors_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoNewsSectors_VideoNews_VideoNewsId",
                        column: x => x.VideoNewsId,
                        principalTable: "VideoNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoNewsTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagId = table.Column<long>(type: "INTEGER", nullable: false),
                    VideoNewsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoNewsTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoNewsTags_Tagstb_TagId",
                        column: x => x.TagId,
                        principalTable: "Tagstb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoNewsTags_VideoNews_VideoNewsId",
                        column: x => x.VideoNewsId,
                        principalTable: "VideoNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FollowBy = table.Column<string>(type: "TEXT", nullable: false),
                    FollowOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TagId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowTags_AspNetUsers_FollowBy",
                        column: x => x.FollowBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowTags_DiscourseTags_TagId",
                        column: x => x.TagId,
                        principalTable: "DiscourseTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_ActivityUserId",
                table: "ActivityLogs",
                column: "ActivityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Creator",
                table: "Articles",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_RegionId",
                table: "Countries",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_DevNews_Creator",
                table: "DevNews",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_DevResearches_Creator",
                table: "DevResearches",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_DiscourseComments_CommentBy",
                table: "DiscourseComments",
                column: "CommentBy");

            migrationBuilder.CreateIndex(
                name: "IX_DiscourseIndex_LivediscourseId",
                table: "DiscourseIndex",
                column: "LivediscourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscourseTags_SectorId",
                table: "DiscourseTags",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscourseTopics_Creator",
                table: "DiscourseTopics",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_Creator",
                table: "Earnings",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_NewsId",
                table: "Earnings",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNavLinks_EventId",
                table: "EventNavLinks",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Creator",
                table: "Events",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_FollowLivediscourses_FollowBy",
                table: "FollowLivediscourses",
                column: "FollowBy");

            migrationBuilder.CreateIndex(
                name: "IX_FollowLivediscourses_LivediscourseId",
                table: "FollowLivediscourses",
                column: "LivediscourseId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowTags_FollowBy",
                table: "FollowTags",
                column: "FollowBy");

            migrationBuilder.CreateIndex(
                name: "IX_FollowTags_TagId",
                table: "FollowTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Infocus_Creator",
                table: "Infocus",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Livediscourse_Creator",
                table: "Livediscourse",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_LiveDiscourseInfocuses_Creator",
                table: "LiveDiscourseInfocuses",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_LiveDiscourseInfocuses_LivediscourseId",
                table: "LiveDiscourseInfocuses",
                column: "LivediscourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LivediscourseVideos_LivediscourseId",
                table: "LivediscourseVideos",
                column: "LivediscourseId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaInternships_UserId",
                table: "MediaInternships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsContents_Creator",
                table: "NewsContents",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTagstb_DevNewsId",
                table: "NewsTagstb",
                column: "DevNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTagstb_TagId",
                table: "NewsTagstb",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsWire_Creator",
                table: "NewsWire",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistory_Creator",
                table: "PaymentHistory",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Creator",
                table: "Payments",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Reacts_ReactBy",
                table: "Reacts",
                column: "ReactBy");

            migrationBuilder.CreateIndex(
                name: "IX_RegionNewsRankings_NewsId",
                table: "RegionNewsRankings",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionNewsRankings_RegionId",
                table: "RegionNewsRankings",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Response_Creator",
                table: "Response",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseReports_ReportBy",
                table: "ResponseReports",
                column: "ReportBy");

            migrationBuilder.CreateIndex(
                name: "IX_SDGSamurais_Creator",
                table: "SDGSamurais",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_SDGSamurais_SDGPosition",
                table: "SDGSamurais",
                column: "SDGPosition");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviours_Creator",
                table: "UserBehaviours",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_UserComments_CommentBy",
                table: "UserComments",
                column: "CommentBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_UserId",
                table: "UserInterests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNewsFiles_NewsId",
                table: "UserNewsFiles",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNewsLabels_UserId",
                table: "UserNewsLabels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNews_Creator",
                table: "VideoNews",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsRegions_EditionId",
                table: "VideoNewsRegions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsRegions_VideoNewsId",
                table: "VideoNewsRegions",
                column: "VideoNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsSectors_RegionId",
                table: "VideoNewsSectors",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsSectors_SectorId",
                table: "VideoNewsSectors",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsSectors_VideoNewsId",
                table: "VideoNewsSectors",
                column: "VideoNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsTags_TagId",
                table: "VideoNewsTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoNewsTags_VideoNewsId",
                table: "VideoNewsTags",
                column: "VideoNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Websites_RegionId",
                table: "Websites",
                column: "RegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AdoptSDGTools");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "AdvisoryPanels");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "AssignNews");

            migrationBuilder.DropTable(
                name: "CampaignPetitions");

            migrationBuilder.DropTable(
                name: "Careers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "CommentLogs");

            migrationBuilder.DropTable(
                name: "ContentLogs");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "DevResearches");

            migrationBuilder.DropTable(
                name: "DevThemes");

            migrationBuilder.DropTable(
                name: "DiscourseComments");

            migrationBuilder.DropTable(
                name: "DiscourseIndex");

            migrationBuilder.DropTable(
                name: "DiscourseTopics");

            migrationBuilder.DropTable(
                name: "Earnings");

            migrationBuilder.DropTable(
                name: "EventNavLinks");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "FollowLivediscourses");

            migrationBuilder.DropTable(
                name: "FollowTags");

            migrationBuilder.DropTable(
                name: "ImageGallery");

            migrationBuilder.DropTable(
                name: "Infocus");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "LiveBlogs");

            migrationBuilder.DropTable(
                name: "LiveDiscourseInfocuses");

            migrationBuilder.DropTable(
                name: "LivediscourseVideos");

            migrationBuilder.DropTable(
                name: "MediaInternships");

            migrationBuilder.DropTable(
                name: "Memes");

            migrationBuilder.DropTable(
                name: "NewsTagstb");

            migrationBuilder.DropTable(
                name: "NewsWire");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "PaymentHistory");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reacts");

            migrationBuilder.DropTable(
                name: "RegionNewsRankings");

            migrationBuilder.DropTable(
                name: "ResearchTeams");

            migrationBuilder.DropTable(
                name: "Response");

            migrationBuilder.DropTable(
                name: "ResponseReports");

            migrationBuilder.DropTable(
                name: "SDGSamurais");

            migrationBuilder.DropTable(
                name: "SubscribeNews");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "UserBehaviours");

            migrationBuilder.DropTable(
                name: "UserComments");

            migrationBuilder.DropTable(
                name: "UserFiles");

            migrationBuilder.DropTable(
                name: "UserInterests");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "UserNewsFiles");

            migrationBuilder.DropTable(
                name: "UserNewsLabels");

            migrationBuilder.DropTable(
                name: "UserWorks");

            migrationBuilder.DropTable(
                name: "VideoNewsRegions");

            migrationBuilder.DropTable(
                name: "VideoNewsSectors");

            migrationBuilder.DropTable(
                name: "VideoNewsTags");

            migrationBuilder.DropTable(
                name: "Websites");

            migrationBuilder.DropTable(
                name: "NewsContents");

            migrationBuilder.DropTable(
                name: "CommonEvents");

            migrationBuilder.DropTable(
                name: "DiscourseTags");

            migrationBuilder.DropTable(
                name: "Livediscourse");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "DevNews");

            migrationBuilder.DropTable(
                name: "Tagstb");

            migrationBuilder.DropTable(
                name: "VideoNews");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "DevSectors");

            migrationBuilder.DropColumn(
                name: "AboutMe",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DigitCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePic",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isPRManager",
                table: "AspNetUsers");
        }
    }
}
