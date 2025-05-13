using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UyumProje_.Models;
using UyumProje_.ViewModel;
using Uyumsoft_Proje.Security;

namespace Uyumsoft_Proje.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly baharEntities1 be = new baharEntities1();
        
        [AllowAnonymous]
        public ActionResult Index()
        {
            // Eğer kullanıcı giriş yapmışsa
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var userId = be.Users.FirstOrDefault(u => u.Name == username)?.ID;
                if (userId != null)
                {
                    ViewBag.IsAdmin = be.UserRoles.Any(ur => ur.UserID == userId && ur.Role == "Admin");
                }
                else
                {
                    ViewBag.IsAdmin = false;
                }
            }
            else
            {
                ViewBag.IsAdmin = false;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User u)
        {
            User user = be.Users.FirstOrDefault(x => x.Name == u.Name);
            if (user != null && VerifyPassword(u.Password, user.Password))
            {

                FormsAuthentication.SetAuthCookie(u.Name, false);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Handle login failure (e.g., display an error message)
                return View();
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SignUp(User u)
        {
            User user = be.Users.FirstOrDefault(x => x.Email == u.Email || x.Name == u.Name);
            if (user != null)
            {
                // Handle duplicate user (e.g., display an error message)
                return View();
            }
            else
            {
                u.Password = HashPassword(u.Password);
                be.Users.Add(u);

                // Assign "Consumer" role to the new user
                UserRole userRole = new UserRole
                {
                    UserID = u.ID,
                    Role = "Consumer"
                };
                be.UserRoles.Add(userRole);
                be.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult BecomeHost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BecomeHost(Property p, Amenity a, ListOfProperty l, HttpPostedFileBase Photo1, HttpPostedFileBase Photo2, HttpPostedFileBase Photo3)
        {
            int hostID = GetUserId();
            Property properties = new Property
            {
                Title = p.Title,
                Description = p.Description,
                PricePerNight = p.PricePerNight,
                Category = p.Category,
                MaxNumberOfGuests = p.MaxNumberOfGuests,
                HostID = hostID,
            };
            be.Properties.Add(properties);
            be.SaveChanges();

            ListOfProperty listOfProperty = new ListOfProperty
            {
                Host_ID = hostID,
                Property_ID = properties.ID,
            };

            be.ListOfProperties.Add(listOfProperty);

            UserRole userRole = new UserRole
            {
                UserID = hostID,
                Role = "Host"
            };

            be.UserRoles.Add(userRole);
            be.SaveChanges();

            SaveImageToDatabase(properties.ID, Photo1);
            SaveImageToDatabase(properties.ID, Photo2);
            SaveImageToDatabase(properties.ID, Photo3);


            ViewBag.Message = p.Title + " mülkü sistemimize eklendi.";
            return View();
        }

        [AllowAnonymous]        
        
        private int GetUserId()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                // Log or handle the case where the username is null or empty
                throw new Exception("User is not authenticated.");
            }

            var user = be.Users.FirstOrDefault(u => u.Name == username);
            if (user == null)
            {
                // Log or handle the case where the user is not found
                throw new Exception("User not found.");
            }

            return user.ID;
        }

        // Helper method to hash the password
        private string HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hashedPassword = ComputeHash(password, salt);
            return $"{salt}:{hashedPassword}";
        }

        // Helper method to verify the hashed password
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var parts = storedPassword.Split(':');
            if (parts.Length != 2) return false;

            var salt = parts[0];
            var hashedPassword = parts[1];

            var computedHash = ComputeHash(enteredPassword, salt);
            return computedHash == hashedPassword;
        }

        // Generate a new salt
        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Compute hash with salt
        private string ComputeHash(string password, string salt)
        {
            var saltedPassword = password + salt;
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void SaveImageToDatabase(int propertyId, HttpPostedFileBase photo)
        {
            if (photo != null && photo.ContentLength > 0)
            {
                var image = new Image
                {
                    PropertyID = propertyId,
                    FileName = photo.FileName,
                    ImageData = ConvertToBytes(photo)
                };
                be.Images.Add(image);
                be.SaveChanges();
            }
        }

        private byte[] ConvertToBytes(HttpPostedFileBase photo)
        {
            byte[] data = null;
            using (var binaryReader = new System.IO.BinaryReader(photo.InputStream))
            {
                data = binaryReader.ReadBytes(photo.ContentLength);
            }
            return data;
        }


    }
}
