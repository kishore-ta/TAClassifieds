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
        public DbSet<User> Users { get; set; }      
        public DbSet<Classified> Classifieds { get; set; }
        public DbSet<ClassifiedContact> ClassifiedContacts { get; set; }
    }
    
}
