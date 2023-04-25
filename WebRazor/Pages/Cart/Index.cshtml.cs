using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebRazor.Helpers;
using WebRazor.Models;

namespace WebRazor.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public Dictionary<Product, int> Cart { get; set; } = new Dictionary<Product, int>();

        public Account Auth { get; set; }
        [BindProperty(SupportsGet = true)]
        public Customer Customer { get; set; }

        [BindProperty]
        public DateTime RequireDate { get; set; }
        public decimal Sum { get; set; } = 0;

        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> OnGet()
        {
            RequireDate = DateTime.Now;
            var cart = HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;
            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            }
            else
            {
                list = new Dictionary<int, int>();
            }

            foreach (var item in list)
            {
                Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));

                Cart.Add(product, item.Value);

                Sum += (decimal)product.UnitPrice * item.Value;
            }
            Auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("CustSession"));
            if (Auth != null)
            {
                Customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId.Equals(Auth.CustomerId));
            }
            return Page();
        }

        [BindProperty]
        public string ContactName { get; set; }

        public async Task<IActionResult> OnPost()
        {
            var cart = HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;
            list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            if (cart == null || list.Count < 1)
            {
                ViewData["msg"] = "No product to order, please add products in your card!";
                return Page();

            }

            foreach (var item in list)
            {
                Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));

                Cart.Add(product, item.Value);

                Sum += (decimal)product.UnitPrice * item.Value;
            }

            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return Redirect("/Account/Login");
            }

            Auth = JsonSerializer.Deserialize<Account>(HttpContext.Session.GetString("CustSession"));

            if (Auth == null)
            {
                return Redirect("/Account/Login");
            }

            try
            {
                if (DateTime.Compare(RequireDate, DateTime.Now) < 0)
                {
                    ViewData["msg"] = "Require date must bigger than now!";
                    return Page();
                }

                Models.Order order = new Models.Order();
                order.CustomerId = Auth.CustomerId;
                order.OrderDate = DateTime.Now;
                if (RequireDate <= order.OrderDate)
                {
                    ViewData["fail"] = "Required Date is invalid";
                    return Page();
                }
                order.ShipAddress = Customer.Address;
                order.ShippedDate = DateTime.Now.AddDays(10);
                order.RequiredDate = RequireDate;

                await dbContext.Orders.AddAsync(order);
                await dbContext.SaveChangesAsync();
                order = await dbContext.Orders.OrderBy(o => o.OrderDate).LastOrDefaultAsync();
                Dictionary<Product, OrderDetail> listProducts = new Dictionary<Product, OrderDetail>();
                foreach (var item in list)
                {
                    Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));
                    OrderDetail od = new OrderDetail();
                    od.OrderId = order.OrderId;
                    od.ProductId = product.ProductId;
                    od.Quantity = (short)item.Value;
                    od.UnitPrice = (decimal)product.UnitPrice;
                    od.Discount = 0;
                    listProducts.Add(product, od);
                    await dbContext.OrderDetails.AddAsync(od);
                }
                await dbContext.SaveChangesAsync();

                ViewData["msg"] = "Order successfull";
                Dictionary<Stream, string> attachments = new Dictionary<Stream, string>();
                attachments.Add(new MemoryStream(PDFHelper.GenPDFInvoice(ContactName, order.ShipAddress, listProducts).Save()), "Invoice_" + order.OrderId + ".pdf");
                SendMailHelper.SendMail(Auth.Email, "Invoice orders", attachments);
                HttpContext.Session.Remove("cart");
            }
            catch (Exception e)
            {
                ViewData["fail"] = e.Message;
            }

            return Page();
        }
    }
}
