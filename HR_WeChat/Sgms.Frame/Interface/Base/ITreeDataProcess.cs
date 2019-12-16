using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeDataProcess<T> where T : class,ITree
    {
        /// <summary>
        /// 获取整棵树
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetTree();

        /// <summary>
        /// 根据条件获取树
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetTree(Func<T, bool> predicate);

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenTree(string nodeID);

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="filterID">过滤ID</param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenTree(string nodeID, string filterID);

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenTree(string nodeID, Func<T, bool> predicate);

        /// <summary>
        /// 获取深度(0开始计算)
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        int GetDepth(T node);

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        T GetParent(T node);

        /// <summary>
        /// 获取所有父级节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<T> GetParents(T node);

        /// <summary>
        /// 获取某个节点了的子节点（集合）
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenNodes(string nodeID);

        /// <summary>
        /// 获取子孙节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<T> GetDescendantsNodes(T node);

        /// <summary>
        /// 获取包含顶级的树
        /// </summary>
        /// <param name="filterID"></param>
        /// <param name="topName"></param>
        /// <returns></returns>
        IEnumerable<T> GetTreeWithTop(string topName, string filterID = null);
    }
}
