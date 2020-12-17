using Hasebni.Model.Base;
using Hasebni.Model.Security;
using Hasebni.Model.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hasebni.Model.Main
{
    public class Profile:EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Avatar { get; set; }
        public int Gender { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string Token { get; set; }

        public int HUserFk { get; set; }

        [ForeignKey(nameof(HUserFk))]
        public HUser HUser { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<Device> Devices { get; set; }

    }
}
