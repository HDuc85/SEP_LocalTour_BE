using LocalTour.Domain.Entities;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
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
                Task<ServiceResponseModel<User>> CheckLogin(string phoneNumber, string password);
                Task<User> FindByPhoneNumber(string phoneNumber);
                Task<User> FindByEmail(string email);
                Task<bool> BanUser(string userId, DateTime timeEnd);
                Task<User> CreateUser(User user);
                Task<List<User>> GetAll();
                Task<bool> SetPassword(string userId, string password);
                Task<bool> AddRole(string userId, string role);
                Task<bool> RemoveRole(string userId, string role);
                Task<ServiceResponseModel<User>?> UpdateUser(string userId, UpdateUserRequest updateUserRequest);
                Task<ServiceResponseModel<ChangePasswordResponse>> ChangePassword(string userId, ChangePasswordRequest request);
                Task<bool> IsUserBanned(string userId);
                Task<User> CreateModerate(CreateUserRequest createUserRequest);
                Task<ServiceResponseModel<UserProfileVM>> GetProfile(string userId, string currentUserId);
                Task<bool> UpdatePhoneOrEmail(User user);
                Task<bool> RemovePhone(String userId);
                Task<bool> RemoveEmail(String userId);
                Task<bool> UnbanUser(String userId);
                Task<List<GetUserRequest>> GetAllUsersAsync();
    }
}
