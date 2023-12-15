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
        // Dapper direkt veri tabanından işlemleri çektiği için veri tabanına bağlantı kurmalıyız
        //private readonly string _connectionString= "server=(localdb)\\MSSQLLocalDB;database=Biletleme;integrated security=true";

        //public GenericRepository(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        private readonly UnitOfWork _unitOfWork;

        public GenericRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Delete(T t) // Dapper ile Silme İşlemi
        {
            //using var c = new Context(); // using ile işlem bittikten hemen sonra bellekten atılır
            //c.Remove(t); // t nesnesini kaldırır(EF Core metodu ile)
            //c.SaveChanges(); // Veri tabanına yansıması için değişikliklerin kaydedilmesi gereklidir.

            //using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            //{ // Bağlantı adresinden veri tabanına bağlantı sağladık

            //    var idProperty = t.GetType().GetProperty("CustomerID");
            //    if (idProperty != null) // Verilen ID değeri varsa
            //    {
            //        var idValue = idProperty.GetValue(t, null);

            //        string tableName = typeof(T).Name; // Tablonun adı alınır
            //        string deleteQuery = $"DELETE FROM {tableName} WHERE CustomerID = @CustomerID"; // Tablo adı üzerinden uygun id değeri için arama yapılır

            //        dbConnection.Execute(deleteQuery, new { CustomerID = idValue });
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException("Entity doesn't have a property named 'CustomerID'");
            //    }
            //}

            var idProperty = t.GetType().GetProperty("CustomerID");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(t, null);

                string tableName = typeof(T).Name;
                string deleteQuery = $"DELETE FROM {tableName} WHERE CustomerID = @CustomerID";

                _unitOfWork.Connection.Execute(deleteQuery, new { CustomerID = idValue }, _unitOfWork.Transaction);
            }
            else
            {
                throw new InvalidOperationException("Entity doesn't have a property named 'CustomerID'");
            }
        }

        public T GetByID(int id) // İstediğimiz id değerinin tablosunun getirilmesi
        {
            //using var c = new Context();
            //return c.Set<T>().Find(id);// Verilen ID değerine göre Class döner

            //using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            //{
            //    string tableName = typeof(T).Name;
            //    string selectQuery = $"SELECT * FROM {tableName} WHERE CustomerID = @CustomerID";

            //    var result=dbConnection.QueryFirstOrDefault<T>(selectQuery, new { CustomerID = id }); // Id değerine uyan sorgu döndürülür

            //    if (result != null)
            //    {
            //        return result;
            //    }
            //    else
            //    {
            //        // Burada uygun bir işlem veya alternatif değeri dönebilirsiniz.
            //        throw new InvalidOperationException("Kayıt bulunamadı");
            //    }

            //}

            string tableName = typeof(T).Name;
            string selectQuery = $"SELECT * FROM {tableName} WHERE CustomerID = @CustomerID";

            var result = _unitOfWork.Connection.QueryFirstOrDefault<T>(selectQuery, new { CustomerID = id }, _unitOfWork.Transaction);

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
            //using var c = new Context();
            //return c.Set<T>().ToList(); // Verileri listeler

            //using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            //{
            //    string tableName = typeof(T).Name;
            //    string selectQuery = $"SELECT * FROM {tableName}";

            //    return dbConnection.Query<T>(selectQuery).ToList();
            //}

            string tableName = typeof(T).Name;
            string selectQuery = $"SELECT * FROM {tableName}";

            return _unitOfWork.Connection.Query<T>(selectQuery, transaction: _unitOfWork.Transaction).ToList();
        }

        public void Insert(T t)
        {
            //using var c = new Context();
            //c.Add(t); // Veriyi ekler
            //c.SaveChanges();  // Veri tabanına yansıması için değişikliklerin kaydedilmesi gereklidir.  

            //using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            //{
            //    string tableName = typeof(T).Name;
            //    string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name)); // Tablonun özellikleri(sütunlar) , ile ayrılır.
            //    string values = string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name)); // @ eklenerek özellikler parametrelere dönüştürülür

            //    string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})"; // Sütunların içerisine parametreler eklenir

            //    dbConnection.Execute(insertQuery, t); // Sorgu çalıştırılır
            //}

            string tableName = typeof(T).Name;
            string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            string values = string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name));

            string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            _unitOfWork.Connection.Execute(insertQuery, t, _unitOfWork.Transaction);
        }


        public void Update(T t)
        {
            //using var c = new Context();
            //c.Update(t); // Veriyi günceller
            //c.SaveChanges();  // Veri tabanına yansıması için değişikliklerin kaydedilmesi gereklidir.  

            //using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            //{
            //    string tableName = typeof(T).Name;
            //    string updateQuery = $"UPDATE {tableName} SET ";

            //    var properties = typeof(T).GetProperties().Where(p => !p.Name.Equals("CustomerID")).ToList(); // ID sütunu haricindeki sütunları alır. ID güncellenmez.

            //    foreach (var prop in properties)
            //    {
            //        updateQuery += $"{prop.Name} = @{prop.Name}, "; // Sütunlar ve parametreler eşleştirilir
            //    }

            //    updateQuery = updateQuery.TrimEnd(',', ' ');

            //    updateQuery += " WHERE CustomerID = @CustomerID";

            //    dbConnection.Execute(updateQuery, t); // Sorgu çalıştırılır
            //}

            string tableName = typeof(T).Name;
            string updateQuery = $"UPDATE {tableName} SET ";

            var properties = typeof(T).GetProperties().Where(p => !p.Name.Equals("CustomerID")).ToList();

            foreach (var prop in properties)
            {
                updateQuery += $"{prop.Name} = @{prop.Name}, ";
            }

            updateQuery = updateQuery.TrimEnd(',', ' ');
            updateQuery += " WHERE CustomerID = @CustomerID";

            _unitOfWork.Connection.Execute(updateQuery, t, _unitOfWork.Transaction);


        }
    }
}
