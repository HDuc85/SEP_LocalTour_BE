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
        public async Task<ServiceResponseModel<User>> CheckLogin(string phoneNumber, string password)
        {

            var user = await FindByPhoneNumber(phoneNumber);
            if (user == null)
            {
                user = await FindByEmail(phoneNumber);
            }

            if (user == null)
            {
                return new ServiceResponseModel<User>(success: false, message: "Phone number or email address not found");
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                return new ServiceResponseModel<User>(success: false, message: "Wrong password");
            }

            return new ServiceResponseModel<User>(success: true, user);
        }
        public async Task<User> FindByPhoneNumber(string phoneNumber)
        {
            var user = await _unitOfWork.RepositoryUser.GetData(x => x.PhoneNumber == phoneNumber);
            if (!user.Any())
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
            await _unitOfWork.RepositoryUserBan.Insert(new UserBan()
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
            if (check == null)
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
            if (user == null)
            {
                return false;
            }
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, password);

            return true;
        }
        public async Task<ServiceResponseModel<User>?> UpdateUser(string userId, UpdateUserRequest updateUserRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new(false, "User is not exist");
            }
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddYears(-60);
            DateTime maxDate = today.AddYears(-18);

            if (updateUserRequest.DateOfBirth.HasValue)
                if (!(updateUserRequest.DateOfBirth <= maxDate) || !(updateUserRequest.DateOfBirth >= minDate))
                {
                    return new(false, $"{updateUserRequest.DateOfBirth} is too young or too old");
                }
                else
                {
                    user.DateOfBirth = updateUserRequest.DateOfBirth;
                }

            if (updateUserRequest.FullName != null)
            {
                user.FullName = updateUserRequest.FullName;
            }
            if (updateUserRequest.ProfilePicture != null)
            {
                var url = await _fileService.SaveImageFile(updateUserRequest.ProfilePicture);
                user.ProfilePictureUrl = url.Data;
            }

            if (updateUserRequest.Address != null)
            {
                user.Address = updateUserRequest.Address;
            }
            if (updateUserRequest.Gender != null)
            {
                user.Gender = updateUserRequest.Gender;
            }

            if (updateUserRequest.FullName != null)
            {
                user.FullName = updateUserRequest.FullName;
            }
            user.DateUpdated = today;

            await _userManager.UpdateAsync(user);

            return new ServiceResponseModel<User>(user);
        }
        public async Task<ServiceResponseModel<bool>> ChangePassword(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponseModel<bool>(false, "User is not exist");
            }

            if (request.newPassword != request.confirmPassword)
            {
                return new ServiceResponseModel<bool>(false,"Passwords do not match"); 
            }
            
            var checkPassword = await _userManager.CheckPasswordAsync(user, request.oldPassword);
            if (!checkPassword)
            {
                return new ServiceResponseModel<bool>(false, "Old password is incorrect");
            }
            var result = await _userManager.ChangePasswordAsync(user, request.oldPassword, request.newPassword);
            if (result.Succeeded)
            {
                return new ServiceResponseModel<bool>(true, "Password changed successfully");
            }

            string error = string.Empty;
            foreach (var errorDescription in result.Errors)
            {
                error += errorDescription.Code + "\n";
            }
            return new ServiceResponseModel<bool>(false, $"{error}");
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

        public async Task<ServiceResponseModel<UserProfileVM>> GetProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponseModel<UserProfileVM>(false, "User is not exist");
            }

            var totalFollowers = await _unitOfWork.RepositoryFollowUser.GetData(x => x.UserId == user.Id);
            var totalPosteds = await _unitOfWork.RepositoryPost.GetData(x => x.AuthorId == user.Id);
            var totalFollowed = await _unitOfWork.RepositoryFollowUser.GetData(x => x.UserFollow == user.Id);
            var totalReviews = await _unitOfWork.RepositoryPlaceFeeedback.GetData(x => x.UserId == user.Id);
            var totalSchedules = await _unitOfWork.RepositorySchedule.GetData(x => x.UserId == user.Id);

            return new ServiceResponseModel<UserProfileVM>(true, new UserProfileVM
            {
                totalFollowers = totalFollowers.Count(),
                totalPosteds = totalPosteds.Count(),
                totalFollowed = totalFollowed.Count(),
                totalReviews = totalReviews.Count(),
                totalSchedules = totalSchedules.Count(),
                userName = user.UserName,
                phoneNumber = user.PhoneNumber,
                email = user.Email,
                address = user.Address,
                gender = user.Gender,
                userProfileImage = user.ProfilePictureUrl,
                dateOfBirth = user.DateOfBirth,
            });
        }
    }
}
