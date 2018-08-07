using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace WindowsFormsApplication1
{
    public static class Extensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            using (stream)
            {
                byte[] destination = new byte[stream.Length];
                stream.Position = 0;
                while (stream.ReadByte() != -1)
                {
                    stream.Position--;
                    destination[stream.Position] = (byte)(stream.ReadByte());
                }
                return destination;
            }
        }
    }
}
