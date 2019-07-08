
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{
    public class DotNetZip
    {
        /// <summary>
        /// 获取压缩后的流
        /// </summary>
        /// <param name="filesPath">要压缩的路径信息，Key为新文件名，Value为原路径</param>
        /// <returns></returns>
        public static Stream GetCompressStream(Dictionary<string, string> filesPath)
        {
            using (ZipFile zip = new ZipFile(System.Text.Encoding.Default)) //System.Text.Encoding.Default设置中文附件名称乱码，不设置会出现乱码
            {
                foreach (var file in filesPath)
                {
                    zip.AddFile(file.Value, "").FileName=file.Key+".jpg";
                    //第二个参数为空，说明压缩的文件不会存在多层文件夹。比如C:\test\a\b\c.doc 压缩后解压文件会出现c.doc
                    //如果改成zip.AddFile(file);则会出现多层文件夹压缩，比如C:\test\a\b\c.doc 压缩后解压文件会出现test\a\b\c.doc
                }
                MemoryStream ms = new MemoryStream();
                zip.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }
    }
}
