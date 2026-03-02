namespace Devdiscourse.Models.ViewModel
{
    public class MLSourceNewsView
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Category { get; set; }
        public string NewsLabel { get; set; }
        public string AdminCheck { get; set; }
        public string AutoAssign { get; set; }
        public string Origin { get; set; }
        public string IsRepeat { get; set; }
        public string ImageUrl { get; set; }
        public string Sector { get; set; }
        public string ImageCopyright { get; set; }
        public string Edition { get; set; }
        public string Country { get; set; }
        public string ImageCaption { get; set; }
        public string SubTitle { get; set; }
        public List<NewsRankingViewModel> NewsRanking { get; set; }
    }
}