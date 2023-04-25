
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebRazor.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebRazor.Pages
{
    public class RegisterModel : PageModel
    {
        private PRN221DBContext context;
        public string Error { get; set; }

        public RegisterModel(PRN221DBContext context) { 
            this.context = context;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string company = Request.Form["company"];
            string rePassword = Request.Form["repass"];
            string contactName = Request.Form["contactName"];
            string address = Request.Form["address"];
            string title = Request.Form["title"];
            if (email == string.Empty)
            {
                ModelState.AddModelError("Error", "email cannot empty!");
                return Page();
            }
            else if(password == string.Empty)
            {
                ModelState.AddModelError("Error", "password cannot empty!");
                return Page();
            }
            else if(company == string.Empty)
            {
                ModelState.AddModelError("Error", "company cannot empty!");
                return Page();
            }
            else if(rePassword == string.Empty)
            {
                ModelState.AddModelError("Error", "Password confirm cannot empty!");
                return Page();
            }
            else if(rePassword != password)
            {

                ModelState.AddModelError("Error", "Password confirm doesn't matching!");
                return Page();
            }
            else if (context.Accounts.Where(a=>a.Email.Equals(email)).ToList().Count > 0){
                
                ModelState.AddModelError("Error", "email was existed!");
                return Page();
            }
                string idCus = genIdCus();
                context.Customers.Add(new Customer()
                {
                    Active= true,
                    CreateDate= DateTime.Now,
                    CustomerId= idCus,
                    CompanyName = company,
                    ContactName= contactName,
                    ContactTitle = title,
                    Address= address
                   
                });
                context.SaveChanges();

                context.Accounts.Add(new Account()
                {
                  
                    CustomerId = idCus,
                    Email = email,
                    Password = password,
                    Role = 2
                });

            context.SaveChanges();

            Account account = context.Accounts.FirstOrDefault(u => u.Email.Equals(email));
            account.Customer = context.Customers.FirstOrDefault(u => u.CustomerId.Equals(account.CustomerId));
            account.Customer.Accounts = null;

            string jsonAcount = JsonConvert.SerializeObject(account);
            HttpContext.Session.SetString("session", jsonAcount);
            return Redirect("/Index");
        }

        private string genIdCus()
        {
            Random random = new Random();
            string id = "";
            for (int i = 0; i < 5; i++)
            {
                char c = (char)random.Next('a', 'z' + 1);
                id += c;
            }
            if(context.Customers.Where(c=>c.CustomerId.Equals(id.ToUpper())).ToList().Count > 0)
            {
               return genIdCus();
            }
            return id.ToUpper();
        }
    }
    
}
