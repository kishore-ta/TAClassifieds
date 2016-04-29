using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAClassifieds.Model;
namespace TAClassifieds.Data
{
    public class ClassifiedsContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
    }
}
