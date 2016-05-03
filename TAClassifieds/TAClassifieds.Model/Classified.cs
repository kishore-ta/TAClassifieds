using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
   public class Classified
    {
        public int UserId { get; set; }
        public int ClassifiedId { get; set; }
        public string ClassifiedTitle { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ClassifiedImage { get; set; }
        public decimal ClassifiedPrice { get; set; }
        public System.DateTime PostedDate { get; set; }
        public System.Guid CreatedBy { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ClassifiedContact> ClassifiedContacts { get; set; }
    }
}
