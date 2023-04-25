using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebRazor.Models;
using System.Net.Http.Json;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebRazor.Models;

namespace WebRazor.Pages
{
    public class LoginModel : PageModel
    {

        private PRN221DBContext context;
        public string Error { get; set; }

        public LoginModel(PRN221DBContext context)
        {
            this.context = context;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("session") != null)
            {
                return Redirect("/Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string email = Request.Form["username"];
            string password = Request.Form["password"];

            if (email == string.Empty || password == string.Empty)
            {
                ModelState.AddModelError("Error", "email and password must fill up");
                return Page();
            }
            else if (context.Accounts.Where(a => a.Email.Equals(email.Trim()) && a.Password.Equals(password.Trim())).Count() < 1)
            {
                ModelState.AddModelError("Error", "email or password incorrect!");
                return Page();
            }

            Account account = context.Accounts.FirstOrDefault(u => u.Email.Equals(email));

            if (account.Role == 1)// role employee
            {
                account.Employee = context.Employees.Include(e => e.Department).FirstOrDefault(u => u.EmployeeId.Equals(account.EmployeeId));
                account.Employee.Department.Employees = null;
                account.Employee.Accounts = null;
            }
            else
            {
                account.Customer = context.Customers.FirstOrDefault(u => u.CustomerId.Equals(account.CustomerId));
                account.Customer.Accounts = null;
                if (account.Customer.Active == false)
                {
                    ModelState.AddModelError("Error", "your account disabled!");
                    return Page();
                }
            }

            string jsonAcount = JsonConvert.SerializeObject(account);
            HttpContext.Session.SetString("session", jsonAcount);

            return RedirectToPage("/Index");
        }
    }
}