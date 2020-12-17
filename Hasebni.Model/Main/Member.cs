using Hasebni.Model.Base;
using Hasebni.Model.Security;
using Hasebni.Model.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace Hasebni.Model.Main
{
    public class Member:EntityBase
    {
        public bool IsAdmin { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public long Balance { get; set; }

        public bool IsAlwaysAccepted { get; set; }

        public Profile Profile { get; set; }
        
        [ForeignKey(nameof(ProfileFk))]
        public int ProfileFk { get; set; }

        [ForeignKey(nameof(GroupFk))]
        public Group Group { get; set; }
        public int GroupFk { get; set; }
        public ICollection<Subscriber> Subscribers { get; set; }

       // [NotMapped]
        public ICollection<Notification> FromNotifications { get; set; }

       // [NotMapped]
        public ICollection<Notification> ToNotifications { get; set; }

    }
}
