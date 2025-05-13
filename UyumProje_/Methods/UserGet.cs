using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UyumProje_.Models;

namespace UyumProje_.Methods
{
    public class UserGet
    {
        private static readonly baharEntities1 db = new baharEntities1(); // DbContext sınıfınızı kullanın

        public static int GetUserId(System.Security.Principal.IPrincipal user)
        {
            var username = user.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("User is not authenticated.");
            }

            var userEntity = db.Users.FirstOrDefault(u => u.Name == username);
            if (userEntity == null)
            {
                throw new Exception("User not found.");
            }

            return userEntity.ID;
        }
    }
}