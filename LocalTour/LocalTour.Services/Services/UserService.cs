using LocalTour.Data;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(UserManager<User> userManager,RoleManager<Role> roleManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> CheckLogin(string phonenumber, string password)
        {

            var user = await FindByPhoneNumber(phonenumber);
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

        public async Task<User> FindByPhoneNumber(string phonenumber)
        {
            var user = await  _unitOfWork.RepositoryUser.GetData(x => x.PhoneNumber  == phonenumber);
            if(user == null)
            {
                return null;
            }
            return user.First();
        }
        public async Task<User> FindById(string userid)
        {
            return await _userManager.FindByIdAsync(userid);
        }
        public async Task<bool> BanUser(string phonenumber, DateTime timeEnd)
        {
            var user = await FindByPhoneNumber(phonenumber);
           

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


    }
}
