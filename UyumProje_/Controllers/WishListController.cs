using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UyumProje_.Methods;
using UyumProje_.Models;

namespace UyumProje_.Controllers
{
    public class WishListController : Controller
    {
        // GET: WishList
        private readonly baharEntities1 _context = new baharEntities1();

        // GET: WishList/Add
        [Authorize] 
        public ActionResult Add(int propertyId)
        {
            // Kullanıcının kimliğini al
            var userId = UserGet.GetUserId(User); 
            if (userId == -1)
            {
                return RedirectToAction("Login", "Account"); // Eğer kullanıcı kimliği alınamazsa, giriş sayfasına yönlendir
            }

            // Yeni istek listesi öğesini oluştur
            var wishlistItem = new CustomerWishlist
            {
                CustomerID = userId, // User.Identity.GetUserId() genellikle string döner, bu yüzden parse etmeniz gerekebilir
                PropertyID = propertyId
            };

            // Veritabanına ekle
            _context.CustomerWishlists.Add(wishlistItem);
            _context.SaveChanges();

            // Kullanıcıyı istek listesi sayfasına yönlendir
            return View();
        }
    }
}