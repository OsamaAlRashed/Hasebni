using Hasebni.Dashboard.Models;
using Hasebni.Dashboard.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hasebni.Dashboard.Pages
{
    public partial class User
    {
        public List<UserModel> UserList { get; set; } = new List<UserModel>();
        [Inject]
        public IUserHttpRepository UserRepo { get; set; }
        protected async override Task OnInitializedAsync()
        {
            UserList = await UserRepo.GetUsers();
        }
    }
}
