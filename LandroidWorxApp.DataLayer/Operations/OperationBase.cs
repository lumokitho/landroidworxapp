using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace LandroidWorxApp.DataLayer.Operations
{
    public class OperationBase
    {
        protected RepoManager RepositoryManager { get; set; }

        public OperationBase(RepoManager repositoryManager)
        {
            RepositoryManager = repositoryManager;
        }

        public SqlSugarClient GetDBInstance(string connectionNamed)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = RepositoryManager.Configuration.GetConnectionString(connectionNamed),
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true
            });

            return db;
        }

        public List<T> GetAll<T>(string brand = null) where T : class
        {
            try
            {

                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Queryable<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> GetByExpression<T>(Expression<Func<T, bool>> predicate, string brand = null, bool references = true) where T : class
        {
            try
            {

                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Queryable<T>().Where(predicate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetSingleByExpression<T>(Expression<Func<T, bool>> predicate, string brand = null, bool references = true) where T : class
        {
            try
            {

                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Queryable<T>().Single(predicate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update<T>(T element, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Updateable<T>(element).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateOnly<T>(Expression<Func<T, T>> updateFields, Expression<Func<T, bool>> where, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Updateable<T>().SetColumns(updateFields).Where(where).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateNonDefaults<T>(T updateFields, Expression<Func<T, bool>> where, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Updateable(updateFields).Where(where).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Insert<T>(T element, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Insertable(element).ExecuteReturnIdentity();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertAll<T>(List<T> elements, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Insertable(elements).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Save<T>(T element, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Saveable(element).ExecuteReturnEntity();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> SaveAll<T>(List<T> elements, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Saveable(elements).ExecuteReturnList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteByExpression<T>(Expression<Func<T, bool>> predicate, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Deleteable<T>().Where(predicate).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete<T>(T element, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Deleteable<T>(element).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteAll<T>(List<T> elements, string brand = null) where T : class, new()
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    db.Deleteable<T>(elements).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExecuteStoredProcedure(string storedProcedureName, object parameters, string brand = null, bool references = true)
        {
            try
            {
                string connString = !string.IsNullOrEmpty(brand) ? string.Format("{0}-{1}", RepositoryManager.ConnectionString, brand.ToUpper()) : RepositoryManager.ConnectionString;

                using (var db = GetDBInstance(connString))
                {
                    return db.Ado.UseStoredProcedure().GetDataTable(storedProcedureName, parameters);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
