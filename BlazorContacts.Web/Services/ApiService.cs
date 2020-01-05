using BlazorContacts.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorContacts.Web.Services
{
    public class ApiService
    {
        public HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            // The ApiService service will use the IHttpClientFactory interface, which is the best way to use HttpClient in a server-side Blazor application. HttpClientFactory ensures that the sockets associated with each HttpClient instance are shared, thus preventing the issue of socket exhaustion.
            _httpClient = httpClient;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            var response = await _httpClient.GetAsync("api/contacts");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Contact>>(responseContent);
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contacts/{id}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Contact>(responseContent);
        }

        public async Task<string> GetContactStringByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contacts/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
