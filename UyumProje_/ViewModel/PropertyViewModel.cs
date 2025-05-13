using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UyumProje_.Models;

namespace UyumProje_.ViewModel
{
    public class PropertyViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxNumberOfGuests { get; set; }
    }
}