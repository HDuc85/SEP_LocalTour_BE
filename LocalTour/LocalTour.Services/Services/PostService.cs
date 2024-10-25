using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PostRequest> CreatePost(PostRequest postRequest)
        {
            if (postRequest == null) throw new ArgumentNullException(nameof(postRequest));

            var postEntity = _mapper.Map<Post>(postRequest);
            postEntity.CreatedDate = DateTime.UtcNow;
            postEntity.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.RepositoryPost.Insert(postEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PostRequest>(postEntity);
        }

        public async Task<PostRequest?> GetPostById(int postId)
        {
            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            return postEntity == null ? null : _mapper.Map<PostRequest>(postEntity);
        }

        public async Task<PostRequest?> UpdatePost(int postId, PostRequest postRequest)
        {
            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            if (postEntity == null) return null;

            _mapper.Map(postRequest, postEntity);
            postEntity.UpdateDate = DateTime.UtcNow;

            _unitOfWork.RepositoryPost.Update(postEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PostRequest>(postEntity);
        }

        public async Task<bool> DeletePost(int postId)
        {
            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            if (postEntity == null) return false;

            _unitOfWork.RepositoryPost.Delete(postEntity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<PaginatedList<PostRequest>> GetAllPosts(GetPostRequest request)
        {
            var posts = _unitOfWork.RepositoryPost.GetAll().AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                posts = posts.Where(p => p.Title.Contains(request.SearchTerm) || p.Content.Contains(request.SearchTerm));
            }

            // Mapping, sorting, and pagination
            var result = await posts.ListPaginateWithSortAsync<Post, PostRequest>(
                request.Page,
                request.Size,
                request.SortOrder,
                _mapper.ConfigurationProvider
            );

            return result;
        }


        //thêm mapper
        public PostRequest MapPostToRequest(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            return _mapper.Map<PostRequest>(post);
        }
    }
}
