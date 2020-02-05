using AutoMapper;
using MongoDB.Bson;
using PLP.Social.DataService.data.Models;
using PLP.Social.DataService.data.Repositories;
using PLP.Social.DataService.data.XAL;
using OnlineShopApi.domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.domain.Managers
{
    public class SocialManager : ISocialManager
    {
        private readonly IMapper _mapper;
        private readonly ISocialRepository _repo;
        private readonly ISocialXmlAccessLayer _xal;
        private const string RecentlyAddedId = "5dd69c3c17fce357dc82444e";

        public SocialManager(IMapper mapper, ISocialRepository repo, ISocialXmlAccessLayer xal)
        {
            _mapper = mapper;
            _repo = repo;
            _xal = xal;
        }

        #region lists details
        public SocialMediaListsDetailsVM GetListsDetails()
        {
            var details = _repo.GetListsDetails();

            if (details == null)
                details = new SocialMediaListsDetails
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Created = DateTime.UtcNow,
                    Lists = new List<SocialListBase>()
                };

            if (details.Lists.Count < 1)
                CreateRecentlyAdded(details).Wait();

            var detailsVM = _mapper.Map<SocialMediaListsDetailsVM>(details);
            return detailsVM;
        }

        private async Task CreateRecentlyAdded(SocialMediaListsDetails details)
        {
            var now = DateTime.UtcNow;
            var name = "Recently Added";

            details.Lists.Add(new SocialListBase
            {
                Id = RecentlyAddedId,
                Created = now,
                Name = name
            });

            var updateListsDetailsTask = _repo.UpdateListsDetailsAsync(details.Id, details);
            var createListTask = _repo.CreateAsync(new SocialList
            {
                Id = RecentlyAddedId,
                Created = now,
                Name = name,
                MediaPosts = new List<MediaPost>()
            });

            await Task.WhenAll(updateListsDetailsTask, createListTask);
        }

        public async Task<SocialMediaListsDetailsVM> CreateListsDetails(SocialMediaListsDetailsVM detailsVM)
        {
            var details = _mapper.Map<SocialMediaListsDetails>(detailsVM);
            details.Created = DateTime.UtcNow;

            await _repo.CreateListsDetailsAsync(details);

            detailsVM.Id = details.Id;
            return detailsVM;
        }

        public async Task UpdateListsDetails(SocialMediaListsDetailsVM detailsVM)
        {
            var details = _repo.GetListsDetails();

            if (details == null)
                await CreateListsDetails(detailsVM);
            else
            {
                var updatedDetails = _mapper.Map<SocialMediaListsDetails>(detailsVM);
                var updateTask = _repo.UpdateListsDetailsAsync(details.Id, updatedDetails);
                var checkNamesTask = CheckListNames(details, updatedDetails);
                var deleteRemovedTask = DeleteRemovedLists(details, updatedDetails);

                await Task.WhenAll(updateTask, checkNamesTask, deleteRemovedTask);
            }
        }

        private async Task CheckListNames(SocialMediaListsDetails currentDetails, SocialMediaListsDetails updatedDetails)
        {
            var tasks = new List<Task>();
            foreach (var updatedListDetails in updatedDetails.Lists)
            {
                var listDetails = currentDetails.Lists.Find(l => l.Id == updatedListDetails.Id);

                // If it doesn't exist in old details, create new list doc
                if (listDetails == null)
                {
                    var newList = _mapper.Map<SocialList>(updatedListDetails);
                    newList.MediaPosts = new List<MediaPost>();
                    tasks.Add(_repo.CreateAsync(newList));
                }
                else if (listDetails.Name != updatedListDetails.Name)
                {
                    // Update list doc with name change
                    var list = _repo.Get(listDetails.Id);
                    list.Name = updatedListDetails.Name;
                    tasks.Add(_repo.UpdateAsync(list.Id, list));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task DeleteRemovedLists(SocialMediaListsDetails currentDetails, SocialMediaListsDetails updatedDetails)
        {
            var tasks = new List<Task>();
            foreach (var currentListDetails in currentDetails.Lists)
            {
                var listDetails = updatedDetails.Lists.Find(l => l.Id == currentListDetails.Id);

                // If list has been removed, remove the list doc
                if (listDetails == null)
                    tasks.Add(_repo.DeleteAsync(currentListDetails.Id));
            }

            await Task.WhenAll(tasks);
        }
        #endregion

        #region lists
        public SocialMediaListsVM Get()
        {
            var lists = _repo.Get();
            if (lists == null) return null;

            var listsVM = _mapper.Map<List<SocialListVM>>(lists);

            return new SocialMediaListsVM { Lists = listsVM };
        }

        public SocialListVM Get(string id)
        {
            var doc = _repo.Get(id);
            var docVM = (doc != null) ? _mapper.Map<SocialListVM>(doc) : null;
            return docVM;
        }

        public async Task<SocialListVM> Create(SocialListVM listVM)
        {
            listVM.Created = DateTime.UtcNow;
            var list = _mapper.Map<SocialList>(listVM);
            var createTask = _repo.CreateAsync(list);

            // Also add the new details into the listDetails
            var details = _repo.GetListsDetails();
            await createTask;
            details.Lists.Add(_mapper.Map<SocialListBase>(list));
            await _repo.UpdateListsDetailsAsync(details.Id, details);

            listVM.Id = list.Id;
            return listVM;
        }

        public async Task Update(string id, SocialListVM listVM)
        {
            var list = _repo.Get(id);

            if (list == null)
                await Create(listVM);
            else
            {
                var updatedList = _mapper.Map<SocialList>(listVM);
                var updateTask = _repo.UpdateAsync(id, updatedList);

                // Update the list details if name has changed
                if (list.Name != listVM.Name)
                {
                    var details = _repo.GetListsDetails();
                    var listDetails = details.Lists.Find(l => l.Id == id);
                    listDetails.Name = listVM.Name;
                    await _repo.UpdateListsDetailsAsync(details.Id, details);
                }

                await updateTask;
            }
        }

        public async Task AddItemsToList(string listId, List<MediaPostVM> posts)
        {
            var list = _repo.Get(listId);
            if (list == null) throw new Exception($"No list with id '{listId}' could be found");

            foreach (var post in posts)
            {
                AddDetailsToPost(post);
                list.MediaPosts.Add(_mapper.Map<MediaPost>(post));
            }

            await _repo.UpdateAsync(list.Id, list);
        }

        public async Task AddItemToList(string listId, MediaPostVM post)
        {
            var list = _repo.Get(listId);
            if (list == null) throw new Exception($"No list with id '{listId}' could be found");

            AddDetailsToPost(post);
            list.MediaPosts.Add(_mapper.Map<MediaPost>(post));
            await _repo.UpdateAsync(list.Id, list);
        }

        private void AddDetailsToPost(MediaPostVM post)
        {
            if (post.PostId == null || post.PostId.Length < 1) post.PostId = Guid.NewGuid().ToString();
            if (post.TimeAdded == null) post.TimeAdded = DateTime.UtcNow;
        }

        public async Task DeleteItemFromList(string listId, string id)
        {
            var list = _repo.Get(listId);
            if (list == null) throw new Exception($"No list with id '{listId}' could be found");

            var post = list.MediaPosts.Find(p => p.PostId == id);
            if (post != null)
            {
                list.MediaPosts.Remove(post);
                await _repo.UpdateAsync(list.Id, list);
            }
        }

        public async Task Delete(string id)
        {
            // Don't delete recently added
            if (id == RecentlyAddedId) return;

            var deleteTask = _repo.DeleteAsync(id);

            // Remove from lists details also
            var details = _repo.GetListsDetails();
            var listDetails = details.Lists.Find(l => l.Id == id);
            if (listDetails != null)
            {
                details.Lists.Remove(listDetails);
                await _repo.UpdateListsDetailsAsync(details.Id, details);
            }

            await deleteTask;
        }
        #endregion

        #region publish
        public async Task UpdateAndPublish(string id, SocialListVM listVM)
        {
            await Update(id, listVM);
            _xal.PublishList(_mapper.Map<SocialList>(listVM));
        }

        public void PublishAllLists()
        {
            var details = _repo.GetListsDetails();
            if (details != null)
            {
                foreach (var list in details.Lists)
                {
                    Publish(list.Id);
                }
            }
        }

        public bool? Publish(string id)
        {
            var list = _repo.Get(id);
            if (list == null) return null;

            _xal.PublishList(list);

            return true;
        }
        #endregion
    }
}
