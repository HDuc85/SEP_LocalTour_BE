﻿using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services;

public class StatisticService : IStatisticService
{
    readonly private IUnitOfWork _unitOfWork;

    public StatisticService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

   
    public async Task<Dictionary<int, int>> GetUserRegistrationByMonthAsync(int year)
    {
        var registrationByMonth = new Dictionary<int, int>();

        for (int month = 1; month <= 12; month++)
        {
            registrationByMonth[month] = 0;
        }

        var users = await _unitOfWork.RepositoryUser.GetDataQueryable()
            .Where(u => u.DateCreated.Year == year)
            .GroupBy(u => u.DateCreated.Month)
            .Select(group => new 
            {
                Month = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        foreach (var userGroup in users)
        {
            registrationByMonth[userGroup.Month] = userGroup.Count;
        }

        return registrationByMonth;
    }

    public async Task<int> GetTotalSuccessfulTravelsAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _unitOfWork.RepositoryTraveledPlace
            .GetDataQueryable(x => x.TimeArrive >= startDate && x.TimeArrive <= endDate)
            .CountAsync();
        return total;
    }

    public async Task<int> GetTotalSchedulesCreatedAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _unitOfWork.RepositorySchedule
            .GetDataQueryable(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
            .CountAsync();
        return total;
    }

    public async Task<int> GetTotalPostsCreatedAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _unitOfWork.RepositoryPost
            .GetDataQueryable(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
            .CountAsync();
        return total;
    }


    

    
}