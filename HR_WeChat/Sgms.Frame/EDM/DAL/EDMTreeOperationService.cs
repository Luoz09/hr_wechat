using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Objects.DataClasses;
using Sgms.Frame.DAL;
using System.Data.Objects;
using System.Linq.Expressions;
using Sgms.Frame.Interface.DAL;
using Sgms.Frame.Interface;

namespace Sgms.Frame.EDM.DAL
{
    /// <summary>
    /// 树形数据操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EDMTreeOperationService<T> : EDMOperationService<T>, ITreeOperationService<T> where T : class,ITree
    {
        #region TreeOperation

        private ITreeOperation _TmpTreeOperation;
        /// <summary>
        /// 数据过滤
        /// </summary>
        protected virtual ITreeOperation TreeOperation
        {
            get
            {
                if (_TmpTreeOperation == null)
                {
                    _TmpTreeOperation = new TreeOperation();
                }
                return _TmpTreeOperation;
            }
        }

        /// <summary>
        /// 获取整棵树
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetTree()
        {
            return TreeOperation.GetTree(GetData().ToArray());
        }

        /// <summary>
        /// 根据条件获取整棵树（此条件也会应用到子孙节点）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTree(Func<T, bool> predicate)
        {
            return TreeOperation.GetTree(GetData().ToArray(), predicate);
        }

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenTree(string nodeID)
        {
            return TreeOperation.GetChildrenTree(GetData().ToArray(), nodeID);
        }

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="filterID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenTree(string nodeID, string filterID)
        {
            return TreeOperation.GetChildrenTree(GetData().ToArray(), nodeID, filterID);
        }

        /// <summary>
        /// 获取子树
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenTree(string nodeID, Func<T, bool> predicate)
        {
            return TreeOperation.GetChildrenTree(GetData().ToArray(), nodeID, predicate);
        }

        /// <summary>
        /// 获取深度(0开始计算)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual int GetDepth(T node)
        {
            return TreeOperation.GetDepth(GetData().ToArray(), node);
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public T GetParent(T node)
        {
            return TreeOperation.GetParent(GetData().ToArray(), node);
        }

        /// <summary>
        /// 获取父节点集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetParents(T node)
        {
            return TreeOperation.GetParents(GetData().ToArray(), node);
        }

        /// <summary>
        /// 获取子节点结合  不会遍历孙节点 执行效率较高
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetChildrenNodes(string nodeID)
        {
            return TreeOperation.GetChildrenNodes(GetData().ToArray(), nodeID);
        }

        /// <summary>
        /// 获取子孙节点集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetDescendantsNodes(T node)
        {
            return TreeOperation.GetDescendantsNodes(GetData().ToArray(), node);
        }

        /// <summary>
        /// 获取包含顶级节点的树
        /// </summary>
        /// <param name="topName"></param>
        /// <param name="filterID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTreeWithTop(string topName, string filterID = null)
        {
            return TreeOperation.GetTreeWithTop(GetData().ToArray(), topName, filterID);
        }

        #endregion TreeOperation
    }
}