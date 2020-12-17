using Hasebni.Dashboard.Models;
using Hasebni.Dashboard.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hasebni.Dashboard.Pages
{
    public partial class Group
    {
        public List<GroupModel> GroupList { get; set; } = new List<GroupModel>();
        
        [Inject]
        public IGroupHttpRepository GroupHttpRepository { get; set; }
        protected async override Task OnInitializedAsync()
        {
            GroupList = await GroupHttpRepository.GetGroups();
        }

    }
}
