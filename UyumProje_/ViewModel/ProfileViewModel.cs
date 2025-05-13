using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UyumProje_.Models;

namespace UyumProje_.ViewModel
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public List<Property> UserProperties { get; set; }

        public List<CustomerWishlist> CustomerWishlist { get; set;}

        public List<string> UserRoles { get; set; }
        public List<Booking> UserBookings { get; set; } // Yeni ekleme
        public List<BookingStatu> BookingStatuses { get; set; } // Yeni ekleme
        public List<Property> Properties { get; set; }


        public List<Review> Reviews { get; set; } // For handling existing reviews

        public int SelectedPropertyID { get; set; } // For the property where the user wants to leave a review
        public int Rating { get; set; } // User's rating
        public string Comment { get; set; } // User's comment

    }
}