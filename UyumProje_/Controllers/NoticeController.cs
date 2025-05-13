using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using UyumProje_.Models;

namespace UyumProje_.Controllers
{
    public class NoticeController : Controller
    {
        private readonly baharEntities1 _context;

        public NoticeController()
        {
            _context = new baharEntities1();
        }

        // GET: Notice
        public async Task<ActionResult> Index()
        {
            var properties = await _context.Properties.ToListAsync();
            return View(properties);
        }

        // GET: Notice/Details/5
        // GET: Notice/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            // Retrieve the property
            var property = await _context.Properties
                .Include(p => p.User) // Adjust based on your actual navigation property name
                .FirstOrDefaultAsync(p => p.ID == id);

            if (property == null)
            {
                return HttpNotFound();
            }

            // Retrieve images associated with the property
            var images = await _context.Images
                .Where(i => i.PropertyID == id)
                .ToListAsync();

            // Retrieve reviews associated with the property
            var reviews = await _context.Reviews
                .Where(r => r.PropertyID == id)
                .Include(r => r.User) // Assuming you want to include user details in the reviews
                .ToListAsync();

            // Pass property, images, and reviews to the view
            var viewModel = new PropertyDetailsViewModel
            {
                Property = property,
                Images = images,
                Reviews = reviews
            };

            return View(viewModel);
        }


    }
}
