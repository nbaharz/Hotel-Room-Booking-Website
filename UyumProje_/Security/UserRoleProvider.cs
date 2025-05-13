using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using UyumProje_.Models;

namespace Uyumsoft_Proje.Security
{
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            baharEntities1 be = new baharEntities1();
            User u = be.Users.FirstOrDefault(x => x.Name == username);
            if (u != null)
            {
                var roles = be.UserRoles
                              .Where(ur => ur.UserID == u.ID)
                              .Select(ur => ur.Role)
                              .ToArray();
                return roles;
            }
            else
            {
                return new string[] { }; // Kullanıcı bulunamazsa boş bir dizi döndür
            }
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}