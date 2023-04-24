using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebRazor.Pages.Admin.Product
{
    public class DeleteModel : PageModel
    {
        private readonly WebRazor.Models.PRN221DBContext _context;
        
        public DeleteModel(WebRazor.Models.PRN221DBContext context)
        {
            _context = context;
           
        }

        [BindProperty]
        public Models.Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                Product = product;
                _context.Products.Remove(Product);
                await _context.SaveChangesAsync();

            }
            
            return RedirectToPage("./Index");
        }
    }
}
