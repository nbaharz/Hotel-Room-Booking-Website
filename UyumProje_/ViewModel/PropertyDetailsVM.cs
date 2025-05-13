using System.Collections.Generic;

namespace UyumProje_.Models
{
    public class PropertyDetailsViewModel
    {
        public Property Property { get; set; }
        public List<Image> Images { get; set; }
        public List<Review> Reviews { get; set; } // Add this property

    }
}