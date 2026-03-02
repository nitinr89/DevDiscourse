using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class DevNewsMetaData
    {
        [Key]
        public Guid DevNewsId { get; set; }
        public int Views { get; set; }
    }
}
