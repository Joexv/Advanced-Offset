﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Reflection;

namespace WindowsFormsApplication1
{
    class LZ77Handler
    {
        private int _lastStructSize;

        public int LastStructSize
        {
            get { return _lastStructSize; }
        }

        public Bitmap LoadSprite(int spriteLocation, int paletteLocation, byte[] rom, bool showBackgroundColours)
        {
            byte[] spriteData;
            byte[] paletteData;
            Decompress(rom, spriteLocation, out spriteData);
            Decompress(rom, paletteLocation, out paletteData);
            return ConstructSprite(spriteData, ConstructPalette(paletteData, showBackgroundColours));
        }

        public Color[] LoadPalette(int paletteLocation, byte[] rom, bool showBackgroundColours)
        {
            byte[] palette;
            Decompress(rom, paletteLocation, out palette);
            return ConstructPalette(palette, showBackgroundColours);
        }

        public void WritePalette(int writeLocation, byte[] rom, Color[] palette)
        {
            byte[] paletteData = ConvertColoursToByteArray(palette);
            Compress(paletteData.Length, paletteData, out paletteData);
            WriteLoop(rom, paletteData, writeLocation);
        }

        public void WriteSprite(int writeLocation, byte[] rom, Bitmap sprite, Color[] palette)
        {
            byte[] spriteData = ConvertBitmapTo4BPPByteArray(sprite, palette);
            Compress(spriteData.Length, spriteData, out spriteData);
            WriteLoop(rom, spriteData, writeLocation);
        }

        private byte[] ConvertBitmapTo4BPPByteArray(Bitmap sprite, Color[] palette)
        {
            byte[] toReturn = new byte[(sprite.Height * sprite.Width) >> 1];
            int index = 0;
            for (int i = 0; i < sprite.Height; i++)
            {
                for (int j = 0; j < sprite.Width / 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Color temp = sprite.GetPixel((j * 2) + k, i);
                        byte outValue = 0;
                        byte index2 = 0;
                        foreach (Color c in palette)
                        {
                            if (temp == c)
                            {
                                outValue = (byte)(index2 << (k * 4));
                                break;
                            }
                            index2++;
                        }
                        toReturn[index] = (byte)(toReturn[index] | outValue);
                    }
                    index++;
                }
            }
            return toReturn;
        }

        private byte[] ConvertColoursToByteArray(Color[] palette)
        {
            byte[] toReturn = new byte[palette.Length << 1];
            int index = 0;
            foreach (Color c in palette)
            {
                int r = (c.R >> 3);
                int g = (c.G << 2);
                int b = (c.B << 7);
                WriteLoop(index, 2, (uint)(b | g | r), toReturn);
                index += 2;
            }
            return toReturn;
        }

        private void WriteLoop(byte[] rom, byte[] data, int baseLocation)
        {
            for (int i = 0; i < data.Length; i++)
            {
                rom[baseLocation + i] = data[i];
            }
        }

        private void WriteLoop(int baselocation, int length, uint value, byte[] rom)
        {
            for (uint i = 0; i < length; i++)
            {
                rom[baselocation + i] = Convert.ToByte(value.ToString("X8").Substring(6 - (2 * (int)i), 2), 16);
            }
        }

        private Bitmap ConstructSprite(byte[] bits, Color[] palette)
        {
            try
            {
                Bitmap bmpTiles = new Bitmap(64, 64);
                int i = 0;
                for (int y1 = 0; y1 <= 56; y1 += 8)
                {
                    for (int x1 = 0; x1 <= 56; x1 += 8)
                    {
                        for (int y2 = 0; y2 <= 7; y2++)
                        {
                            for (int x2 = 0; x2 <= 7; x2 += 2)
                            {
                                byte Temp = bits[i];
                                bmpTiles.SetPixel(x1 + x2 + 1, y1 + y2, palette[(Temp & 0xF0) >> 4]);
                                bmpTiles.SetPixel(x1 + x2, y1 + y2, palette[Temp & 0xF]);
                                i++;
                            }
                        }
                    }
                }
                return bmpTiles;
            }
            catch
            {
                return new Bitmap(64, 64);
            }
        }

