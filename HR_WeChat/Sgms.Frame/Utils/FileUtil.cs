using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using Sgms.Frame.Sys;
using System.Drawing;

namespace Sgms.Frame.Utils
{
    /// <summary>
    /// 文件处理工具类
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// 带 "." 的扩展名 不区分大小写
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="ignoreDot">是否忽略点 如果为True 例如：传入"doc" 返回则认为是office文件</param>
        /// <returns></returns>
        public static bool IsOfficeFile(string fileName, bool ignoreDot = true)
        {
            var exName = GetExtName(fileName, ignoreDot);
            return exName == ".doc" || exName == ".docx" || exName == ".xls" || exName == ".xlsx" || exName == ".wps" || exName == ".et";
        }

        /// <summary>
        /// 获取带"."小写的扩展名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ignoreDot">是否忽略点 如果为True 例如：传入"doc" 返回则返回 ".doc" 否则返回 String.Empty</param>
        /// <returns></returns>
        public static string GetExtName(string fileName, bool ignoreDot = true)
        {
            string exName = Path.GetExtension(fileName);
            if (exName == String.Empty && ignoreDot)
            {
                exName = "." + fileName;
            }
            exName = exName.ToLower();
            return exName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        public static void CreateDirByRelativePath(string relativePath)
        {
            var absPath = HttpContext.Current.Server.MapPath(relativePath);
            DirectoryInfo dirInfo = new DirectoryInfo(absPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CreateDirByRelativePath(string relativePath, ref string msg)
        {
            try
            {
                CreateDirByRelativePath(relativePath);
            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="extName">需要带"."</param>
        /// <param name="msg"></param>
        /// <returns>保存失败返回 String.Empty</returns>
        public static string SaveFileByRelativePathWithExtName(Stream stream, string extName, ref string msg)
        {
            string fileName = Path.Combine(SysPara.UploadsPath, DateTime.Today.ToString("yyyyMM"), Guid.NewGuid().ToString() + extName);
            if (SaveFileByRelativePath(stream, fileName, ref msg))
            {
                return fileName;
            }
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="relativePath"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SaveFileByRelativePath(Stream stream, string relativePath, ref string msg)
        {
            try
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                using (FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(relativePath), FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                    return true;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 根据相对路径获取尺寸
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static int[] GetImageSizeByRelativePath(string relativePath)
        {
            int[] result = new int[2];
            try
            {
                relativePath=relativePath.ToLower();
                if (relativePath.StartsWith("http://"))
                {
                    relativePath = relativePath.Replace("http://" + HttpContext.Current.Request.Url.Authority, "");
                }
                using (Image img = Image.FromFile(HttpContext.Current.Server.MapPath(relativePath)))
                {
                    result[0] = img.Width;
                    result[1] = img.Height;
                }
            }
            catch { }
            return result;
        }
    }
}
