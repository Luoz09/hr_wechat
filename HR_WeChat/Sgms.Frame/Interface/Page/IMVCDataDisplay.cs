using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections;

namespace Sgms.Frame.Interface.UI
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMVCDataDisplay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        ContentResult DisplayJson(string json);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ContentResult DisplayJson(IEnumerable<object> data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ContentResult DisplayJson(object obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        ContentResult DisplayJson(bool isSuccess, string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        ContentResult DisplayText(string text);

        /// <summary>
        /// 
        /// </summary>
        void DisplayNoRightsPage();

        /// <summary>
        /// 
        /// </summary>
        ContentResult DisplayNoRightsJson();

        /// <summary>
        /// 需要带有总数的数据 会根据data.Count计算
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ContentResult DisplayDataOfTotal(IEnumerable<object> data);

        /// <summary>
        /// 需要带有总数的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        ContentResult DisplayDataOfTotal(IEnumerable data,int count);
    }
}
