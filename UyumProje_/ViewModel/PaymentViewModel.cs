using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UyumProje_.Models;

namespace UyumProje_.ViewModel
{
    public class PaymentViewModel
    {

        public int PropertyID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public Booking booking { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }

        public DateTime ConvertToDateTime(string expiryDate)
        {
            DateTime parsedDate;
            if (DateTime.TryParseExact(expiryDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                // The date is successfully parsed
                return parsedDate;
            }
            else
            {
                // Handle parsing failure
                throw new FormatException("Invalid expiry date format.");
            }
        }

    }
}