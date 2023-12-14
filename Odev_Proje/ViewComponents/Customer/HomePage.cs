using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Odev_Proje.ViewComponents.Customer
{
    public class HomePage:ViewComponent
    {
        CustomerManager customerManager=new CustomerManager(new EfCustomerDal()); // Repository'e erişebilmek için parametre olarak new EfCustomerDal veriyoruz. Buna da CustomerManager aracılığıyla ulaşıyoruz.
        public IViewComponentResult Invoke()
        {
            var values = customerManager.TGetList(); // Verilerin listelenmesi
            return View(values);  
        }


    }
}
