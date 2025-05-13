using System.Linq;
using System.Web.Mvc;
using UyumProje_.Models;
using UyumProje_.ViewModel;

namespace Uyumsoft_Proje.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly baharEntities1 be = new baharEntities1();

        public ActionResult Index()
        {
            var model = new AdminViewModel
            {
                Users = be.Users.ToList(),
                Properties = be.Properties.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            var user = be.Users.Find(userId);
            if (user != null)
            {
                // Delete user
                be.Users.Remove(user);
                // Optionally, remove user roles or related data
                var userRoles = be.UserRoles.Where(ur => ur.UserID == userId);
                be.UserRoles.RemoveRange(userRoles);
                be.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteProperty(int propertyId)
        {
            var property = be.Properties.Find(propertyId);
            if (property != null)
            {
                // Delete property
                be.Properties.Remove(property);
                //// Optionally, remove related data such as amenities
                //var amenities = be.Amenities.Where(a => a.PropertyID == propertyId);
                //be.Amenities.RemoveRange(amenities);
                be.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
