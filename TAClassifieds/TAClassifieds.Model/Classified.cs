using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
    [Table("TAC_Classified")]
    public class Classified 
    {            
        public int CategoryId { get; set; }
        public int ClassifiedId { get; set; }

        [Required]
        public string ClassifiedTitle { get; set; }
        public string Summary { get; set; }

        [Required]
        public string Description { get; set; }
        public string ClassifiedImage { get; set; }
        [Required]
        public decimal ClassifiedPrice { get; set; }
        public System.DateTime PostedDate { get; set; }
        public System.Guid CreatedBy { get; set; }
       // public string ContactCity { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; }
        public virtual ICollection<ClassifiedContact> ClassifiedContacts { get; set; }
        //public virtual ClassifiedContact ClsfiedContacts { get; set; }
    }
}
