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
        private readonly IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private bool _disposed = false;

        public UnitOfWork(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
            _dbConnection.Open();
            _dbTransaction = _dbConnection.BeginTransaction();
        }

        public IDbConnection Connection => _dbConnection;
        public IDbTransaction Transaction => _dbTransaction;

        public void Commit()
        {
            _dbTransaction?.Commit();
        }

        public void Rollback()
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

                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
    