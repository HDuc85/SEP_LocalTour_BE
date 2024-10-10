using LocalTour.Domain.Entities;
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
    }
}
