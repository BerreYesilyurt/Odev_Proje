using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Odev_Proje.Controllers
{
    public class CustomerController : Controller
    {

        CustomerManager customerManager = new CustomerManager(new EfCustomerDal());
        Context c = new Context();

        public IActionResult Index() // Müşterilerin listelendiği ana sayfa metodudur
        {
            var values = customerManager.TGetList();    
            return View(values);
        }

        [HttpGet]
        public IActionResult AddCustomer() // Müşteri ekleme tuşuna bastığımızda bizi karşılayan ekranın metodudur
        {

            return View();

        }

        [HttpPost]
        public IActionResult AddCustomer(Customer p) // Ekleme işlemi yapmak için aksiyon gösterdiğimizde aşağıdaki kodlar çalışarak ekleme işlemi yapılır
        {
            customerManager.TAdd(p);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult UpdateCustomer(int id) // Müşteri ekleme tuşuna bastığımızda bizi karşılayan ekranın metodudur
        {
            var values=customerManager.TGetByID(id);
            return View(values);

        }

        [HttpPost]
        public IActionResult UpdateCustomer(Customer p) // Ekleme işlemi yapmak için aksiyon gösterdiğimizde aşağıdaki kodlar çalışarak ekleme işlemi yapılır
        {
            customerManager.TUpdate(p); 
            return RedirectToAction("Index");

        }

        public IActionResult ActivateCustomer(int id)
        {
            Customer customer = new Customer();

            

            if (customer.Status == true)
            {
                customer.Status = false;    
            }

            else
            {
                customer.Status=true;   
            }

            return RedirectToAction("Index");

        
        }




    }
}
