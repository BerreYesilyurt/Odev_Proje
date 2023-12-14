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
            return View(); // Ekranın görünntülemek için View komutu kullanılır
        }



        [HttpGet]
        public IActionResult AddCustomer() {  // Müşteri ekleme tuşuna bastığımızda bizi karşılayan ekranın metodudur(get)

            return View();

        }

        [HttpPost]
        public IActionResult AddCustomer(Customer p) // Ekleme işlemi yapmak için aksiyon gösterdiğimizde aşağıdaki kodlar çalışarak ekleme işlemi yapılır(post)
        {
            customerManager.TAdd(p);
            var values = JsonConvert.SerializeObject(p); // Veriler eklenirken Serialize işlemi uygulanır ve JSON formatına çevirilir
            return Json(values); // JSON türünde döndürülür

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


        public IActionResult DeleteCustomer(int id) // Seçilen müşteriy silmeye yarar
        {
            var values = customerManager.TGetByID(id); // Önce ID'sini bulup sonra silme işlemi yapar.
            customerManager.TDelete(values);
            return RedirectToAction("Index");

        }

        public IActionResult ActivateCustomer(int id) // Müşterinin aktif ya da pasif olma durumunu gösterir
        {
            var customer = customerManager.TGetByID(id);

            if (customer != null)
            {
                // Müşteri bulunursa, aktif ise pasif, pasif ise aktif yapılır
                customer.Status = !customer.Status;

                // Güncellenen müşteriyi veritabanına kaydet
                customerManager.TUpdate(customer);
            }

            return RedirectToAction("Index");

        
        }

        public IActionResult ActiveCustomers() // Aktif müşterileri gösterir
        {
            //var activeCustomers = customerManager.TGetList().Where(c => c.Status == true).ToList();
            //var values = JsonConvert.SerializeObject(activeCustomers);
            //return Json(values);
            var activeCustomers = customerManager.TGetList().Where(c => c.Status == true).ToList();
            return PartialView("_ActiveCustomersPartial", activeCustomers); //  Bir Partial oluşturuyoruz ve verilerimizi veriyoruz
        }

        public IActionResult NotActiveCustomers() // Pasif müşterileri gösterir
        {
            //var notactiveCustomers = customerManager.TGetList().Where(c => c.Status == false).ToList();
            //var values = JsonConvert.SerializeObject(notactiveCustomers);
            //return Json(values);
            var notactiveCustomers = customerManager.TGetList().Where(c => c.Status == false).ToList();
            return PartialView("_NotActiveCustomersPartial", notactiveCustomers);  //  Bir Partial oluşturuyoruz ve verilerimizi veriyoruzs
        }




    }
}
