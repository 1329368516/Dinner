using System;
using System.Collections.Generic;
using System.Text;

namespace Dinner.Base.Model
{
    public abstract class BaseResultModel<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public abstract bool IsSuccess { get; }

        /// <summary>
        /// 值
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public virtual string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public virtual string ErrorMessage { get; set; }
    }
}
