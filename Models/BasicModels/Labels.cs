using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Devdiscourse.Models.BasicModels
{
    public class Labels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SrNo { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}