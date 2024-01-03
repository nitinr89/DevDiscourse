namespace Devdiscourse.Models.ViewModel
{
    public class ApiNewsView
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string? ImageUrl { get; set; }
        public string? Subtitle { get; set; }
        public string? Country { get; set; }
        public long NewsId { get; set; }
        public string? Tags { get; set; }
        public string? Slug { get; set; }
        public string? Type { get; set; }
        public string? SubType { get; set; }
        public string? Label { get; set; }
        public string? ImageCopyright { get; set; }
        public string? Source { get; set; }
        public string? SourceUrl { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime PublishedOn { get; set; }
        public string? PublishedOnString { get; set; }
        public string? ModifiedOnString { get; set; }
        public string? Themes { get; set; }
        public string? Avatar { get; set; }
        public string? Sector { get; set; }
        public string? SectorSlug {get; set; }
        public string? TagArray {get; set; }
        public string? Disclaimer {get; set; }
        public Boolean IsBlog {get; set; }
    }
    }