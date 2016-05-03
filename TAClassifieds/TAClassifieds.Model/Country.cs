using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
   public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
