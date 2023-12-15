using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly IDbConnection _dbConnection; // Veri tabanı ile bağlantı içindir
        private IDbTransaction _dbTransaction; // İşlemlerin gerçekleştirilmesi için
        private bool _disposed = false; // İlk başta verilerin serbest bırakma işlemi olamdığı için "falseé'dur

        public UnitOfWork(string connectionString) // Veri tabanı bağlantı adresini constructor üzerinde alıyoruz
        {
            _dbConnection = new SqlConnection(connectionString); // Bağlantı oluşturuyoruz
            _dbConnection.Open(); // Bağlantı açılır
            _dbTransaction = _dbConnection.BeginTransaction(); // İşlemler başlar
        }

        public IDbConnection Connection => _dbConnection;
        public IDbTransaction Transaction => _dbTransaction;

        public void Commit() // İşlem başarılı olursa çalışır ve verileri toplu olarak kalıcı hale getirir
        {
            _dbTransaction?.Commit();
        }

        public void Rollback() // Eğer işlem başarılı olmazsa işlemi geri alır
        {
            _dbTransaction?.Rollback();
        }

        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbTransaction?.Dispose();
                    _dbConnection?.Dispose();
                }

                _disposed = true; // Başlangıçta false olan değer, serbest bırakılmayla birlikte true'ya döner
            }
        }

        ~UnitOfWork() 
        {
            Dispose(false);
        }
    }
}
    