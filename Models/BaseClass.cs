using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Devdiscourse.Models
{
    public class BaseClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public BaseClass()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
            if (ModifiedOn == new DateTime())
            {
                ModifiedOn = DateTime.UtcNow;
            }
        }
           
    }
}