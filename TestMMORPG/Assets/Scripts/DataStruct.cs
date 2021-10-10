using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class DataStruct : MonoBehaviour
    {
        public enum HeaderByte
        {
            CheckAlive,
            ButtonPressed,
            SendPosition,
            LoginCheckFlag,
            SignUpDuplicationCheck,
            CreateAccount,
            GameStart
        }

        public enum PressedButton
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public static Dictionary<KeyCode, PressedButton> buttonDic = new Dictionary<KeyCode, PressedButton>
        {
            {KeyCode.UpArrow, PressedButton.UP},
            {KeyCode.DownArrow, PressedButton.DOWN},
            {KeyCode.LeftArrow, PressedButton.LEFT},
            {KeyCode.RightArrow, PressedButton.RIGHT}
        };

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionStruct
        {
            [MarshalAs(UnmanagedType.I4)]
            public int X;

            [MarshalAs(UnmanagedType.I4)]
            public int Y;
        }

        public static byte[] RawSerialize(PositionStruct positionStruct)
        {
            int MAX_SIZE = Marshal.SizeOf(typeof(PositionStruct));
            IntPtr buffer = Marshal.AllocHGlobal(MAX_SIZE);
            Marshal.StructureToPtr(positionStruct, buffer, false);
            byte[] RawData = new byte[MAX_SIZE];
            Marshal.Copy(buffer, RawData, 0, MAX_SIZE);
            Marshal.FreeHGlobal(buffer);
            return RawData;
        }

        public static PositionStruct RawDeSerialize(byte[] buff, ref int offset)
        {
            int MAX_SIZE = Marshal.SizeOf(typeof(PositionStruct));
            IntPtr buffer = Marshal.AllocHGlobal(MAX_SIZE);
            Marshal.Copy(buff, offset, buffer, MAX_SIZE);
            PositionStruct positionStruct = (PositionStruct)Marshal.PtrToStructure(buffer, typeof(PositionStruct));
            Marshal.FreeHGlobal(buffer);
            offset += MAX_SIZE;
            return positionStruct;
        }

        public static void WriteInt(byte[] buff, ref int offset, int k)
        {
            byte[] tempbuf = BitConverter.GetBytes(k);
            Buffer.BlockCopy(tempbuf, 0, buff, offset, tempbuf.Length);
            offset += 4;
        }

        public static void WriteString(byte[] buff, ref int offset, string str)
        {
            byte[] tempbuf = Encoding.Unicode.GetBytes(str);
            Buffer.BlockCopy(tempbuf, 0, buff, offset, tempbuf.Length);
            offset += tempbuf.Length;
        }

        public static int ReadInt(byte[] buff, ref int index)
        {
            int ret = BitConverter.ToInt32(buff, index);
            index += 4;
            return ret;
        }

        public static string ReadString(byte[] buff, ref int index, int length)
        {
            byte[] tempbuff = new byte[length];
            Buffer.BlockCopy(buff, index, tempbuff, 0, length);
            index += length;
            return Encoding.Unicode.GetString(tempbuff);
        }
    }
}
