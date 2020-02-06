using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineShopApi.data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public class WooliesService : IWooliesService
    {
        private readonly HttpClient _client;
        private IConfigurationSection _settings;

        public WooliesService(IConfiguration config, HttpClient client)
        {
            _client = client;
            _settings = config.GetSection("WooliesService");
            var host = _settings["Host"];

            _client.BaseAddress = new Uri(host);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "OnlineShopApi");
        }

        public User GetUser(int id)
        {
            return new User
            {
                Url = "https://online-shop-api.azurewebsites.net/api/shop",
                Token = _settings["Token"]
            };
        }

        public async Task<List<ShopperHistory>> GetShopperHistory()
        {
            var path = "/api/resource/shopperHistory";
            var response = await _client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ShopperHistory>>(jsonString);
                return result;
            }
            else throw new Exception(response.ReasonPhrase);
        }

        public async Task<List<Product>> GetProducts()
        {
            var path = "/api/resource/products";
            var response = await _client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Product>>(jsonString);
                return result;
            }
            else throw new Exception(response.ReasonPhrase);
        }
    }
}
