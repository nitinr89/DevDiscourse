namespace Devdiscourse.Models.ViewModel
{
    public class NewsFileCache
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string FilePath { get; set; }

        public string FileMimeType { get; set; }

        public string FileSize { get; set; }

        public string FileType { get; set; }

        public string FileCaption { get; set; }
        public string FileThumbUrl { get; set; }
        public string Duration { get; set; }

        public string Source { get; set; }

        public Guid NewsId { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}