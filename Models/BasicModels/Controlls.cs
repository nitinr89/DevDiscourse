using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class Controlls
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
