using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Interface;
using Sgms.Frame.Entities;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Sgms.Frame.ADO
{
    public class ADOFilter : IDataFilter
    {
        private int _TmpLastCount = 0;
        public int LastCount
        {
            get { return _TmpLastCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="filterInfo"></param>
        /// <param name="searchFields"></param>
        /// <returns></returns>
        public DataTable Filter(SqlConnection connection, string tableName, string primaryKey, DataFilterInfo filterInfo, IEnumerable<string> searchFields = null)
        {
            return Filter(connection, tableName, primaryKey, filterInfo.SearchStr, searchFields, filterInfo.AdvancedSearchJson, filterInfo.SortField, filterInfo.IsDesc, filterInfo.PageSize, filterInfo.CurPage, filterInfo.AdvancedUseOrSearch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="searchStr"></param>
        /// <param name="searchFields"></param>
        /// <param name="sortField"></param>
        /// <param name="isDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public DataTable Filter(SqlConnection connection, string tableName, string primaryKey, string searchStr, IEnumerable<string> searchFields, string sortField, bool isDesc, int pageSize, int curPage)
        {
            return Paging(connection, tableName, primaryKey, Search(searchFields, searchStr), OrderBy(sortField, isDesc, primaryKey), pageSize, curPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="advancedSearchJson"></param>
        /// <param name="sortField"></param>
        /// <param name="isDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="advancedUseOrSearch"></param>
        /// <returns></returns>
        public DataTable Filter(SqlConnection connection, string tableName, string primaryKey, string advancedSearchJson, string sortField, bool isDesc, int pageSize, int curPage, bool advancedUseOrSearch = false)
        {
            return Paging(connection, tableName, primaryKey, AdvancedSearch(advancedSearchJson, advancedUseOrSearch), OrderBy(sortField, isDesc, primaryKey), pageSize, curPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="searchStr"></param>
        /// <param name="searchFields"></param>
        /// <param name="advancedSearchJson"></param>
        /// <param name="sortField"></param>
        /// <param name="isDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="advancedUseOrSearch"></param>
        /// <returns></returns>
        public DataTable Filter(SqlConnection connection, string tableName, string primaryKey, string searchStr, IEnumerable<string> searchFields, string advancedSearchJson, string sortField, bool isDesc, int pageSize, int curPage, bool advancedUseOrSearch = false)
        {
            DataTable result = null;
            if (!String.IsNullOrEmpty(searchStr))
            {
                result = Filter(connection, tableName, primaryKey, searchStr, sortField, isDesc, pageSize, curPage);
            }
            else if (!String.IsNullOrEmpty(advancedSearchJson))
            {
                result = Filter(connection, tableName, primaryKey, advancedSearchJson, sortField, isDesc, pageSize, curPage, advancedUseOrSearch);
            }
            else
            {
                _TmpLastCount = result.Rows.Count;
                result = Paging(connection, tableName, primaryKey, String.Empty, OrderBy(sortField, isDesc, primaryKey), pageSize, curPage);
            }
            return result;
        }

        /// <summary>
        /// 普通查询
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        public string Search(IEnumerable<string> fields, string searchStr)
        {
            List<string> result = new List<string>();
            foreach (var elem in fields)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[").Append(elem).Append("] LIKE ").Append("'%").Append(searchStr).Append("%'");
                result.Add(sb.ToString());
            }
            return "WHERE " + String.Join(" OR ", result);
        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="advancedSearchJson"></param>
        /// <param name="useOrSearch"></param>
        /// <returns></returns>
        public string AdvancedSearch(string advancedSearchJson, bool useOrSearch = false)
        {
            List<string> result = new List<string>();
            JArray jArray = (JArray)JsonConvert.DeserializeObject(advancedSearchJson);
            foreach (var elem in jArray)
            {
                var field = elem["Field"].Value<string>();
                StringBuilder sb = new StringBuilder();
                if (elem["IsRange"] != null && elem["IsRange"].Value<bool>())//范围搜索
                {
                    string minValue = elem["MinValue"].Value<string>(), maxValue = elem["MaxValue"].Value<string>();
                    sb.Append("[").Append(field).Append("] BETWEEN '").Append(minValue).Append("' AND '").Append(maxValue).Append("'");
                }
                else
                {
                    string searchStr = elem["SearchStr"].Value<string>();
                    sb.Append("[").Append(field).Append("] LIKE ").Append("'%").Append(searchStr).Append("%'");
                    result.Add(sb.ToString());
                }
            }
            return "WHERE " + String.Join(useOrSearch ? " OR " : " AND ", result);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="field"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        public string OrderBy(string field, bool isDesc, string primaryKey)
        {
            if (String.IsNullOrWhiteSpace(field))
            {
                return primaryKey;
            }
            if (field.Contains(","))
            {
                return field;
            }
            if (isDesc)
            {
                return field + " DESC";
            }
            return field;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="whereSql"></param>
        /// <param name="orderSql"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public DataTable Paging(SqlConnection connection, string tableName, string primaryKey, string whereSql, string orderSql, int pageSize, int curPage)
        {
            StringBuilder countSb = new StringBuilder();
            countSb.Append("SELECT COUNT(").Append(primaryKey).Append(") FROM [").Append(tableName).Append("] WHERE ").Append(whereSql);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (SqlCommand cmd = new SqlCommand(countSb.ToString(), connection))
            {
                _TmpLastCount = (int)cmd.ExecuteScalar();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM (SELECT * ROW_NUMBER() OVER (ORDER BY ").Append(orderSql).Append(") AS RowNumber FROM [").Append(tableName).Append("] WHERE ").Append(whereSql).Append(") Tmp WHERE RowNumber>").Append(pageSize * (curPage - 1)).Append(" AND ").Append("RowNumber<=").Append(pageSize * curPage);
            DataTable result = new DataTable();
            using (SqlDataAdapter sda = new SqlDataAdapter(sb.ToString(), connection))
            {
                sda.Fill(result);
            }
            return result;
        }
    }
}
