using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class LivediscourseVideo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Title { get; set; }

        public string FilePath { get; set; }

        public string FileMimeType { get; set; }

        public string FileSize { get; set; }

        public string FileType { get; set; }

        public string FileCaption { get; set; }
        public string FileThumbUrl { get; set; }
        public string Duration { get; set; }

        public string Source { get; set; }
        public DateTime CreatedOn { get; set; }
        public long LivediscourseId { get; set; }
        [ForeignKey("LivediscourseId")]
        public virtual Livediscourse DevNews { get; set; }
        public LivediscourseVideo()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}