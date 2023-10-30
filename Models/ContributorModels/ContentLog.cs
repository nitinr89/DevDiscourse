using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ContributorModels
{
    public class ContentLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInfo { get; set; }
        public string MacAddress { get; set; }
        public string UserRegion { get; set; }
        public long NewsId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}