using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OnlineShopApi.data.Models;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public class SocialRepository : MongoBaseRepository<SocialList>, ISocialRepository
    {
        private static IMongoCollection<SocialMediaListsDetails> _listsDetailsCollection { get; set; }

        public SocialRepository(IConfiguration config) : base(config)
        {
            _listsDetailsCollection = _database.GetCollection<SocialMediaListsDetails>("sociallistsdetails");
        }

        // Collection that helps keep order of lists being shown
        public SocialMediaListsDetails GetListsDetails() =>
            _listsDetailsCollection.Find(doc => true).SortByDescending(doc => doc.Created).FirstOrDefault();

        public async Task CreateListsDetailsAsync(SocialMediaListsDetails doc) =>
            await _listsDetailsCollection.InsertOneAsync(doc);

        public async Task UpdateListsDetailsAsync(string id, SocialMediaListsDetails listDetails) =>
            await _listsDetailsCollection.ReplaceOneAsync(doc => doc.Id == id, listDetails, new UpdateOptions { IsUpsert = true });

        // CRUD for each social media list
        public SocialList Get(string id) =>
            GetBy(doc => doc.Id == id).FirstOrDefault();

        public async Task UpdateAsync(string id, SocialList list) =>
           await _collection.ReplaceOneAsync(l => l.Id == id, list);

        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(l => l.Id == id);
    }
}
