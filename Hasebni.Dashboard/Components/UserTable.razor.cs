using Hasebni.Dashboard.Models;
using Hasebni.Dashboard.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hasebni.Dashboard.Components
{
    public partial class UserTable
    {
        [Parameter]
        public List<UserModel> Users { get; set; }

        //public void NanigateToGroups()
        //{
        //    NavigationManage.NavigateTo("/")
        //}
        public List<GroupModel> Groups { get; set; }
        [Inject]
        public IGroupHttpRepository GroupHttpRepository { get; set; }

        public async Task ShowGroups(int id)
        {
            Groups = await GroupHttpRepository.GetGroupsForUser(id);
        }
        protected override void OnInitialized()
        {
            Groups = new List<GroupModel>();
            base.OnInitialized();   
        }
    }
}