        private Color[] ConstructPalette(byte[] palette, bool showBackgroundColours)
        {
            Color[] paletteColours = new Color[16];
            int startPoint = 1;
            if (showBackgroundColours)
            {
                startPoint = 0;
            }
            else
            {
                paletteColours[0] = SystemColors.Control;
            }
            for (int i = startPoint; i < 16; i++)
            {
                ushort tempValue = (ushort)(palette[i * 2] + (palette[i * 2 + 1] << 8));
                ushort r = (ushort)((tempValue & 0x1F) << 3);
                ushort g = (ushort)((tempValue & 0x3E0) >> 2);
                ushort b = (ushort)((tempValue & 0x7C00) >> 7);
                Color c = Color.FromArgb(0xFF, r, g, b);
                paletteColours[i] = c;
            }
            return paletteColours;
        }

        private void Decompress(byte[] source, int baseLocation, out byte[] destination)
        {
            Stream temp = null;
            int retValue = LZ77Decompress(new MemoryStream(source), out temp, baseLocation);
            destination = temp.ToByteArray();
        }

        private void Compress(int decompressedSize, byte[] source, out byte[] destination)
        {
            Stream ms = null;
            int retValue = LZ77Compress(decompressedSize, new MemoryStream(source), out ms);
            destination = ms.ToByteArray();
        }

        private int LZ77Decompress(Stream instream, out Stream outstream, int baseLocation)
        {
            int inLength = (int)instream.Length;
            outstream = new MemoryStream();
            long readBytes = 0;
            instream.Position = baseLocation;
            byte type = (byte)instream.ReadByte();
            if (type != 0x10)
                throw new InvalidDataException("The provided stream is not a valid LZ-0x10 "
                            + "compressed stream (invalid type 0x" + type.ToString("X") + ")");
            byte[] sizeBytes = new byte[3];
            instream.Read(sizeBytes, 0, 3);
            int decompressedSize = IOUtils.ToNDSu24(sizeBytes, 0);
            readBytes += 4;
            if (decompressedSize == 0)
            {
                sizeBytes = new byte[4];
                instream.Read(sizeBytes, 0, 4);
                decompressedSize = IOUtils.ToNDSs32(sizeBytes, 0);
                readBytes += 4;
            }
            int bufferLength = 0x1000;
            byte[] buffer = new byte[bufferLength];
            int bufferOffset = 0;
            int currentOutSize = 0;
            int flags = 0, mask = 1;
            while (currentOutSize < decompressedSize)
            {
                if (mask == 1)
                {
                    if (readBytes >= inLength)
                    {
                        throw new Exception();
                    }
                    flags = instream.ReadByte(); readBytes++;
                    if (flags < 0)
                    {
                        throw new Exception();
                    }
                    mask = 0x80;
                }
                else
                {
                    mask >>= 1;
                }
                if ((flags & mask) > 0)
                {
                    if (readBytes + 1 >= inLength)
                    {
                        if (readBytes < inLength)
                        {
                            instream.ReadByte(); readBytes++;
                        }
                        throw new Exception();
                    }
                    int byte1 = instream.ReadByte(); readBytes++;
                    int byte2 = instream.ReadByte(); readBytes++;
                    if (byte2 < 0)
                    {
                        throw new Exception();
                    }
                    int length = byte1 >> 4;
                    length += 3;
                    int disp = ((byte1 & 0x0F) << 8) | byte2;
                    disp += 1;
                    if (disp > currentOutSize)
                    {
                        throw new InvalidDataException("Cannot go back more than already written. "
                                + "DISP = 0x" + disp.ToString("X") + ", #written bytes = 0x" + currentOutSize.ToString("X")
                                + " at 0x" + (instream.Position - 2).ToString("X"));
                    }
                    int bufIdx = bufferOffset + bufferLength - disp;
                    for (int i = 0; i < length; i++)
                    {
                        byte next = buffer[bufIdx % bufferLength];
                        bufIdx++;
                        outstream.WriteByte(next);
                        buffer[bufferOffset] = next;
                        bufferOffset = (bufferOffset + 1) % bufferLength;
                    }
                    currentOutSize += length;
                }
                else
                {
                    if (readBytes >= inLength)
                    {
                        throw new Exception();
                    }
                    int next = instream.ReadByte(); readBytes++;
                    if (next < 0)
                    {
                        throw new Exception();
                    }
                    currentOutSize++;
                    outstream.WriteByte((byte)next);
                    buffer[bufferOffset] = (byte)next;
                    bufferOffset = (bufferOffset + 1) % bufferLength;
                }
                outstream.Flush();
            }
            this._lastStructSize = (int)instream.Position - baseLocation;
            return decompressedSize;
        }

