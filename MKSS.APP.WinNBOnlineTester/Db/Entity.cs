using SqlSugar;
using System;


namespace MKSS.Core.Model.BaseEntity
{
    /// <summary>
    /// 包含指定类型主键的实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class Entity<TKey> : IEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
        public virtual TKey F_Id { get; set; }

    }

    /// <summary>
    /// 主键类型为GUID的实体
    /// </summary>
    public class Entity : Entity<Guid>
    {

    }
}
