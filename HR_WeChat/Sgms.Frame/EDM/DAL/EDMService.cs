using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.DAL;
using System.Data.Linq;
using System.Data.Objects;
using System.Reflection;
using Sgms.Frame.EDM.DAL;
using Sgms.Frame.Sys;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using Sgms.Frame.Interface.DAL;
using System.Data.Entity;
using System.Web;
using System.Data;

namespace Sgms.Frame.EDM.DAL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EDMService<T> : IService<T>, IDisposable where T : class, IEntity
    {
        [ThreadStatic]
        internal static DbContext tmpDBContext;
        #region 过滤信息

        private SysMessage _TmpRunMessage = new SysMessage();
        /// <summary>
        /// 执行中的系统消息
        /// </summary>
        public SysMessage RunMessage
        {
            get { return _TmpRunMessage; }
        }

        public DataFilterInfo FilterInfo
        {
            get;
            set;
        }

        private int _TmpLastCount;
        public virtual int LastCount
        {
            get { return _TmpLastCount; }
        }

        #endregion 过滤信息

        #region 数据访问信息

        /// <summary>
        /// DataContext 类型的名称
        /// </summary>
        private static Type _TmpDbContextType;
        private Type DbContextType
        {
            get
            {
                if (_TmpDbContextType == null)
                {
                    _TmpDbContextType = typeof(T).Assembly.GetTypes().FirstOrDefault(m => typeof(DbContext).IsAssignableFrom(m.BaseType));
                }
                return _TmpDbContextType;
            }
        }
        private volatile DbContext _TmpObjectContext;

        /// <summary>
        /// 事务单件模式
        /// </summary>
        /// <returns></returns>
        public JDDbTransaction CreateTransaction()
        {
            JDDbTransaction transaction = new JDDbTransaction();
            lock (typeof(DbContext))
            {




                if (DbContext.Database.CurrentTransaction == null)
                {
                    transaction.Transaction = DbContext.Database.BeginTransaction();
                    transaction.IsNewTransaction = true;



                }
                else
                {
                    transaction.Transaction = DbContext.Database.CurrentTransaction;
                    transaction.IsNewTransaction = false;
                }
                return transaction;
            }
        }


        /// <summary>
        /// ObjectContext
        /// </summary>
        public DbContext DbContext
        {
            get
            {

                DbContext _TmpObjectContext = null;


                if (IsWebApp)
                {
                    HttpContext context = HttpContext.Current;
                  
                    _TmpObjectContext = (DbContext)context.Items[typeof(T).Assembly.FullName];

                    if (_TmpObjectContext == null)
                    {
                        _TmpObjectContext = (DbContext)typeof(T).Assembly.CreateInstance(DbContextType.FullName, true, BindingFlags.Default, null, new object[0], null, null);
                        context.Items[typeof(T).Assembly.FullName] = _TmpObjectContext;

                    }
                }

                else
                {
                    if (tmpDBContext == null)
                    {
                        _TmpObjectContext = (DbContext)typeof(T).Assembly.CreateInstance(DbContextType.FullName, true, BindingFlags.Default, null, new object[0], null, null);
                        tmpDBContext = _TmpObjectContext;

                    }
                    else
                    {
                        _TmpObjectContext = tmpDBContext;
                    }

                }


                return _TmpObjectContext;
            }
        }


        /// <summary>
		/// 判断当前应用是否是Web应用
		/// </summary>
		private static bool IsWebApp
        {
            get
            {
                bool bResult = true;

                try
                {
                    HttpContext.Current.GetType();
                }
                catch (System.Exception)
                {
                    bResult = false;
                }

                return bResult;
            }
        }

        /// <summary>
        /// Table
        /// </summary>
        private DbSet<T> _TmpDbSet;
        /// <summary>
        /// Table
        /// </summary>
        protected DbSet<T> DbSet
        {
            get
            {
                if (_TmpDbSet == null)
                {
                    _TmpDbSet = DbContext.Set<T>();
                }
                return _TmpDbSet;
            }
        }

        /// <summary>
        /// 同步数据源
        /// </summary>
        /// <param name="objectContext"></param>
        public void SynObjectContext<TEntity>(EDMService<TEntity> service) where TEntity : class, IEntity
        {
            service._TmpDbSet = null;
            if (service._TmpObjectContext != null)
            {
                service._TmpObjectContext.Dispose();
            }
            service._TmpObjectContext = DbContext;
        }

        #endregion 数据访问相关

        /// <summary>
        /// 构析函数
        /// </summary>

        /// <summary>
        /// 获取第一个
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetData(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntity(string id)
        {
            return this.DbSet.Find(id);
            /*var paramExp = Expression.Parameter(typeof(T), "m");
            var propertyInfo = GetPrimaryKey();
            var memberExp = Expression.MakeMemberAccess(paramExp, propertyInfo);
            var constantExp = Expression.Constant(id, typeof(string));
            var equalExp = Expression.Equal(memberExp, constantExp);
            return GetData().FirstOrDefault(Expression.Lambda<Func<T, bool>>(equalExp, paramExp));*/
        }

        /// <summary>
        /// 获取查询信息
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetData()
        {
            return DbSet;
        }

        /// <summary>
        /// 获取查询信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetData(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return GetData().Where(predicate);
        }

        /// <summary>
        /// 获取过滤的数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetFilteredData(Expression<Func<T, bool>> predicate)
        {
            EDMFilter filter = new EDMFilter();
            var result = filter.Filter(GetData(predicate), FilterInfo);
            _TmpLastCount = filter.LastCount;
            return result;
        }

        /// <summary>
        /// 获取过滤的数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetFilteredData()
        {
            EDMFilter filter = new EDMFilter();
            var result = filter.Filter(GetData(), FilterInfo);
            _TmpLastCount = filter.LastCount;
            return result;
        }

        /// <summary>
        /// 根据sql 语句返回datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable SqlQueryForDataTatable(string sql)
        {

            SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = DbContext.Database.Connection.ConnectionString;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);

            conn.Close();//连接需要关闭
            conn.Dispose();
            return table;
        }
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        protected PropertyInfo GetPrimaryKey()
        {
            return typeof(T).GetProperties().First(m => m.GetCustomAttributes(false).Any(m1 => m1 as EdmScalarPropertyAttribute != null && (m1 as EdmScalarPropertyAttribute).EntityKeyProperty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="dataSrc"></param>
        /// <returns></returns>
        public IQueryable<T1> Filter<T1>(IQueryable<T1> dataSrc) where T1 : class
        {

            /*var sort = FilterInfo.SortField;
            if (String.IsNullOrEmpty(sort))
            {
                var genericArgumentsType = dataSrc.GetType().GetGenericArguments()[0];
                var propertyInfo = genericArgumentsType.GetProperties().FirstOrDefault();
                if (propertyInfo != null)
                {
                    sort = propertyInfo.Name;
                }
            }
            FilterInfo.SortField = sort;*/
            EDMFilter filter = new EDMFilter();
            var result = filter.Filter(dataSrc, FilterInfo);
            _TmpLastCount = filter.LastCount;
            return result;
        }

        #region 隐式实现

        System.Collections.IEnumerable Interface.IDataAccess<T>.GetData()
        {
            return DbSet;
        }

        System.Collections.IEnumerable IService<T>.GetFilteredData()
        {
            EDMFilter filter = new EDMFilter();
            if (String.IsNullOrEmpty(FilterInfo.SortField))
            {
                FilterInfo.SortField = typeof(T).GetProperties().First().Name;
                FilterInfo.IsDesc = true;
            }
            var result = filter.Filter(GetData(), FilterInfo);
            _TmpLastCount = filter.LastCount;
            return result;
        }

        #endregion 隐式实现

        public void Dispose()
        {
            if (_TmpObjectContext != null)
            {
                _TmpObjectContext.Dispose();
                _TmpObjectContext = null;
            }
        }
    }


    public class JDDbTransaction: System.IDisposable
    {
        public DbContextTransaction Transaction;
        public bool IsNewTransaction;
       

        public void Commit()
        {
            if (IsNewTransaction)
            {
                if (Transaction != null)
                {

                    Transaction.Commit();


                }


            }
        }

        public void Rollback()
        {
            if (IsNewTransaction)
            {
                if (Transaction != null)
                {

                    Transaction.Rollback();


                }


            }



        }

        public void Dispose()
        {
            if (IsNewTransaction)
            {
                if (Transaction != null)
                {
                    Transaction.Dispose();
                }
            }
            Transaction = null;
        }
    }
}