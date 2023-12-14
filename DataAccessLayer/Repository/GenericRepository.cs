using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        public void Delete(T t)
        {
            using var c = new Context(); // using ile işlem bittikten hemen sonra bellekten atılır
            c.Remove(t); // t nesnesini kaldırır(EF Core metodu ile)
            c.SaveChanges(); // Veri tabanına yansıması için değişikliklerin kaydedilmesi gereklidir.
        }

        public T GetByID(int id)
        {
            using var c = new Context();
            return c.Set<T>().Find(id);// Verilen ID değerine göre Class döner
        }

        public List<T> GetList()
        {
            using var c = new Context();
            return c.Set<T>().ToList(); // Verileri listeler
        }

        public void Insert(T t)
        {
            using var c = new Context();
            c.Add(t); // Veriyi ekler
            c.SaveChanges();  // Veri tabanına yansıması için değişikliklerin kaydedilmesi gereklidir.  
        }

        public void Update(T t)
        {
            using var c = new Context();
            c.Update(t); // Veriyi günceller
            c.SaveChanges();  // Veri tabanına yansıması için değişikliklerin kaydedilmesi gereklidir.  
        }
    }
}
