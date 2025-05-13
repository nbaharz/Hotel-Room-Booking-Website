using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UyumProje_.Methods;
using UyumProje_.Models;
using UyumProje_.ViewModel;

namespace UyumProje_.Controllers
{
    public class BookingController : Controller
    {
        private readonly baharEntities1 db = new baharEntities1(); // DbContext sınıfınızı kullanın

        // GET: Booking/Create
        public ActionResult Create(int propertyID)
        {
            var property = db.Properties.Find(propertyID);
            int hostID = UserGet.GetUserId(User);
            if (property == null)
            {
                return HttpNotFound();
            }

            var model = new BookingViewModel
            {
                Bookings = new Booking
                {
                    PropertyID = propertyID,
                    CheckInDate = DateTime.Now,
                    CheckOutDate = DateTime.Now.AddDays(1), // Default values
                    HostID =hostID,
                },
                Properties = property
            };

            // Set the initial total price
            model.Bookings.TotalPrice = (model.Bookings.CheckOutDate - model.Bookings.CheckInDate).Days * property.PricePerNight;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Create(BookingViewModel viewModel)
        {
            int userId = UserGet.GetUserId(User); // Get user ID
            if (ModelState.IsValid)
            {
                var booking = viewModel.Bookings;
                var property = db.Properties.Find(booking.PropertyID);
                if (property != null)
                {
                    booking.TotalPrice = (booking.CheckOutDate - booking.CheckInDate).Days * property.PricePerNight;
                    booking.UserID = userId; // Set user ID
                    db.Bookings.Add(booking);
                    db.SaveChanges();

                    // Redirect to Payment page
                    return RedirectToAction("Payment", new { bookingId = booking.ID });
                }
            }

            // Reload the property information if validation fails
            viewModel.Properties = db.Properties.Find(viewModel.Bookings.PropertyID);
            return View(viewModel); // Return view with model data if there are errors
        }


        public ActionResult Payment(int bookingId)
        {
            var booking = db.Bookings.Find(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var viewModel = new PaymentViewModel
            {
                booking = booking,
                TotalPrice = booking.TotalPrice
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(PaymentViewModel viewModel)
        {
            var booking = db.Bookings.Find(viewModel.booking.ID);
            if (booking == null)
            {
                return HttpNotFound();
            }

            // Process payment (integrate with payment gateway)
            // For demo purposes, we assume payment is always successful

            // Add payment details to the Payments table
            var payment = new Payment
            {
                BookingID = viewModel.booking.ID,
                Amount = viewModel.TotalPrice,
                PaymentStatus = "Paid" // Change based on actual payment status
            };
            db.Payments.Add(payment);

            // Update booking status to "Pending"
            var bookingStatus = new BookingStatu
            {
                BookingID = viewModel.booking.ID,
                Status = "Pending"
            };
            db.BookingStatus.Add(bookingStatus);

            // Convert ExpiryDate to DateTime
            DateTime expiryDate = viewModel.ConvertToDateTime(viewModel.ExpiryDate);

            // Save credit card details
            var creditCard = new CreditCard
            {
                CustomerID = UserGet.GetUserId(User),
                CardNumber = viewModel.CardNumber,
                ExpiryDate = expiryDate, // Store in the same format
                CVV = viewModel.CVV
            };
            db.CreditCards.Add(creditCard);


            db.SaveChanges();
            
            booking.StatusID = bookingStatus.ID; // Assuming StatusID is the foreign key in the Booking table
            db.Entry(booking).State = EntityState.Modified;
            db.SaveChanges();

            // Redirect to confirmation page or back to profile
            return RedirectToAction("Confirmation", new { bookingId = viewModel.booking.ID });
        }
        public ActionResult Confirmation(int bookingId)
        {
            var booking = db.Bookings.Find(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            return View(booking);
        }

        [HttpPost]
        public JsonResult CheckAvailability(int propertyId, DateTime startDate, DateTime endDate)
        {
            var isAvailable = !db.Bookings.Any(b =>
                b.PropertyID == propertyId &&
                ((b.CheckInDate < endDate && b.CheckOutDate > startDate)));

            return Json(new { isAvailable });
        }

    }
}
