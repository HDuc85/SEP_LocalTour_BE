using LocalTour.Domain.Entities;
using LocalTour.Services.Model;
using LocalTour.WebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IUserService
    {
        Task<User> FindById(string userid);
        Task<User> CheckLogin(string phonenumber, string password);
        Task<User> FindByPhoneNumber(string phonenumber);
        Task<bool> BanUser(string phonenumber, DateTime timeEnd);
        Task<User> CreateUser(User user);
        Task<List<User>> GetAll();
        Task<bool> SetPassword(string phonenumber, string password);
        Task<bool> AddRole(string phoneNumber, string role);
        Task<bool> RemoveRole(string phoneNumber, string role);
        Task<ServiceResponseModel<User>?> UpdateUser(string phonenumber, UpdateUserRequest updateUserRequest, string requestUrl);
        Task<bool> ChangePassword(string phoneNumber, string oldPassword, string newPassword);
    }
}
