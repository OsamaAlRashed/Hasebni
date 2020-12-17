using Hasebni.Dashboard.Models;
using Hasebni.Dashboard.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hasebni.Dashboard.Services.HttpRepository
{
    public class UserHttpRepository : IUserHttpRepository
    {
        private readonly HttpClient client;
        public UserHttpRepository(HttpClient client)
        {
            this.client = client;
        }
        public async Task<List<UserModel>> GetUsers()
        {
            var response = await client.GetAsync("http://localhost:9467/Dashboard/GetAllUsers");
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<IEnumerable<UserModel>>
                (content, new JsonSerializerOptions 
                { PropertyNameCaseInsensitive = true }).ToList();
            return users;
        }
    }
}
