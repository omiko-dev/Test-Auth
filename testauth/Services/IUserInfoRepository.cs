using Microsoft.AspNetCore.Mvc;

namespace testauth.Services
{
    public interface IUserInfoRepository
    {

        public ActionResult<List<string>> GetUserInfo();
    }
}
