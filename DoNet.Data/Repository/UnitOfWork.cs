using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{
    public class UnitOfWork
    {
        public static void BeginTransaction(IsolationLevel iolationLevel = IsolationLevel.Unspecified)
        {
            DoNetDbContext dbContext = DoNetDbContextFactory.GetUnitOfWorkDbContext();
            DbContextTransaction transaction = dbContext.Database.CurrentTransaction;
            if (transaction == null)
            {
                dbContext.Database.BeginTransaction(iolationLevel);
            }
        }

        public static void CommitTransaction()
        {
            DbContextTransaction transaction = DoNetDbContextFactory.GetUnitOfWorkDbContext().Database.CurrentTransaction;
            if (transaction != null)
            {
                try
                {
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static void RollbackTransaction()
        {
            DbContextTransaction transaction = DoNetDbContextFactory.GetUnitOfWorkDbContext().Database.CurrentTransaction;
            if (transaction != null)
            {
                transaction.Rollback();
            }
        }

        public static void DisposeTransaction()
        {
            DbContextTransaction transaction = DoNetDbContextFactory.GetUnitOfWorkDbContext().Database.CurrentTransaction;
            if (transaction != null)
            {
                transaction.Dispose();
            }
            DoNetDbContextFactory.RemoveUnitOfWorkDbContext();
        }
    }
}
