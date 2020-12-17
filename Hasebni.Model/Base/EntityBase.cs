using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hasebni.Model.Base
{
    public abstract class EntityBase
    {

        [Key]
        public int Id { get; set; }
        public DateTimeOffset? DateDeleted { get; set; }
        public bool IsDeleted => DateDeleted.HasValue;
    }
}
