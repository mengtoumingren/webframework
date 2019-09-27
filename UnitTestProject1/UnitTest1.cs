using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var file = File.OpenRead(@"C:\Users\mengt\Desktop\微信截图_20190904135123.png");
            byte[] buffer = new byte[file.Length];
            
            var basestr = 



            file.Read(buffer, 0, buffer.Length);
            var str = Encoding.UTF8.GetString(buffer);
            using (var newFile = File.Open(@"C:\Users\mengt\Desktop\无标题1.png", FileMode.Create))
            {
                var encodings = Encoding.GetEncodings();
                using (var writer = new BinaryWriter(newFile))
                {
                    var data = Encoding.UTF8.GetBytes(str);
                    var newdata = Encoding.Convert(Encoding.UTF8, Encoding.Default, data);
                    writer.Write(data);
                }
            }
            
        }

       
    }
}
