using System;
using System.Collections.Generic;
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
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCity { get; set; }
        public int ClassifiedId { get; set; }

        public virtual Classified Classified { get; set; }
    }
}
