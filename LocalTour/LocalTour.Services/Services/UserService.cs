using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace LocalTour.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public UserService(UserManager<User> userManager, IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileService = fileService;
        }
        public async Task<List<User>> GetAll()
        {
            var users = await _unitOfWork.RepositoryUser.GetData();
            return users.ToList();
        }
        public async Task<User> CheckLogin(string phoneNumber, string password)
        {

            var user = await FindByPhoneNumber(phoneNumber);
            if (user == null)
            {
                user = await FindByEmail(phoneNumber);
            }

            if (user == null)
            {
                return null;
            }
            
            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                return null;
            }

            return user;
        }
        public async Task<User> FindByPhoneNumber(string phoneNumber)
        {
            var user = await  _unitOfWork.RepositoryUser.GetData(x => x.PhoneNumber  == phoneNumber);
            if(!user.Any())
            {
                return null;
            }
            return user.First();
        }
        public async Task<User> FindById(string userid)
        {
            return await _userManager.FindByIdAsync(userid);
        }
        public async Task<User> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<bool> BanUser(string userId, DateTime timeEnd)
        {
            var user = await _userManager.FindByIdAsync(userId);            
            await  _unitOfWork.RepositoryUserBan.Insert(new UserBan()
            {
                EndDate = timeEnd,
                UserId = user.Id,
            });
            try
            {
               await _unitOfWork.CommitAsync();
               return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<User> CreateUser(User user)
        {
            var result = await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, "Visitor");
            if (result.Succeeded)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> CreateModerate(CreateUserRequest createUserRequest)
        {
            var check = await FindByEmail(createUserRequest.Email);
            if(check == null)
                check = await FindByPhoneNumber(createUserRequest.PhoneNumber);
            if (check == null)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    Email = createUserRequest.Email,
                    PhoneNumber = createUserRequest.PhoneNumber,
                    UserName = createUserRequest.PhoneNumber,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                };
                var result = await _userManager.CreateAsync(user, createUserRequest.Password);
                await _userManager.AddToRoleAsync(user, "Moderator");
                if (result.Succeeded)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
           return null;
        }
        public async Task<bool> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> AddRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> SetPassword(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return false;
            }
            await _userManager.RemovePasswordAsync(user);
            await  _userManager.AddPasswordAsync(user, password);

            return true;
        }
        public async Task<ServiceResponseModel<User>?> UpdateUser(string userId, UpdateUserRequest updateUserRequest, string requestUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return   new ( false,"Can not find User" );
            }
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddYears(-60);
            DateTime maxDate = today.AddYears(-18);

            if (updateUserRequest.DateOfBirth.HasValue)
                if (!(updateUserRequest.DateOfBirth <= maxDate) || !(updateUserRequest.DateOfBirth >= minDate))
                {
                    return new(false,$"{updateUserRequest.DateOfBirth} is too young or too old");
                }
                else
                {
                    user.DateOfBirth = updateUserRequest.DateOfBirth;
                }

            if(updateUserRequest.ProfilePicture != null)
            {
                var url = await _fileService.SaveImageFile(updateUserRequest.ProfilePicture, requestUrl);
                user.ProfilePictureUrl = url.Data;
            }

            if(updateUserRequest.Address != null)
            {
                user.Address  = updateUserRequest.Address;    
            }
            if (updateUserRequest.Gender != null)
            {
                user.Gender = updateUserRequest.Gender;
            }
            user.DateUpdated = today;

            await _userManager.UpdateAsync(user);

            return new ServiceResponseModel<User>(user);
        }
        public async Task<bool> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            return result.Succeeded;
        }
        public async Task<bool> IsUserBanned(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _unitOfWork.RepositoryUserBan.GetData(x => x.UserId == user.Id);
            if (!result.Any())
            {
                return false;
            }
            return true;
        }
    }
}
