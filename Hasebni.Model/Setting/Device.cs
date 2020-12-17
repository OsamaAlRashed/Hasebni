using Hasebni.Model.Base;
using Hasebni.Model.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hasebni.Model.Setting
{
    public class Device:EntityBase
    {
        [Column(TypeName = "nvarchar(max)")]
        public string DeviceToken { get; set; }

        public int ProfileId { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public Profile Profile { get; set; }
    }
}
