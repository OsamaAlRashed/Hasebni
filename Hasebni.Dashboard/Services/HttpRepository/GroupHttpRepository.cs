using Hasebni.Dashboard.Models;
using Hasebni.Dashboard.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hasebni.Dashboard.Services.HttpRepository.Group
{
    public class GroupHttpRepository : IGroupHttpRepository
    {
        public GroupHttpRepository(HttpClient client)
        {
            this.client = client;
        }

        public HttpClient client { get; }

        public async Task<List<GroupModel>> GetGroups()
        {
            //var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9467/Dashboard/GetAllGroups");

            //// add authorization header
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoib3NhbWFAY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiIiwiaXNzIjoiYSIsImF1ZCI6ImEifQ.xdVZzlkB3anQ8dBxetKMWxc_vtPTnd1dqglw8yqCyQU");
            //var response = await client.SendAsync(request);
            var response = await client.GetAsync("http://localhost:9467/Dashboard/GetAllGroups");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            var groups = JsonSerializer.Deserialize<IEnumerable<GroupModel>>
                (content, new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true }).ToList();
            return groups;
        }
       
        public async Task<List<GroupModel>> GetGroupsForUser(int userId)
        {
            var response = await client.GetAsync($"http://localhost:9467/Dashboard/GetAllGroupsForUser?userid={userId}");
            var content = await response.Content.ReadAsStringAsync();
            var groups = JsonSerializer.Deserialize<IEnumerable<GroupModel>>
                (content, new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true }).ToList();
            return groups;
        }
    }   
}
    