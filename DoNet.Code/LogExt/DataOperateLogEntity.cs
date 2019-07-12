

using System;
namespace DoNet.Code
{
    /// <summary>
    /// DataOperateLogEntity
    /// </summary>	
    public class DataOperateLogEntity
    {
        public string F_Id { get; set; }
        public string F_OperateType { get; set; }
        public string F_DataId { get; set; } 
        public string F_Account { get; set; }
        public string F_NickName { get; set; }
        public string F_IPAddress { get; set; }
        public string F_IPAddressName { get; set; }
        public string AssemblyName { get; set; }
        public string ModelName { get; set; }
        public string TableName { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public DateTime F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public long F_RowIndex { get; set; }
    }

    public class DataOperateLogContrastEntity
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
