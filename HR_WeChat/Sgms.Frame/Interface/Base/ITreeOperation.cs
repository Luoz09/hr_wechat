using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Interface
{
    /// <summary>
    /// 树形数据访问
    /// </summary>
    public interface ITreeOperation
    {
        /// <summary>
        /// 获取整棵树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData">数据源</param>
        /// <returns></returns>
        IEnumerable<T> GetTree<T>(IEnumerable<T> srcData) where T : class,ITree;

        /// <summary>
        /// 根据条件获取树
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetTree<T>(IEnumerable<T> srcData, Func<T, bool> predicate) where T : class,ITree;

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="nodeID">节点ID</param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenTree<T>(IEnumerable<T> srcData, string nodeID) where T : class,ITree;

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="filterID">过滤ID</param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenTree<T>(IEnumerable<T> srcData, string nodeID, string filterID) where T : class,ITree;

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="nodeID"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenTree<T>(IEnumerable<T> srcData, string nodeID, Func<T, bool> predicate) where T : class,ITree;

        /// <summary>
        /// 获取深度(0开始计算)
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="node">节点</param>
        /// <returns></returns>
        int GetDepth<T>(IEnumerable<T> srcData, T node) where T : class,ITree;

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="node">节点</param>
        /// <returns></returns>
        T GetParent<T>(IEnumerable<T> srcData, T node) where T : class,ITree;

        /// <summary>
        /// 获取所有父级节点
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<T> GetParents<T>(IEnumerable<T> srcData, T node) where T : class,ITree;

        /// <summary>
        /// 获取某个节点了的子节点（集合）
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        IEnumerable<T> GetChildrenNodes<T>(IEnumerable<T> srcData, string nodeID) where T : class,ITree;

        /// <summary>
        /// 获取子孙节点
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<T> GetDescendantsNodes<T>(IEnumerable<T> srcData, T node) where T : class,ITree;

        /// <summary>
        /// 获取包含顶级的树
        /// </summary>
        /// <param name="srcData">数据源</param>
        /// <param name="filterID"></param>
        /// <param name="topName"></param>
        /// <returns></returns>
        IEnumerable<T> GetTreeWithTop<T>(IEnumerable<T> srcData, string topName, string filterID = null) where T : class,ITree;
    }
}
