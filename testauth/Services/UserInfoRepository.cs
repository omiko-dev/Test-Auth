﻿using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;
using testauth.Models;

namespace testauth.Services
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ActionResult<BackUser>? GetUserInfo()
        {

            if(_httpContextAccessor.HttpContext is not null)
            {
                var user = _httpContextAccessor?.HttpContext?.User;

                var userName = user?.FindFirstValue(ClaimTypes.Name);
                var userEmail = user?.FindFirstValue(ClaimTypes.Email);
                var userInfo = new BackUser { Name = userName, Email = userEmail };

                return userInfo;
            }
            return null;
        }  
    }
}
