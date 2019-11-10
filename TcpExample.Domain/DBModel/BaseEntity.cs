
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TcpExample.Domain.DBModel
{
    public class BaseEntity
    {
        [Required]
        [Column("DateCreated")]
        public DateTime? DateCreated { get; set; }
        [Required]
        [Column("UserCreated")]
        public Guid UserCreated { get; set; }
        [Required]
        [Column("DateModified")]
        public DateTime? DateModified { get; set; }
        [Required]
        [Column("UserModified")]
        public Guid UserModified { get; set; }

        [Required]
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}
