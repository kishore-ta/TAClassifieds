using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
    [Table("TAC_ClassifiedContact")]
    public class ClassifiedContact
    {
        [Key]
        public int ContactId { get; set; }
        [DisplayName("Name*")]
        public string ContactName { get; set; }
        [DisplayName("Phone")]
        public string ContactPhone { get; set; }

        [DisplayName("City*")]
        [Required]
        public string ContactCity { get; set; }
        public int ClassifiedId { get; set; }

        public virtual Classified Classified { get; set; }
    }
}
