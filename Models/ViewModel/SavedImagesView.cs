using System;

namespace Devdiscourse.Models.ViewModel
{
    public class SavedImagesView
    {
        public string ImageUrl { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string FileMimeType { get; internal set; }
        public string FileSize { get; internal set; }
        public string Sector { get; internal set; }
        public string Title { get; internal set; }
        public string Caption { get; internal set; }
        public string ImageCopyright { get; internal set; }
        public string Tags { get; internal set; }
    }
}