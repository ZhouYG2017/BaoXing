/*******************************************************************************
 * Copyright © 2016 DoNet.Framework 版权所有
 * Author: DoNet
 * Description: DoNet快速开发平台
 * Website：http://www.DoNet.cn
*********************************************************************************/
using System;

namespace DoNet.Domain
{
    public interface IModificationAudited
    {
        string F_Id { get; set; }
        string F_LastModifyUserId { get; set; }
        DateTime? F_LastModifyTime { get; set; }
    }
}