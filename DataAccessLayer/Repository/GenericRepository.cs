using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        
        private readonly UnitOfWork _unitOfWork; // UnitOfWork sınıfından bir nesne

        public GenericRepository(UnitOfWork unitOfWork) // Constructor'a parametre olarak unitofwork sınıfı türünden bir parametre verildi
        {
            _unitOfWork = unitOfWork;
        }

        public void Delete(T t) // Dapper ile Silme İşlemi
        {
            var idProperty = t.GetType().GetProperty("CustomerID"); // CustomerID özellikli sütunu alır
            if (idProperty != null) // Null değilse devam eder
            {
                var idValue = idProperty.GetValue(t, null); // Özelliğin değerini alır

                string tableName = typeof(T).Name; // Tablo adını alır
                string deleteQuery = $"DELETE FROM {tableName} WHERE CustomerID = @CustomerID"; // Verilen parametreyle sütun adının eşleştiği sorguyu bulur

                _unitOfWork.Connection.Execute(deleteQuery, new { CustomerID = idValue }, _unitOfWork.Transaction); // Sorguyu çalıştırır
            }
            else
            {
                throw new InvalidOperationException("Entity doesn't have a property named 'CustomerID'"); // Değer null dönerse hata fırlatır
            }
        }

        public T GetByID(int id) // İstediğimiz id değerli sorgunun getirilmesi
        {          

            string tableName = typeof(T).Name;
            string selectQuery = $"SELECT * FROM {tableName} WHERE CustomerID = @CustomerID";

            var result = _unitOfWork.Connection.QueryFirstOrDefault<T>(selectQuery, new { CustomerID = id }, _unitOfWork.Transaction); // Bulunan T değerini result değişkenine atar

            if (result != null)
            {
                return result;
            }
            else
            {
                // Burada uygun bir işlem veya alternatif değeri dönebilirsiniz.
                throw new InvalidOperationException("Kayıt bulunamadı");
            }
        }

        public List<T> GetList()
        {

            string tableName = typeof(T).Name;
            string selectQuery = $"SELECT * FROM {tableName}";

            return _unitOfWork.Connection.Query<T>(selectQuery, transaction: _unitOfWork.Transaction).ToList(); // Burada direk olarak tablodaki değerleri listeler
        }

        public void Insert(T t)
        {

            string tableName = typeof(T).Name;
            string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name)); // Özellikleri(sütun ayırır)
            string values = string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name)); // Değişkenleri belirler

            string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})"; // Sütunlar ile uyan değişkenleri eşleştirir

            _unitOfWork.Connection.Execute(insertQuery, t, _unitOfWork.Transaction);
        }


        public void Update(T t)
        {

            string tableName = typeof(T).Name;
            string updateQuery = $"UPDATE {tableName} SET ";

            var properties = typeof(T).GetProperties().Where(p => !p.Name.Equals("CustomerID")).ToList();

            foreach (var prop in properties)
            {
                updateQuery += $"{prop.Name} = @{prop.Name}, "; // Tüm özellikler üzerinde döner ve sütun parametre eşleştirmesi yapar
            }

            updateQuery = updateQuery.TrimEnd(',', ' ');
            updateQuery += " WHERE CustomerID = @CustomerID";

            _unitOfWork.Connection.Execute(updateQuery, t, _unitOfWork.Transaction); // Sorguyu çalıştırır


        }
    }
}
