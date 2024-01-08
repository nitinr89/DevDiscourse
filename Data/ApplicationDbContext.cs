using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ContributorModels;
using Devdiscourse.Models.Others;
using Devdiscourse.Models.ResearchModels;
using Devdiscourse.Models.VideoNewsModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<SectorMapping>()
            .HasKey(x => new { x.SectorId, x.NewsId });

        // Configure relationships if not done through navigation properties
        builder.Entity<SectorMapping>()
            .HasOne(ns => ns.DevNews).WithMany(ns => ns.SectorMapping)
            .HasForeignKey(ns => ns.NewsId);

        builder.Entity<SectorMapping>()
            .HasOne(ns => ns.DevSector)
            .WithMany(ns=> ns.SectorMapping)
            .HasForeignKey(ns => ns.SectorId);
    }
    public DbSet<DevNews> DevNews { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<DevResearch> DevResearches { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Response> Response { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Advertisement> Advertisements { get; set; }
    public DbSet<UserInterest> UserInterests { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Website> Websites { get; set; }
    public DbSet<AdvisoryPanel> AdvisoryPanels { get; set; }
    public DbSet<SDGSamurai> SDGSamurais { get; set; }
    public DbSet<AdoptSDGTool> AdoptSDGTools { get; set; }
    public DbSet<DevSector> DevSectors { get; set; }
    public DbSet<DevTheme> DevThemes { get; set; }
    public DbSet<SubscribeNews> SubscribeNews { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Rank> Ranks { get; set; }
    public DbSet<ResearchTeam> ResearchTeams { get; set; }
    public DbSet<Infocus> Infocus { get; set; }
    public DbSet<UserFiles> UserFiles { get; set; }
    public DbSet<CommentLog> CommentLogs { get; set; }
    public DbSet<Career> Careers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserLog> UserLogs { get; set; }
    public DbSet<ResponseReport> ResponseReports { get; set; }
    public DbSet<UserBehaviour> UserBehaviours { get; set; }
    public DbSet<UserComment> UserComments { get; set; }
    public DbSet<Partners> Partners { get; set; }
    public DbSet<Labels> Labels { get; set; }
    public DbSet<LiveBlog> LiveBlogs { get; set; }
    public DbSet<Meme> Memes { get; set; }
    public DbSet<UserWork> UserWorks { get; set; }
    //public DbSet<NewsTag> NewsTags { get; set; }
    public DbSet<AssignNews> AssignNews { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<NewsWireModel> NewsWireModels { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Earnings> Earnings { get; set; }
    public DbSet<PaymentHistory> PaymentHistory { get; set; }
    public DbSet<ContentLog> ContentLogs { get; set; }
    public DbSet<ImageGallery> ImageGalleries { get; set; }
    public DbSet<DiscourseComment> DiscourseComments { get; set; }
    public DbSet<Livediscourse> Livediscourses { get; set; }
    public DbSet<DiscourseTopic> DiscourseTopics { get; set; }
    public DbSet<DiscourseTag> DiscourseTags { get; set; }
    public DbSet<FollowTag> FollowTags { get; set; }
    public DbSet<FollowLivediscourse> FollowLivediscourses { get; set; }
    public DbSet<React> Reacts { get; set; }
    public DbSet<DiscourseIndexes> DiscourseIndexes { get; set; }
    public DbSet<UserNewsLabel> UserNewsLabels { get; set; }
    public DbSet<CampaignPetition> CampaignPetitions { get; set; }
    public DbSet<CommonEvent> CommonEvents { get; set; }
    public DbSet<EventNavLink> EventNavLinks { get; set; }
    public DbSet<UserNewsFile> UserNewsFiles { get; set; }
    public DbSet<LiveDiscourseInfocus> LiveDiscourseInfocus { get; set; }
    public DbSet<Tagstb> Tagstb { get; set; }
    public DbSet<NewsTagstb> NewsTagstb { get; set; }
    public DbSet<VideoNews> VideoNews { get; set; }
    public DbSet<VideoNewsRegion> VideoNewsRegions { get; set; }
    public DbSet<VideoNewsSector> VideoNewsSectors { get; set; }

    //public DbSet<FollowNewsTags> FollowNewsTags { get; set; }
    public DbSet<VideoNewsTag> VideoNewsTags { get; set; }
    public DbSet<LivediscourseVideo> LivediscourseVideos { get; set; }
    public DbSet<MediaInternship> MediaInternships { get; set; }
    public DbSet<RegionNewsRanking> RegionNewsRankings { get; set; }
    public DbSet<SectorMapping> SectorMappings { get; set; }
}

