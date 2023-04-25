using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public int categoryChoose { get; set; }
        public Dictionary<Models.Product, int> Cart { get; set; } = new Dictionary<Models.Product, int>();
        [BindProperty]
        public List<Category> Categories { get; set; }

        [BindProperty]
        public List<Models.Product> Products { get; set; } = new List<Models.Product>();

        [FromQuery(Name = "id")]
        public string CatId { get; set; }

        public void OnGet()
        {
            Categories = dbContext.Categories.ToList();

            var products = dbContext.Products.ToList();

            if (CatId != null)
            {
                Products = dbContext.Products
                    .Where(p => p.CategoryId == Int32.Parse(CatId))
                    .ToList();
            }
        }
    }
}
