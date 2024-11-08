﻿using Devdiscourse.Controllers.Research;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ContributorModels;
using Devdiscourse.Models.Others;
using Devdiscourse.Models.ResearchModels;
using Devdiscourse.Models.VideoNewsModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Devdiscourse.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    }
    public DbSet<TopNewsItem> TopNewsItems { get; set; }
    public DbSet<Controlls> Controlls { get; set; }
    public DbSet<DevNews> DevNews { get; set; }
    public DbSet<DevNewsMetaData> DevNewsMetaDatas { get; set; }
    public DbSet<TrendingNews> TrendingNews { get; set; }
    public DbSet<SponsoredNews> SponsoredNews { get; set; }
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
    public DbSet<VideoNewsTag> VideoNewsTags { get; set; }
    public DbSet<LivediscourseVideo> LivediscourseVideos { get; set; }
    public DbSet<MediaInternship> MediaInternships { get; set; }
    public DbSet<RegionNewsRanking> RegionNewsRankings { get; set; }
}


public class BlankTriggerAddingConvention : IModelFinalizingConvention
{
    public virtual void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            var table = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
            if (table != null
                && entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(table.Value) == null)
                && (entityType.BaseType == null
                    || entityType.GetMappingStrategy() != RelationalAnnotationNames.TphMappingStrategy))
            {
                entityType.Builder.HasTrigger(table.Value.Name + "_Trigger");
            }

            foreach (var fragment in entityType.GetMappingFragments(StoreObjectType.Table))
            {
                if (entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(fragment.StoreObject) == null))
                {
                    entityType.Builder.HasTrigger(fragment.StoreObject.Name + "_Trigger");
                }
            }
        }
    }
}