using Microsoft.AspNetCore.Mvc;
using testauth.Models;

namespace testauth.Services
{
    public interface IUserInfoRepository
    {

        public ActionResult<BackUser>? GetUserInfo();
    }
}
