using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Cart
{
    public class UpModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public UpModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public Models.Account Auth { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return Redirect("/Account/Login");
            }

            var custSession = HttpContext.Session.GetString("CustSession");
            if (custSession == null)
            {
                return Redirect("/Account/Login");
            }
            else
            {
                Auth = JsonSerializer.Deserialize<Models.Account>(custSession);
                if (Auth == null)
                {
                    return Redirect("/Account/Login");
                }
            }

            Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id));


            try
            {
                Dictionary<int, int> list;
                var session = HttpContext.Session.GetString("cart");

                if (session == null)
                {
                    list = new Dictionary<int, int>();
                }
                else
                {
                    list = JsonSerializer.Deserialize<Dictionary<int, int>>(session);
                }

                if (list.ContainsKey((int)id))
                {
                    list[(int)id] += 1;
                }


                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(list));
                TempData["success"] = "Quanity up successfull";
            }
            catch (Exception e)
            {
                TempData["fail"] = e.Message;
            }

            return Redirect("/Cart");
        }
    }
}
