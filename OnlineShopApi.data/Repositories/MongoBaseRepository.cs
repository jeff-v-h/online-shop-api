using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public class MongoBaseRepository<T> : IDocumentRepository<T>
    {
        private readonly IConfiguration _config;
        private static IMongoClient _client { get; set; }
        internal static IMongoDatabase _database { get; set; }
        internal static IMongoCollection<T> _collection { get; set; }

        public MongoBaseRepository(IConfiguration config)
        {
            _config = config;
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            SetupConnection();
        }

        internal void SetupConnection(string configSection = "DatabaseSettings", string connectionString = "ConnectionString",
            string databaseName = "DatabaseName", string collectionName = "CollectionName")
        {
            var section = _config.GetSection(configSection);
            Connect(section[connectionString]);

            _database = _client.GetDatabase(section[databaseName]);
            _collection = _database.GetCollection<T>(section[collectionName]);
        }

        public void Connect(string connectionString)
        {
            try
            {
                _client = new MongoClient(connectionString);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to connect to database client: {e.Message}");
            }
        }

        public IFindFluent<T, T> GetBy(Expression<Func<T, bool>> predicate) =>
            _collection.Find(predicate);

        public List<T> Get() =>
            GetBy(doc => true).ToList();

        public async Task CreateAsync(T doc) =>
            await _collection.InsertOneAsync(doc);
    }
}
