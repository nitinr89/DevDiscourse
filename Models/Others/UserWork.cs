using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.Others
{
    public class UserWork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string WorkStage { get; set; }
        public string ColorCode { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}