using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAClassifieds.Model
{
    [Table("TAC_User")]
   public class User
    {
        [Key]
        public System.Guid UserId { get; set; }
        public string Email { get; set; }
        public string UPassword { get; set; }
        [Required]
        [Column("First Name")]
        public string First_Name { get; set; }
        [Required]
        [Column("Last Name")]
        public string Last_Name { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Column("Country")]
        public Nullable<int> CountryId { get; set; }
    
        public virtual ICollection<Classified> Classified { get; set; }
        public virtual Country Country { get; set; }
    }
}

