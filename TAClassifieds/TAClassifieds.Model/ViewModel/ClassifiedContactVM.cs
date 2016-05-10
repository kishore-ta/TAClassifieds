using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
    public class ClassifiedContactVM
    {
        [Key]
        public int ClassifiedId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Ad Title*")]
        public string ClassifiedTitle { get; set; }

        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

       
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Ad Description*")]
        public string Description { get; set; }
        public string ClassifiedImage { get; set; }

        [DisplayName("Quote Price")]
        public string ClassifiedPrice { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostedDate { get; set; }

        public int CategoryId { get; set; }
        public virtual Category categories { get; set; }

        public string CreatedBy { get; set; }
        public virtual User users { get; set; }

        public virtual ClassifiedContact classifiedsContacts { get; set; }

        public IEnumerable<Category> categoriesList { get; set; }
        public IEnumerable<Classified> classifiedList { get; set; }

        public IEnumerable<string> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