        private unsafe int LZ77Compress(int inLength, Stream instream, out Stream outstream)
        {
            outstream = new MemoryStream();
            if (inLength > 0xFFFFFF)
            {
                throw new Exception();
            }
            byte[] indata = new byte[inLength];
            int numReadBytes = instream.Read(indata, 0, (int)inLength);
            if (numReadBytes != inLength)
            {
                throw new Exception();
            }
            outstream.WriteByte(0x10);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));
            int compressedLength = 4;
            fixed (byte* instart = &indata[0])
            {
                byte[] outbuffer = new byte[8 * 2 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;
                while (readBytes < inLength)
                {
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }
                    int disp;
                    int oldLength = Math.Min(readBytes, 0x1000);
                    int length = LZUtil.GetOccurrenceLength(instart + readBytes, (int)Math.Min(inLength - readBytes, 0x12),
                                                          instart + readBytes - oldLength, oldLength, out disp);
                    if (length < 3)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        readBytes += length;
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));
                        outbuffer[bufferlength] = (byte)(((length - 3) << 4) & 0xF0);
                        outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
                        bufferlength++;
                    }
                    bufferedBlocks++;
                }
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                    while ((compressedLength % 4) != 0)
                    {
                        outstream.WriteByte(0);
                        compressedLength++;
                    }
                }
            }
            this._lastStructSize = inLength;
            return compressedLength;
        }
    }

    public static class LZUtil
    {
        /// <summary>
        /// Determine the maximum size of a LZ-compressed block starting at newPtr, using the already compressed data
        /// starting at oldPtr. Takes O(inLength * oldLength) = O(n^2) time.
        /// </summary>
        /// <param name="newPtr">The start of the data that needs to be compressed.</param>
        /// <param name="newLength">The number of bytes that still need to be compressed.
        /// (or: the maximum number of bytes that _may_ be compressed into one block)</param>
        /// <param name="oldPtr">The start of the raw file.</param>
        /// <param name="oldLength">The number of bytes already compressed.</param>
        /// <param name="disp">The offset of the start of the longest block to refer to.</param>
        /// <param name="minDisp">The minimum allowed value for 'disp'.</param>
        /// <returns>The length of the longest sequence of bytes that can be copied from the already decompressed data.</returns>
        public static unsafe int GetOccurrenceLength(byte* newPtr, int newLength, byte* oldPtr, int oldLength, out int disp, int minDisp = 1)
        {
            disp = 0;
            if (newLength == 0)
                return 0;
            int maxLength = 0;
            // try every possible 'disp' value (disp = oldLength - i)
            for (int i = 0; i < oldLength - minDisp; i++)
            {
                // work from the start of the old data to the end, to mimic the original implementation's behaviour
                // (and going from start to end or from end to start does not influence the compression ratio anyway)
                byte* currentOldStart = oldPtr + i;
                int currentLength = 0;
                // determine the length we can copy if we go back (oldLength - i) bytes
                // always check the next 'newLength' bytes, and not just the available 'old' bytes,
                // as the copied data can also originate from what we're currently trying to compress.
                for (int j = 0; j < newLength; j++)
                {
                    // stop when the bytes are no longer the same
                    if (*(currentOldStart + j) != *(newPtr + j))
                        break;
                    currentLength++;
                }

                // update the optimal value
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                    disp = oldLength - i;

                    // if we cannot do better anyway, stop trying.
                    if (maxLength == newLength)
                        break;
                }
            }
            return maxLength;
        }
    }

    public static class IOUtils
    {

        #region byte[] <-> (u)int
        /// <summary>
        /// Returns a 4-byte unsigned integer as used on the NDS converted from four bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The source of the data.</param>
        /// <param name="offset">The location of the data in the source.</param>
        /// <returns>The indicated 4 bytes converted to uint</returns>
        public static uint ToNDSu32(byte[] buffer, int offset)
        {
            return (uint)(buffer[offset]
                        | (buffer[offset + 1] << 8)
                        | (buffer[offset + 2] << 16)
                        | (buffer[offset + 3] << 24));
        }

        /// <summary>
        /// Returns a 4-byte signed integer as used on the NDS converted from four bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The source of the data.</param>
        /// <param name="offset">The location of the data in the source.</param>
        /// <returns>The indicated 4 bytes converted to int</returns>
        public static int ToNDSs32(byte[] buffer, int offset)
        {
            return (int)(buffer[offset]
                        | (buffer[offset + 1] << 8)
                        | (buffer[offset + 2] << 16)
                        | (buffer[offset + 3] << 24));
        }

        /// <summary>
        /// Converts a u32 value into a sequence of bytes that would make ToNDSu32 return
        /// the given input value.
        /// </summary>
        public static byte[] FromNDSu32(uint value)
        {
            return new byte[] {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF)
            };
        }

        /// <summary>
        /// Returns a 3-byte integer as used in the built-in compression
        /// formats in the DS, convrted from three bytes at a specified position in a byte array,
        /// </summary>
        /// <param name="buffer">The source of the data.</param>
        /// <param name="offset">The location of the data in the source.</param>
        /// <returns>The indicated 3 bytes converted to an integer.</returns>
        public static int ToNDSu24(byte[] buffer, int offset)
        {
            return (int)(buffer[offset]
                        | (buffer[offset + 1] << 8)
                        | (buffer[offset + 2] << 16));
        }
        #endregion

        #region Plugin loading
        /// <summary>
        /// (Attempts to) load compression formats from the given file.
        /// </summary>
        /// <param name="file">The dll file to load.</param>
        /// <param name="printFailures">If formats without an empty contrsuctor should get a print.</param>
        /// <returns>A list with an instance of all compression formats found in the given dll file.</returns>
        /// <exception cref="FileNotFoundException">If the given file does not exist.</exception>
        /// <exception cref="FileLoadException">If the file could not be loaded.</exception>
        /// <exception cref="BadImageFormatException">If the file is not a valid assembly, or the loaded
        /// assembly is compiled with a higher version of .NET.</exception>
        internal static IEnumerable<CompressionFormat> LoadCompressionPlugin(string file, bool printFailures = false)
        {
            if (file == null)
                throw new FileNotFoundException("A null-path cannot be loaded.");
            List<CompressionFormat> newFormats = new List<CompressionFormat>();

            string fullPath = Path.GetFullPath(file);

            Assembly dll = Assembly.LoadFile(fullPath);
            foreach (Type dllType in dll.GetTypes())
            {
                if (dllType.IsSubclassOf(typeof(CompressionFormat))
                    && !dllType.IsAbstract)
                {
                    try
                    {
                        newFormats.Add(Activator.CreateInstance(dllType) as CompressionFormat);
                    }
                    catch (MissingMethodException)
                    {
                        if (printFailures)
                            Console.WriteLine(dllType + " is a compression format, but does not have a parameterless constructor. Format cannot be loaded from " + fullPath + ".");
                    }
                }
            }

            return newFormats;
        }

        /// <summary>
        /// Loads all compression formats found in the given folder.
        /// </summary>
        /// <param name="folder">The folder to load plugins from.</param>
        /// <returns>A list with an instance of all compression formats found in the given folder.</returns>
        internal static IEnumerable<CompressionFormat> LoadCompressionPlugins(string folder)
        {
            List<CompressionFormat> formats = new List<CompressionFormat>();

            foreach (string file in Directory.GetFiles(folder))
            {
                try
                {
                    formats.AddRange(LoadCompressionPlugin(file, false));
                }
                catch (Exception) { }
            }

            return formats;
        }
        #endregion

        /// <summary>
        /// Gets the full path to the parent directory of the given path.
        /// </summary>
        /// <param name="path">The path to get the parent directory path of.</param>
        /// <returns>The full path to the parent directory of teh given path.</returns>
        public static string GetParent(string path)
        {
            return Directory.GetParent(path).FullName;
        }
    }
}
