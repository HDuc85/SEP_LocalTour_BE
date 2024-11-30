using AutoMapper;
using Azure.Core;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.IdentityModel.Tokens;


namespace LocalTour.Services.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        public TagService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<TagRequest> CreateTag(TagRequest request)
        {
            var photoSaveResult = await _fileService.SaveImageFile(request.TagPhotoUrl);
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }

            var tagEntity = new Tag
            {
                TagName = request.TagName,
                TagPhotoUrl = photoSaveResult.Data,
            };
            await _unitOfWork.RepositoryTag.Insert(tagEntity);
            await _unitOfWork.CommitAsync();
            return request;
        }

        public async Task<bool> DeleteTag(int tagid)
        {
            var tagEntity = await _unitOfWork.RepositoryTag.GetById(tagid);
            if (tagEntity != null)
            {
                _unitOfWork.RepositoryTag.Delete(tagEntity);
            }
            if (!string.IsNullOrEmpty(tagEntity.TagPhotoUrl))
            {
                await _fileService.DeleteFile(tagEntity.TagPhotoUrl);
            }
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<PaginatedList<TagViewModel>> GetAllTag(PaginatedQueryParams request)
        {
            var tags = _unitOfWork.RepositoryTag.GetAll()
                                                   .AsQueryable();

            if (request.SearchTerm is not null)
            {
                tags = tags.Where(e => e.TagName.Contains(request.SearchTerm));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                tags = tags.OrderByCustom(request.SortBy, request.SortOrder);
            }

            return await tags
                .ListPaginateWithSortAsync<Tag, TagViewModel>(
                request.Page,
                request.Size,
                request.SortBy,
                request.SortOrder,
                _mapper.ConfigurationProvider);
        }

        public async Task<Tag> GetTagById(int tagid)
        {
            var tag = await _unitOfWork.RepositoryTag.GetById(tagid);
            if (tag == null)
            {
                throw new KeyNotFoundException($"Tag with ID {tagid} not found.");
            }
            return tag;
        }

        public async Task<TagRequest> UpdateTag(int tagid, TagRequest request)
        {
            var existingTag = await _unitOfWork.RepositoryTag.GetById(tagid);
            if (existingTag == null)
            {
                throw new ArgumentException($"Tag with id {tagid} not found.");
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (!string.IsNullOrEmpty(existingTag.TagPhotoUrl))
            {
                await _fileService.DeleteFile(existingTag.TagPhotoUrl);
            }
            var photoSaveResult = await _fileService.SaveImageFile(request.TagPhotoUrl);
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }
            existingTag.TagName = request.TagName;
            existingTag.TagPhotoUrl = photoSaveResult.Data;

            _unitOfWork.RepositoryTag.Update(existingTag);
            await _unitOfWork.CommitAsync();
            return request;
        }

        public async Task<List<TagViewModel>> GetTagsTopPlace(String? userId)
        {
            var userTags = !userId.IsNullOrEmpty()
                ? _unitOfWork.RepositoryUserPreferenceTags.GetDataQueryable()
                    .Where(ut => ut.UserId == Guid.Parse(userId))
                    .Select(ut => ut.TagId)
                : Enumerable.Empty<int>();
            
            var tagPlaceCounts = _unitOfWork.RepositoryPlaceTag.GetDataQueryable()
                .GroupBy(pt => pt.TagId)
                .Select(g => new
                {
                    TagId = g.Key,
                    PlaceCount = g.Count()
                });
            
            var userPreferredTags = tagPlaceCounts
                .Where(tag => userTags.Contains(tag.TagId))
                .OrderByDescending(tag => tag.PlaceCount)
                .Take(10)  
                .ToList();

            var additionalTags = tagPlaceCounts
                .Where(tag => !userTags.Contains(tag.TagId))
                .OrderByDescending(tag => tag.PlaceCount)
                .Take(10 - userPreferredTags.Count())  
                .ToList();
            
            
            var tags = await _unitOfWork.RepositoryTag.GetData();
            var result = userPreferredTags
                .Concat(additionalTags)
                .Select(tag => new TagViewModel
                {
                    Id = tag.TagId,
                    TagPhotoUrl = tags.Single(x => x.Id == tag.TagId).TagPhotoUrl,
                    TagName = tags.Single(x => x.Id == tag.TagId).TagName,
                })
                .ToList();
            return result;
        }
        
    }
}
