﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.DateTime;

namespace Masuit.Tools.Logging
{
    /// <summary>
    /// 日志组件
    /// </summary>
    public class LogManager
    {
        static readonly ConcurrentQueue<Tuple<string, string>> LogQueue = new ConcurrentQueue<Tuple<string, string>>();

        private static readonly Task WriteTask;

        static LogManager()
        {
            WriteTask = new Task(obj =>
            {
                while (true)
                {
                    Pause.WaitOne(1000, true);
                    List<string[]> temp = new List<string[]>();
                    foreach (var logItem in LogQueue)
                    {
                        string logPath = logItem.Item1;
                        string logMergeContent = String.Concat(logItem.Item2, Environment.NewLine, "----------------------------------------------------------------------------------------------------------------------", Environment.NewLine);
                        string[] logArr = temp.FirstOrDefault(d => d[0].Equals(logPath));
                        if (logArr != null)
                        {
                            logArr[1] = string.Concat(logArr[1], logMergeContent);
                        }
                        else
                        {
                            logArr = new[] { logPath, logMergeContent };
                            temp.Add(logArr);
                        }
                        LogQueue.TryDequeue(out Tuple<string, string> _);
                    }
                    foreach (var item in temp)
                    {
                        WriteText(item[0], item[1]);
                    }
                }
            }, null, TaskCreationOptions.LongRunning);
            WriteTask.Start();
        }

        private static AutoResetEvent Pause => new AutoResetEvent(false);

        /// <summary>
        /// 日志存放目录，默认日志放在当前应用程序运行目录下的logs文件夹中
        /// </summary>
        public static string LogDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        /// <summary>
        /// 写入Info级别的日志
        /// </summary>
        /// <param name="info"></param>
        public static void Info(string info) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(info).ToUpper()}  {info}"));

        /// <summary>
        /// 写入Info级别的日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="info"></param>
        public static void Info(string source, string info) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(info).ToUpper()}   {source}  {info}"));

        /// <summary>
        /// 写入Info级别的日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="info"></param>
        public static void Info(Type source, string info) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(info).ToUpper()}   {source.FullName}  {info}"));

        /// <summary>
        /// 写入debug级别日志
        /// </summary>
        /// <param name="debug">异常对象</param>
        public static void Debug(string debug) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(debug).ToUpper()}   {debug}"));

        /// <summary>
        /// 写入debug级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="debug">异常对象</param>
        public static void Debug(string source, string debug) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(debug).ToUpper()}   {source}  {debug}"));

        /// <summary>
        /// 写入debug级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="debug">异常对象</param>
        public static void Debug(Type source, string debug) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(debug).ToUpper()}   {source.FullName}  {debug}"));

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="error">异常对象</param>
        public static void Error(Exception error) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {error.Source}  {error.Message}{Environment.NewLine}{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {error.Source}  {error.StackTrace}"));

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常对象</param>
        public static void Error(Type source, Exception error) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {source.FullName}  {error.Message}{Environment.NewLine}{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {source.FullName}  {error.StackTrace}"));

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常信息</param>
        public static void Error(Type source, string error) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {source.FullName}  {error}"));

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常对象</param>
        public static void Error(string source, Exception error) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {source}  {error.Message}{Environment.NewLine}{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {source}  {error.StackTrace}"));

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常信息</param>
        public static void Error(string source, string error) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(error).ToUpper()}   {source}  {error}"));

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Exception fatal) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {fatal.Source}  {fatal.Message}{Environment.NewLine}{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {fatal.Source}  {fatal.StackTrace}"));

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Type source, Exception fatal) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {source.FullName}  {fatal.Message}{Environment.NewLine}{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {source.FullName}  {fatal.StackTrace}"));

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Type source, string fatal) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {source.FullName}  {fatal}"));

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(string source, Exception fatal) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {source}  {fatal.Message}{Environment.NewLine}{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {source}  {fatal.StackTrace}"));

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(string source, string fatal) => LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), $"{Now}   [{Thread.CurrentThread.ManagedThreadId}]   {nameof(fatal).ToUpper()}   {source}  {fatal}"));

        private static string GetLogPath()
        {
            string newFilePath;
            String logDir = string.IsNullOrEmpty(LogDirectory) ? Path.Combine(Environment.CurrentDirectory, "logs") : LogDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            string extension = ".log";
            string fileNameNotExt = String.Concat(Now.ToString("yyyyMMdd"));
            string fileNamePattern = string.Concat(fileNameNotExt, "(*)", extension);
            List<string> filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly).ToList();

            if (filePaths.Count > 0)
            {
                int fileMaxLen = filePaths.Max(d => d.Length);
                string lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).FirstOrDefault();
                if (new FileInfo(lastFilePath).Length > 1 * 1024 * 1024)
                {
                    string no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(lastFilePath)).Value;
                    bool parse = int.TryParse(no, out int tempno);
                    string formatno = $"({(parse ? (tempno + 1) : tempno)})";
                    string newFileName = String.Concat(fileNameNotExt, formatno, extension);
                    newFilePath = Path.Combine(logDir, newFileName);
                }
                else
                {
                    newFilePath = lastFilePath;
                }
            }
            else
            {
                string newFileName = string.Concat(fileNameNotExt, $"({0})", extension);
                newFilePath = Path.Combine(logDir, newFileName);
            }
            return newFilePath;
        }

        private static void WriteText(string logPath, string logContent)
        {
            try
            {
                if (!File.Exists(logPath))
                {
                    File.CreateText(logPath).Close();
                }
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.Write(logContent);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}