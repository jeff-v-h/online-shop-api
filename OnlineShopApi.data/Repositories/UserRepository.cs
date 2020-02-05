using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using PLP.Social.DataService.common.Models;
using OnlineShopApi.data.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseSettings _dbSettings;
        private static IMongoCollection<User> _collection;

        public UserRepository(IOptions<DatabaseSettings> dbOptions)
        {
            _dbSettings = dbOptions.Value;
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            SetupConnection(_dbSettings.ConnectionString, _dbSettings.DatabaseName, "users");
        }

        private void SetupConnection(string connectionString, string databaseName, string collectionName)
        {
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _collection = database.GetCollection<User>(collectionName);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to connect to database client: {e.Message}");
            }
        }

        public User Get(string username)
        {
            return _collection.Find(u => u.Username == username).FirstOrDefault();
        }

        public IEnumerable<User> GetAll()
        {
            return _collection.Find(u => true).ToList();
        }

        public async Task CreateAsync(User user) =>
            await _collection.InsertOneAsync(user);

        public async Task UpdateAsync(User user) =>
           await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);

        public async Task DeleteAsync(string username) =>
            await _collection.DeleteOneAsync(u => u.Username == username);
    }
}
