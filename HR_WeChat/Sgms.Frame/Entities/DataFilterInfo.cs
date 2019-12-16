using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 过滤信息
    /// </summary>
    public class DataFilterInfo
    {
        /// <summary>
        /// 每页多少条
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurPage { get; set; }

        /// <summary>
        /// 是否逆序
        /// </summary>
        public bool IsDesc { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 查询的字段
        /// </summary>
        public string SearchStr { get; set; }

        /// <summary>
        /// 高级查询用的JSON
        /// </summary>
        public string AdvancedSearchJson { get; set; }

        /// <summary>
        /// 高级查询使用或查询
        /// </summary>
        public bool AdvancedUseOrSearch { get; set; }
    }
}
