using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HR.BLL
{
    public class MATERIALManager : OperationManager<MATERIAL>
    {
        private MATERIALService _TmpCurService;
        private MATERIALService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as MATERIALService;
                }
                return _TmpCurService;
            }
        }

        public const string UPLOAD_PATH = "/uploads";
        public string UPLOAD_TMP_PATH = "/uploads/" + DateTime.Now.ToString("yyyyMMdd") + "";

        //上传文件
        public bool Upload(Stream file, string fileName, string time)
        {
            string types = ".png, .jpg, .bmp, .gif, .jpeg, .doc, .docx, .wps, .xls, .xlsx, .et, .zip, .rar, .7z, .txt";
            if (!ValidateFileType(fileName))
            {
                RunMessage.Append("非法文件类型,只支持" + types + "的类型文件");
                return false;
            }
            //fileName = Path.GetExtension(fileName);
            fileName = fileName.Replace(fileName.Split('.')[0], time);
            string path = Path.Combine(UPLOAD_TMP_PATH, fileName);
            path = HttpContext.Current.Server.MapPath(path);
            try
            {
                var tmp = HttpContext.Current.Server.MapPath(UPLOAD_TMP_PATH);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);//不存在就创建文件夹 

                }
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[file.Length];
                    file.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                    return true;
                }
            }
            catch (Exception e)
            {
                RunMessage.Append(e.Message);
                return false;
            }
        }


        /// <summary>
        /// 验证文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool ValidateFileType(string fileName)
        {
            var extName = Path.GetExtension(fileName);
            string[] types = new string[] { ".png", ".jpg", ".bmp", ".gif", ".jpeg", ".doc", ".docx", ".wps", ".xls", ".xlsx", ".et", ".zip", ".rar", ".7z", ".txt" };
            foreach (var elem in types)
            {
                if (elem.Equals(extName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public bool DeleteFile(string fileName, string fileID)
        {
            if (!String.IsNullOrEmpty(fileID))
            {
                var entity = CurService.GetFirstOrDefault(m => m.ID == fileID);
                if (entity != null)
                {
                    fileName = entity.FILE_PATH;
                }
                Delete(entity);
            }
            else
            {
                fileName = Path.Combine(UPLOAD_TMP_PATH, fileName);
            }
            var aPath = HttpContext.Current.Server.MapPath(fileName);
            if (File.Exists(aPath))
            {
                try
                {
                    File.Delete(aPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }


        public bool SaveFiles(string uploadJson, string resourceid, string mtName)
        {
            if (String.IsNullOrEmpty(uploadJson)) return true;
            var result = true;
            uploadJson = uploadJson.Substring(0, uploadJson.Length - 1);
            JArray jArray = JsonConvert.DeserializeObject(uploadJson) as JArray;
            foreach (var elem in jArray)
            {
                var dirPath = Path.Combine(UPLOAD_PATH, DateTime.Today.ToString("yyyyMMdd"));
                DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(dirPath));
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                string targetFileFullName = Path.Combine(dirPath, elem["fileName"].Value<string>());
                var entity = new MATERIAL();
                entity.ID = Guid.NewGuid().ToString();
                entity.TITLE = elem["fileName"].Value<string>();
                entity.FILE_PATH = targetFileFullName;
                entity.RESOURCE_ID = resourceid;
                entity.SORT_ID = 0;
                //entity.PROGRAM_NAME = mtName;
                entity.LAST_MODIFY_TIME = DateTime.Now;
                if (!Insert(entity))
                {
                    return false;
                }
                string srcFileFullName = Path.Combine(UPLOAD_TMP_PATH, elem["fileName"].Value<string>());
                try
                {
                    File.Move(HttpContext.Current.Server.MapPath(srcFileFullName), HttpContext.Current.Server.MapPath(targetFileFullName));
                }
                catch (Exception e)
                {
                    var tmp = e.Message;
                    result = false;
                }
            }

            return result;
        }

        public bool SaveAndUpdateFiles(string uploadJson, string resourceid, string mtName)
        {
              
            if (String.IsNullOrEmpty(uploadJson)) return true;
            uploadJson = uploadJson.Substring(0, uploadJson.Length - 1);
            JArray jArray = JsonConvert.DeserializeObject(uploadJson) as JArray;
            List<string> ids = new List<string>();
            foreach (var elem in jArray)
            {
                var dirPath = Path.Combine(UPLOAD_PATH, DateTime.Today.ToString("yyyyMMdd"));
                DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(dirPath));
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                if (elem["id"] != null)
                {
                    ids.Add(elem["id"].ToString());
                    continue;
                }
                string targetFileFullName = Path.Combine(dirPath, elem["fileName"].Value<string>());
                var entity = new MATERIAL();
                entity.ID = Guid.NewGuid().ToString();
                entity.TITLE = elem["fileName"].Value<string>();
                entity.FILE_PATH = targetFileFullName;
                entity.RESOURCE_ID = resourceid;
                entity.SORT_ID = 0;
                //entity.PROGRAM_NAME = mtName;
                entity.LAST_MODIFY_TIME = DateTime.Now;
                CurService.InsertToCache(entity);
                string srcFileFullName = Path.Combine(UPLOAD_TMP_PATH, elem["fileName"].Value<string>());
                try
                {
                    File.Move(HttpContext.Current.Server.MapPath(srcFileFullName), HttpContext.Current.Server.MapPath(targetFileFullName));
                }
                catch (Exception e)
                {
                }
            }
            CurService.DeleteToCache(CurService.GetData(m => !ids.Contains(m.ID) && m.RESOURCE_ID == resourceid));
            try
            {
                CurService.SubmitToDb();
                return true;
            }
            catch (Exception e)
            {
                RunMessage.Append(e);
                return false;
            }
        }

        public string[] GetFiles(string resourceid)
        {
            return CurService.GetData(m => m.RESOURCE_ID == resourceid).Select(m => m.FILE_PATH).ToArray();
        }



        public List<MATERIAL> GetFilesData(string id)
        {
            return CurService.GetData().Where(m => m.RESOURCE_ID == id).ToList();
        }


    }
}
