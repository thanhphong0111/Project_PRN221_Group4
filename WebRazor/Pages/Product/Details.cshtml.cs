using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;
using System.Text.Json;
namespace WebRazor.Pages.Product
{
    public class DetailsModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public DetailsModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Dictionary<Models.Product, int> Cart { get; set; } = new Dictionary<Models.Product, int>();

        [BindProperty]
        public Models.Product Product { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Categories = dbContext.Categories.ToList();
            if (id == null)
            {
                return NotFound();
            }

            Product = await dbContext.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            var cat = await dbContext.Categories.ToListAsync();

            var cart = HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;
            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
                foreach (var item in list)
                {
                    Models.Product product = dbContext.Products.FirstOrDefault(p => p.ProductId == item.Key);

                    Cart.Add(product, item.Value);
                }
            }
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Categories = dbContext.Categories.ToList();
            if (id == null)
            {
                return NotFound();
            }

            Product = await dbContext.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            var cat = await dbContext.Categories.ToListAsync();

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
