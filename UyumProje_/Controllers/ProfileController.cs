using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UyumProje_.Models;
using UyumProje_.ViewModel;

namespace UyumProje_.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        private baharEntities1 be = new baharEntities1();

        [Authorize]
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            var user = be.Users.FirstOrDefault(u => u.Name == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = be.UserRoles
                .Where(ur => ur.UserID == user.ID)
                .Select(ur => ur.Role)
                .ToList();

            var model = new ProfileViewModel
            {
                UserName = user.Name,
                UserRoles = userRoles
            };

            if (userRoles.Contains("Host"))
            {
                var userProperties = be.Properties.Where(p => p.HostID == user.ID).ToList();
                var userBookings = be.Bookings.Where(b => b.HostID == user.ID).ToList();
                var userBookingIds = userBookings.Select(ub => ub.ID).ToList();
                var bookingStatuses = be.BookingStatus
                    .Where(bs => userBookingIds.Contains(bs.Booking.ID))
                    .ToList();

                model.UserProperties = userProperties;
                model.UserBookings = userBookings;
                model.BookingStatuses = bookingStatuses;
            }
            if (userRoles.Contains("Consumer"))
            {
                var userWishlist = be.CustomerWishlists.Where(w => w.CustomerID == user.ID).ToList();
                var userBookings = be.Bookings.Where(b => b.UserID == user.ID).ToList();
                var userBookingIds = userBookings.Select(ub => ub.ID).ToList();
                var bookingStatuses = be.BookingStatus
                    .Where(bs => userBookingIds.Contains(bs.Booking.ID))
                    .ToList();
                var properties = be.Properties.ToList(); // Fetch all properties
                var reviews = be.Reviews.Where(r => r.UserID == user.ID).ToList(); // Fetch reviews

                model.CustomerWishlist = userWishlist;
                model.UserBookings = userBookings;
                model.BookingStatuses = bookingStatuses;
                model.Properties = properties; // Pass all properties
                model.Reviews = reviews; // Pass user reviews
            }

            return View(model);
        }



        public ActionResult Edit(int id)
        {
            var property = be.Properties.FirstOrDefault(p => p.ID == id);
            if (property == null)
            {
                return HttpNotFound();
            }

            var model = new PropertyViewModel
            {
                ID = property.ID,
                Title = property.Title,
                Description = property.Description,
                Category = property.Category,
                PricePerNight = property.PricePerNight,
                MaxNumberOfGuests = property.MaxNumberOfGuests
            };

            return View(model);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult Edit(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var property = be.Properties.FirstOrDefault(p => p.ID == model.ID);
                if (property == null)
                {
                    return HttpNotFound();
                }

                property.Title = model.Title;
                property.Description = model.Description;
                property.Category = model.Category;
                property.PricePerNight = model.PricePerNight;
                property.MaxNumberOfGuests = model.MaxNumberOfGuests;

                be.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var property = be.Properties.FirstOrDefault(p => p.ID == id);
            if (property == null)
            {
                return HttpNotFound();
            }

            be.Properties.Remove(property);
            be.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult RemoveFromWishlist(int id)
        {
            var username = User.Identity.Name;
            var user = be.Users.FirstOrDefault(u => u.Name == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            var wishlistItem = be.CustomerWishlists.FirstOrDefault(w => w.ID == id && w.CustomerID == user.ID);

            if (wishlistItem == null)
            {
                return HttpNotFound();
            }

            be.CustomerWishlists.Remove(wishlistItem);
            be.SaveChanges();

            return RedirectToAction("Index");
        }

        //yeni ekleme
        [Authorize]
        public ActionResult ApproveBooking(int id)
        {
            var booking = be.Bookings.FirstOrDefault(b => b.ID == id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var bookingStatus = be.BookingStatus.FirstOrDefault(bs => bs.BookingID == id);
            if (bookingStatus != null)
            {
                bookingStatus.Status = "Approved"; // veya "Onaylı"
                be.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult DeleteBooking(int id)
        {
            var booking = be.Bookings.FirstOrDefault(b => b.ID == id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            be.Bookings.Remove(booking);

            var bookingStatus = be.BookingStatus.FirstOrDefault(bs => bs.BookingID == id);
            if (bookingStatus != null)
            {
                be.BookingStatus.Remove(bookingStatus);
            }

            be.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult SubmitReview(int bookingID, string comment, int? rating)
        {
            var username = User.Identity.Name;
            var user = be.Users.FirstOrDefault(u => u.Name == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            var booking = be.Bookings.FirstOrDefault(b => b.ID == bookingID && b.UserID == user.ID);

            if (booking == null)
            {
                return HttpNotFound();
            }

            var existingReview = be.Reviews.FirstOrDefault(r => r.PropertyID == booking.PropertyID && r.UserID == user.ID);

            if (existingReview != null)
            {
                existingReview.Comment = comment;
                existingReview.Rating = rating;
            }
            else
            {
                var review = new Review
                {
                    UserID = user.ID,
                    PropertyID = booking.PropertyID,
                    Rating = rating,
                    Comment = comment
                };

                be.Reviews.Add(review);
            }

            be.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}