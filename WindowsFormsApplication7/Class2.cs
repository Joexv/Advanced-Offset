using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{
    class ClassXV
    {
        public string DisplayOffset2(long Offset1, long Offset2, string FileLocation, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(FileLocation));
            br.BaseStream.Seek(Offset1, SeekOrigin.Begin);
            byte[] vIn = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(vIn, 0);
            br.BaseStream.Seek(Offset2, SeekOrigin.Begin);
            byte[] vIn2 = br.ReadBytes(8);
            int vOut2 = BitConverter.ToInt32(vIn2, 0);
            result = vOut.ToString("x8") + "//" + vOut2.ToString("x8");
            return result;
        }
        public string DisplayOffset1(long Offset1, string FileLocation, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(FileLocation));
            br.BaseStream.Seek(Offset1, SeekOrigin.Begin);
            byte[] vIn = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(vIn, 0);
            result = vOut.ToString("x8");
            return result;
        }
        public string DisplayHeader(string FileLocation, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(FileLocation));
            br.BaseStream.Seek(0xac, SeekOrigin.Begin);
            result = Encoding.UTF8.GetString(br.ReadBytes(4));
            return result;
        }
    }
}
