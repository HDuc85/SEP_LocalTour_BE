﻿using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using LocalTour.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;
using AutoMapper;

namespace LocalTour.Services.Services
{
    public class ScheduleLikeService : IScheduleLikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleLikeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> LikeScheduleAsync(int scheduleId, Guid userId)
        {
            // Kiểm tra xem người dùng đã "like" lịch trình chưa
            var existingLike = await _unitOfWork.RepositoryScheduleLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.ScheduleId == scheduleId && l.UserId == userId);

            if (existingLike != null)
            {
                // Nếu đã "like" thì "unlike"
                _unitOfWork.RepositoryScheduleLike.Delete(existingLike);
                await _unitOfWork.CommitAsync();
                return 2;  // Trả về 2 khi "unlike"
            }
            else
            {
                // Nếu chưa "like", thì thực hiện "like"
                var like = new ScheduleLike
                {
                    ScheduleId = scheduleId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.RepositoryScheduleLike.Insert(like);
                await _unitOfWork.CommitAsync();
                return 1;  // Trả về 1 khi "like"
            }
        }


        public async Task<bool> UnlikeScheduleAsync(int scheduleId, Guid userId)
        {
            var existingLike = await _unitOfWork.RepositoryScheduleLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.ScheduleId == scheduleId && l.UserId == userId);

            if (existingLike == null)
            {
                return false; // Not liked yet
            }

            _unitOfWork.RepositoryScheduleLike.Delete(existingLike);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<int> GetTotalLikesAsync(int scheduleId)
        {
            var likes = await _unitOfWork.RepositoryScheduleLike.GetData(l => l.ScheduleId == scheduleId);
            return likes.Count(); // Return total number of likes
        }

        public async Task<List<Guid>> GetUsersLikedAsync(int scheduleId)
        {
            var likes = await _unitOfWork.RepositoryScheduleLike.GetData(l => l.ScheduleId == scheduleId);
            return likes.Select(l => l.UserId).ToList();
        }
    }
}
