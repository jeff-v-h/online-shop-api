using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineShopApi.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public class WooliesService : IWooliesService
    {
        private readonly HttpClient _client;
        private IConfigurationSection _settings;
        private readonly string _token;

        public WooliesService(IConfiguration config, HttpClient client)
        {
            _client = client;
            _settings = config.GetSection("WooliesService");
            _token = _settings["Token"];
            var host = _settings["Host"];

            _client.BaseAddress = new Uri(host);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "OnlineShopApi");
        }

        public User GetUser(int id)
        {
            return new User
            {
                Url = $"https://online-shop-api.azurewebsites.net/api/shop",
                Token = _token
            };
        }

        public async Task<List<ShopperHistory>> GetShopperHistoryAsync()
        {
            var path = $"/api/resource/shopperHistory?token={_token}";
            var response = await _client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<ShopperHistory>>(jsonString);
                return result.ToList();
            }
            else throw new Exception(response.ReasonPhrase);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var path = $"/api/resource/products?token={_token}";
            var response = await _client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonString);
                return result.ToList();
            }
            else throw new Exception(response.ReasonPhrase);
        }

        public async Task<double> CalculateTrolleyTotal(Trolley trolley)
        {
            var json = JsonConvert.SerializeObject(trolley);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var path = $"/api/resource/trolleyCalculator?token={_token}";
            var response = await _client.PostAsync(path, data);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return double.Parse(jsonString);
            }
            else throw new Exception(response.ReasonPhrase);
        }
    }
}
