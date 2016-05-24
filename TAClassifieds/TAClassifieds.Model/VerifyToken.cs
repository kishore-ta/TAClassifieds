using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAClassifieds.Model
{
    [Table("TAC_VerifyToken")]
    public class VerifyToken
    {
        [Key]
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public Nullable<bool> IsExpired { get; set; }
        public Nullable<bool> IsUsed { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
