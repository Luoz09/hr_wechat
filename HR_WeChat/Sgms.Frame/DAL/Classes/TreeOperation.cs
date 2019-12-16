using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Interface;

namespace Sgms.Frame.DAL
{
    /// <summary>
    /// 
    /// </summary>
    public class TreeOperation : ITreeOperation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTree<T>(IEnumerable<T> srcData) where T : class,ITree
        {
            var result = srcData.Where(m => m.ParentID == null || m.ParentID == String.Empty).OrderBy(m => m.Sort);
            foreach (var elem in result)
            {
                elem.Children = GetChildrenTree(srcData, elem.ID);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTree<T>(IEnumerable<T> srcData, Func<T, bool> predicate) where T : class,ITree
        {
            var result = srcData.Where(m => m.ParentID == null || m.ParentID == String.Empty).Where(predicate).OrderBy(m => m.Sort);
            foreach (var elem in result)
            {
                elem.Children = GetChildrenTree(srcData, elem.ID).Where(predicate);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenTree<T>(IEnumerable<T> srcData, string nodeID) where T : class,ITree
        {
            return GetChildrenTree(srcData, nodeID, m => true);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="nodeID"></param>
        /// <param name="filterID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenTree<T>(IEnumerable<T> srcData, string nodeID, string filterID) where T : class,ITree
        {
            return GetChildrenTree(srcData, nodeID, m => m.ID != filterID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="nodeID"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenTree<T>(IEnumerable<T> srcData, string nodeID, Func<T, bool> predicate) where T : class,ITree
        {
            var result = srcData.Where(m => m.ParentID == nodeID && m.ParentID != m.ID).Where(predicate).OrderBy(m => m.Sort);
            foreach (var elem in result)
            {
                elem.Children = GetChildrenTree(srcData, elem.ID).ToArray();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public int GetDepth<T>(IEnumerable<T> srcData, T node) where T : class,ITree
        {
            int depth = 0;
            var curNode = node;
            while (curNode != null)
            {
                curNode = srcData.FirstOrDefault(m => m.ID == curNode.ParentID);
                depth++;
            }
            return depth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public T GetParent<T>(IEnumerable<T> srcData, T node) where T : class,ITree
        {
            return srcData.FirstOrDefault(m => m.ID == node.ParentID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<T> GetParents<T>(IEnumerable<T> srcData, T node) where T : class,ITree
        {
            List<T> result = new List<T>();
            var curNode = node;
            while (true)
            {
                curNode = srcData.FirstOrDefault(m => m.ID == curNode.ParentID);
                if (curNode == null)
                    break;
                result.Add(curNode);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenNodes<T>(IEnumerable<T> srcData, string nodeID) where T : class,ITree
        {
            return srcData.Where(m => m.ParentID == nodeID).OrderBy(m => m.Sort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDescendantsNodes<T>(IEnumerable<T> srcData, T node) where T : class,ITree
        {
            List<T> result = new List<T>();
            result.Add(node);
            GetChildrenNodes(srcData, node.ID, result);
            return result;
        }

        /// <summary>
        /// 获取子孙节点集合 递归用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="parentID"></param>
        /// <param name="result"></param>
        private void GetChildrenNodes<T>(IEnumerable<T> srcData, string parentID, List<T> result) where T : class,ITree
        {
            var childrenNodes = srcData.Where(m => m.ParentID == parentID);
            result.AddRange(childrenNodes);
            foreach (var elem in childrenNodes)
            {
                GetChildrenNodes(srcData, elem.ID, result);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="topName"></param>
        /// <param name="filterID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTreeWithTop<T>(IEnumerable<T> srcData, string topName, string filterID = null) where T : class,ITree
        {
            T topNode = System.Activator.CreateInstance<T>();
            topNode.Text = topName;
            topNode.ID = String.Empty;
            if (String.IsNullOrEmpty(filterID))
            {
                topNode.Children = GetTree(srcData).ToArray();
            }
            else
            {
                topNode.Children = GetTree(srcData, m => m.ID != filterID).ToArray();
            }
            return new T[] { topNode };
        }
    }
}
