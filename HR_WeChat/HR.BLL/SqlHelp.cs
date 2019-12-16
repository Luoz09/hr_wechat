using Jeedaa.Framework.DataAccess;
using System; 
using System.Data; 

namespace HR.BLL
{
  public class SqlHelp
    {

        public static String RunSqlReturnScalar(String sql)
        {
            return ExecSql.RunSqlReturnScalar("Jeedaa_Base", System.Data.CommandType.Text, sql);
         
        }
         

        public static DataTable LoadDataTable(String sql)
        {
            return ExecSql.LoadDataTable("Jeedaa_Base", sql);

        }


        public static void RunSql(String sql)
        {
             ExecSql.RunSql("Jeedaa_Base", System.Data.CommandType.Text, sql);

        }
    }
}
