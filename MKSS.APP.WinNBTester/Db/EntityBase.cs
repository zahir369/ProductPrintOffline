using System;

namespace MKSS.Core.Model.BaseEntity
{
    public class EntityBase<TKey> : Entity<TKey>
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime F_CreateDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建人编号
        /// </summary>
        public string F_CreateUserId { get; set; } 
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime F_ModifyDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改人编号
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        public string F_ModifyUserName { get; set; }
        /// <summary>
        /// 租户编号，平台的租户默认为"0",其他为16为长整型字符串
        /// </summary>
        public string F_TenantId { get; set; }
    }

    public class EntityBase : EntityBase<Guid>
    {

    }
}
