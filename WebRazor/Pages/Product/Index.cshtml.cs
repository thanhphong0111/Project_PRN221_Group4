using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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


        public async Task OnGetAsync()
        {
            Categories = dbContext.Categories.ToList();
            var products = dbContext.Products;
            if (CatId != null)
            {
                Products = await products
                    .Where(p => p.CategoryId == Int32.Parse(CatId))
                    .ToListAsync();
            }
            else
            {
                Products = await products.ToListAsync();
            }

        }
        public async Task OnPostAsync()
        {
            string search = Request.Form["search"];
            ViewData["search"] = search;
            Categories = dbContext.Categories.ToList();

            var products = dbContext.Products;
            if (!string.IsNullOrEmpty(search))
            {
                Products = await products.Where(p => p.ProductName == search).ToListAsync();
            }
            else if (CatId != null)
            {
                Products = await products
                    .Where(p => p.CategoryId == Int32.Parse(CatId))
                    .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(search) && CatId != null)
            {
                Products = await products.Where(p => p.CategoryId == Int32.Parse(CatId) && p.ProductName == search).ToListAsync();
            }
            else
            {
                Products = await products.ToListAsync();
            }

        }


    }
}
