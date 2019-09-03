using System.IO;
using System;
using System.Text;

namespace nsocket
{
    //处理网络字节序翻转
    //c#是小端，java是大端，c/c++是小端
    //小端字节序（Little Endian）：最高有效位存于最高内存地址，最低有效位存于最低内存处
    //大端相反
    public class BinaryHelper
    {

        //是否是与大端通信，如果是，字节序需要反向
        public static bool BeBigEndian = true;

        public static void WriteString(BinaryWriter _BinaryWriter, string field)
        {
            field.Trim();
            byte[] bytes = Encoding.UTF8.GetBytes(field);
            short len = (short)bytes.Length;
            WriteShort(_BinaryWriter, len);
            _BinaryWriter.Write(bytes);
        }

        public static void WriteShort(BinaryWriter _BinaryWriter, short field)
        {
            byte[] a = BitConverter.GetBytes(field);

            if (BeBigEndian)
            {
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[0]);
            }
            else
            {
                _BinaryWriter.Write(a[0]);
                _BinaryWriter.Write(a[1]);
            }

        }

        public static void WriteByte(BinaryWriter _BinaryWriter, byte field)
        {
            _BinaryWriter.Write(field);
        }

        public static void WriteSByte(BinaryWriter _BinaryWriter, sbyte field)
        {
            _BinaryWriter.Write(field);
        }

        public static void WriteInt(BinaryWriter _BinaryWriter, int field)
        {

            byte[] a = BitConverter.GetBytes(field);

            if (BeBigEndian)
            {
                _BinaryWriter.Write(a[3]);
                _BinaryWriter.Write(a[2]);
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[0]);
            }
            else
            {
                _BinaryWriter.Write(a[0]);
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[2]);
                _BinaryWriter.Write(a[3]);
            }


        }

        public static void WriteLong(BinaryWriter _BinaryWriter, long field)
        {
            byte[] a = BitConverter.GetBytes(field);


            if (BeBigEndian)
            {
                _BinaryWriter.Write(a[7]);
                _BinaryWriter.Write(a[6]);
                _BinaryWriter.Write(a[5]);
                _BinaryWriter.Write(a[4]);
                _BinaryWriter.Write(a[3]);
                _BinaryWriter.Write(a[2]);
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[0]);
            }
            else
            {
                _BinaryWriter.Write(a[0]);
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[2]);
                _BinaryWriter.Write(a[3]);
                _BinaryWriter.Write(a[4]);
                _BinaryWriter.Write(a[5]);
                _BinaryWriter.Write(a[6]);
                _BinaryWriter.Write(a[7]);
            }

        }

        public static void WriteBool(BinaryWriter _BinaryWriter, bool field)
        {
            _BinaryWriter.Write(field);
        }

        public static void WriteFloat(BinaryWriter _BinaryWriter, float field)
        {
            byte[] a = BitConverter.GetBytes(field);
            if (BeBigEndian)
            {
                _BinaryWriter.Write(a[3]);
                _BinaryWriter.Write(a[2]);
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[0]);
            }
            else
            {
                _BinaryWriter.Write(a[0]);
                _BinaryWriter.Write(a[1]);
                _BinaryWriter.Write(a[2]);
                _BinaryWriter.Write(a[3]);
            }

        }

        //NINFO 注意：这里proto的二进制不需要反序
        public static void WriteBytes(BinaryWriter _BinaryWriter, byte[] field)
        {
            _BinaryWriter.Write(field);
        }

        public static short ReadShort(BinaryReader _BinaryReader)
        {
            short ret = 0;
            byte[] ins = _BinaryReader.ReadBytes(2);
            UnityEngine.Debug.Log("--->ReadShort 0:"+ins[0]+" 1:"+ins[1]);
            byte[] n = new byte[2];

            if (BeBigEndian)
            {
                n[0] = ins[1];
                n[1] = ins[0];
            }
            else
            {
                n[0] = ins[0];
                n[1] = ins[1];
            }

            ret = BitConverter.ToInt16(n, 0);
            return ret;
        }

        public static byte ReadByte(BinaryReader _BinaryReader)
        {
            return _BinaryReader.ReadByte();
        }

        public static sbyte ReadSbyte(BinaryReader _BinaryReader)
        {
            return _BinaryReader.ReadSByte();
        }

        public static byte[] ReadBytes(BinaryReader _BinaryReader, int size)
        {
            return _BinaryReader.ReadBytes(size);
        }

        public static string ReadString(BinaryReader _BinaryReader)
        {
            int len = ReadShort(_BinaryReader);
            byte[] bytes = new byte[len];
            bytes = _BinaryReader.ReadBytes(len);
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        public static long ReadLong(BinaryReader _BinaryReader)
        {
            long ret = 0;
            byte[] ins = _BinaryReader.ReadBytes(8);
            byte[] n = new byte[8];

            if (BeBigEndian)
            {
                n[0] = ins[7];
                n[1] = ins[6];
                n[2] = ins[5];
                n[3] = ins[4];
                n[4] = ins[3];
                n[5] = ins[2];
                n[6] = ins[1];
                n[7] = ins[0];
            }
            else
            {
                n[0] = ins[0];
                n[1] = ins[1];
                n[2] = ins[2];
                n[3] = ins[3];
                n[4] = ins[4];
                n[5] = ins[5];
                n[6] = ins[6];
                n[7] = ins[7];
            }

            ret = BitConverter.ToInt64(n, 0);
            return ret;
        }

        public static double ReadDouble(BinaryReader _BinaryReader)
        {
            double ret = 0;
            byte[] ins = _BinaryReader.ReadBytes(8);
            byte[] n = new byte[8];

            if (BeBigEndian)
            {
                n[0] = ins[7];
                n[1] = ins[6];
                n[2] = ins[5];
                n[3] = ins[4];
                n[4] = ins[3];
                n[5] = ins[2];
                n[6] = ins[1];
                n[7] = ins[0];
            }
            else
            {
                n[0] = ins[0];
                n[1] = ins[1];
                n[2] = ins[2];
                n[3] = ins[3];
                n[4] = ins[4];
                n[5] = ins[5];
                n[6] = ins[6];
                n[7] = ins[7];
            }

            ret = BitConverter.ToDouble(n, 0);
            return ret;
        }

        public static bool ReadBool(BinaryReader _BinaryReader)
        {
            return _BinaryReader.ReadBoolean();
        }

        public static int ReadInt(BinaryReader _BinaryReader)
        {
            int ret = 0;
            byte[] ins = _BinaryReader.ReadBytes(4);
            byte[] n = new byte[4];

            if (BeBigEndian)
            {

                n[0] = ins[3];
                n[1] = ins[2];
                n[2] = ins[1];
                n[3] = ins[0];
            }
            else
            {
                n[0] = ins[0];
                n[1] = ins[1];
                n[2] = ins[2];
                n[3] = ins[3];
            }

            ret = BitConverter.ToInt32(n, 0);
            return ret;
        }

        public static float ReadFloat(BinaryReader _BinaryReader)
        {
            float ret = 0.0f;
            byte[] ins = _BinaryReader.ReadBytes(4);
            byte[] n = new byte[4];
            if (BeBigEndian)
            {

                n[0] = ins[3];
                n[1] = ins[2];
                n[2] = ins[1];
                n[3] = ins[0];
            }
            else
            {
                n[0] = ins[0];
                n[1] = ins[1];
                n[2] = ins[2];
                n[3] = ins[3];
            }
            ret = BitConverter.ToSingle(n, 0);
            return ret;
        }


    }
}