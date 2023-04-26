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

        public const int pageSize = 12;
        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public async Task OnGetAsync()
        {
            Categories = dbContext.Categories.ToList();
            int totalProduct = getTotalProducts();
            var products = dbContext.Products;
            countPages = (int)Math.Ceiling((double)totalProduct / pageSize);
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }
            if (CatId != null)
            {
                Products = await (from p in dbContext.Products select p).Where(p => p.CategoryId == int.Parse(CatId))
                    .Skip((currentPage - 1) * pageSize).Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                Products = await (from a in dbContext.Products orderby a.ProductId ascending select a).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            }
        }
        public async Task OnPostAsync()
        {
            int totalProduct = getTotalProducts();
            Categories = dbContext.Categories.ToList();

            var products = dbContext.Products;

            if (CatId != null)
            {
                Products = await (from p in dbContext.Products select p).Where(p => p.CategoryId == int.Parse(CatId))
                     .Skip((currentPage - 1) * pageSize).Take(pageSize)
                     .ToListAsync();
            }

            else
            {
                Products = await (from a in dbContext.Products orderby a.ProductId ascending select a).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            }

        }
        private int getTotalProducts()
        {
            var list = (from p in dbContext.Products select p).ToList();


            if (CatId != null)
            {
                return list.Where(p => p.CategoryId == int.Parse(CatId)).ToList().Count;
            }
            else
            {
                return list.Count;
            }
        }

    }
}
