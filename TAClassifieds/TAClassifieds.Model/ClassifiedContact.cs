using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
   public class ClassifiedContact
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCity { get; set; }
        public int ClassifiedId { get; set; }

        public virtual Classified Classified { get; set; }
    }
}
