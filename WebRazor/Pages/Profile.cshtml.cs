using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebRazor.Models;
using System.Text.Json;

namespace WebRazor.Pages
{
    public class ProfileModel : PageModel
    {
        public string Error { get; set; }

        private PRN221DBContext context;

        public ProfileModel(PRN221DBContext context)
        {
            this.context = context;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("session") != null)
            {
                return Page();
            }

            return Redirect("/Login");

        }

        public IActionResult OnPost()
        {
            var jsonAccount = HttpContext.Session.GetString("session");
            Account account = JsonConvert.DeserializeObject<Account>(jsonAccount);

            if (Request.Form["changePassword"] != string.Empty)
            {
                string password = Request.Form["password"];
                account.Password = password;

                string jsonAcount1 = JsonConvert.SerializeObject(account);
                HttpContext.Session.SetString("session", jsonAcount1);

                context.Accounts.Update(account);
                context.SaveChanges();

                return Redirect("/Profile");
            }
            if (account.Role == 1)
            {
                string etitle = Request.Form["title"];
                string eaddress = Request.Form["address"];
                string fname = Request.Form["fname"];
                string lname = Request.Form["lname"];
                string department = Request.Form["department"];
                string titlecour = Request.Form["titlecour"];
                if (eaddress == string.Empty)
                {
                    ModelState.AddModelError("Error", " address cannot empty!");
                    return Redirect("/Profile");
                }
                if (fname == string.Empty)
                {
                    ModelState.AddModelError("Error", " first name cannot empty!");
                    return Redirect("/Profile");
                }
                if (lname == string.Empty)
                {
                    ModelState.AddModelError("Error", " Last Name cannot empty!");
                    return Redirect("/Profile");
                }
                if (department == string.Empty)
                {
                    ModelState.AddModelError("Error", " department Name cannot empty!");
                    return Redirect("/Profile");
                }
                Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeId == account.EmployeeId);
                employee.Address = eaddress; employee.FirstName = fname; employee.LastName = lname;
                employee.Title = etitle;
                employee.TitleOfCourtesy = titlecour;
                context.Employees.Update(employee);

                Department d = context.Departments.First(x => x.DepartmentId == account.Employee.DepartmentId);
                d.DepartmentName = department;

                context.Departments.Update(d);
                context.SaveChanges();

                account.Employee = employee;
                account.Employee.Department = d;
                account.Employee.Department.Employees = null;
                account.Employee.Accounts = null;

                string json = JsonConvert.SerializeObject(account);

                HttpContext.Session.SetString("session", json);

                return Redirect("/Profile");

            }
            string contact = Request.Form["contact"];
            string title = Request.Form["title"];
            string company = Request.Form["company"];
            string id = Request.Form["userId"];
            string address = Request.Form["address"];
            if (title == string.Empty)
            {
                ModelState.AddModelError("Error", " contact title cannot empty!");
                return Redirect("/Profile");
            }
            if (company == string.Empty)
            {
                ModelState.AddModelError("Error", " company cannot empty!");
                return Redirect("/Profile");
            }
            if (address == string.Empty)
            {
                ModelState.AddModelError("Error", " address cannot empty!");
                return Redirect("/Profile");
            }
            if (contact == string.Empty)
            {

                ModelState.AddModelError("Error", " contact Name cannot empty!");
                return Redirect("/Profile");
            }

            Customer c = context.Customers.FirstOrDefault(u => u.CustomerId.Equals(id));
            c.Address = address;
            c.ContactName = contact;
            c.CompanyName = company;
            c.ContactTitle = title;

            context.Customers.Update(c);
            context.SaveChanges();

            account.Customer = context.Customers.FirstOrDefault(u => u.CustomerId.Equals(id));
            account.Customer.Accounts = null;

            string jsonAcount = JsonConvert.SerializeObject(account);

            HttpContext.Session.SetString("session", jsonAcount);

            return Redirect("/Profile");
        }
    }
}
