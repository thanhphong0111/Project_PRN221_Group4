using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System;

namespace WebRazor.Pages.Admin.Product
{
    public class CreateModel : PageModel
    {
        private readonly WebRazor.Models.PRN221DBContext _context;
        

        public CreateModel(WebRazor.Models.PRN221DBContext context)
        {
            _context = context;
            
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return Page();
        }

        [BindProperty]
        public Models.Product Product { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();
           
            return RedirectToPage("./Index");
        }
    }
}
