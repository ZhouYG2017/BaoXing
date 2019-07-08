/*******************************************************************************
 * Copyright © 2016 DoNet.Framework 版权所有
 * Author: DoNet
 * Description: DoNet快速开发平台
 * Website：http://www.DoNet.cn
*********************************************************************************/
using System;

namespace DoNet.Code
{
    public class OperatorModel
    {
        public string UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string CompanyId { get; set; }
        public string DepartmentId { get; set; }
        public string RoleId { get; set; }
        public string LoginIPAddress { get; set; }
        public string LoginIPAddressName { get; set; }
        public string LoginToken { get; set; }
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 系统超级管理员标志
        /// </summary>
        public bool F_IsAdministrator { get; set; }
        /// <summary>
        /// 公司管理员标志
        /// </summary>
        public bool F_IsCompanyManager { get; set; }
    }
}
