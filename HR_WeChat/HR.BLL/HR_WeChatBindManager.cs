using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class HR_WeChatBindManager : OperationManager<HR_WeChatBind>
    {
        private HR_WeChatBindService _TmpCurService;
        private HR_WeChatBindService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as HR_WeChatBindService;
                }
                return _TmpCurService;
            }
        }
         
     
        /// <summary>
        /// 根据openid获取Customer
        /// </summary>
        /// <returns></returns>
        public USERS GetUserByWechatOpenID(String openID)
        {
            var cWechat = new HR_WeChatBindService().GetData(m => m.OpenID== openID && m.UnBindDate == null); 

            if (cWechat.Count() == 0)
            {
                return null;
            }
            else
            {
                var data = cWechat.FirstOrDefault();
                var userInfo = new USERSService().GetData(m => m.GUID == data.BindID).FirstOrDefault();
                return userInfo;
                //var data = new CRMCustomerInfoManager().GetQueryable();
                //foreach (var item in cWechat)
                //{
                //    var customerData = data.Where(m => m.CustomerID == item.CustomerID).FirstOrDefault();
                //    if (customerData != null)
                //    {
                //        String customerID = item.CustomerID;
                //        return customerData;
                //    }
                //}
                //return null;
            }
        }


        public List<string> GetOpenIDsByUserID(string userID)
        {
            var data = CurService.GetData().Where(m => m.BindID == userID && m.UnBindDate == null).Select(m=>m.OpenID).ToList();
            return data;
        }

        public void BindUserWechat(string openID, string userID)
        { 
            HR_WeChatBind newWechat = new HR_WeChatBind();
            newWechat.BindDate = DateTime.Now;
            newWechat.BindID = userID;
            newWechat.OpenID = openID;
            newWechat.LastLoginDate = DateTime.Now;
            newWechat.GUID = Guid.NewGuid().ToString();
            newWechat.LoginTimes = 0;
            CurService.Insert(newWechat);

            //HR_WeChatBind cWechat = CurService.GetFirstOrDefault(m => m.WeChatOpenID == openID && m.BindID == userID && m.UnBindDate == null);
            //if (cWechat != null)
            //{
            //    cWechat.UnBindDate = DateTime.Now;
            //    CurService.Update(cWechat);
            //}
            //else
            //{
            //    HR_WeChatBind newWechat = new HR_WeChatBind();
            //    newWechat.BindDate = DateTime.Now;
            //    newWechat.BindID = userID; 
            //    newWechat.WeChatOpenID = openID;
            //    newWechat.LastLoginDate = DateTime.Now;
            //    newWechat.Guid = Guid.NewGuid().ToString();
            //    CurService.Insert(newWechat);
            //}
        }

        public void UnBindUserWechat(String openID, String userID)
        { 
            HR_WeChatBind cWechat = CurService.GetFirstOrDefault(m => m.OpenID == openID && m.BindID == userID && m.UnBindDate == null);
            if (cWechat != null)
            {
                cWechat.UnBindDate = DateTime.Now;
                CurService.Update(cWechat);
            } 
        }

        public void UpdateLoginDate(String openID,string userID)
        {
            HR_WeChatBind cWechat = CurService.GetFirstOrDefault(m => m.OpenID == openID && m.BindID==userID && m.UnBindDate == null);
            if (cWechat != null)
            {
                cWechat.LastLoginDate = DateTime.Now;
                cWechat.LoginTimes += 1 ;
                CurService.Update(cWechat);
            }
             
        }
          
         
    }
}
