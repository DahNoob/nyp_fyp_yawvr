using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace YawVR
{
    /*Communication between the game and the simulator:
        (Commands can be created easily by calling static functions on Commands class)

        UDP messages:
        Every udp command is ascii encoded string sent as byte array

        TCP messages:
        Every tcp command begins with the byte identifier of the given command, 
        followed by the command parameters.
        Integer and float parameters are converted into 4 bytes (sent in big endian format), 
        string parameters are converted into byte array with ASCII encoding.
        (CommandIds class contains the tcp command ids)
    */

        //MARK: - Command ID-s
        /// <summary>
        /// TCP CommandIDs
        /// </summary>
        public static class CommandIds
    {
        public const byte CHECK_IN = 0x30;
        public const byte START = 0xA1;
        public const byte STOP = 0xA2;
        public const byte EXIT = 0xA3;
        public const byte RESET_PORTS = 0x01;
        public const byte SET_SIMU_INPUT_PORT = 0x10;
        public const byte SET_GAME_INPUT_PORT = 0x11;
        public const byte SET_GAME_IP_ADDRESS = 0xA4;
        public const byte SET_OUTPUT_PORT = 0x12;
        public const byte SET_POWER = 0x30;
        public const byte CHECK_IN_ANS = 0x31;
        public const byte ERROR = 0xA5;



        //UDP CMDS
        public const byte UDP_LED_CMD = 0xB2;

    };


    public static class Commands
    {
        //MARK: - CALLS FROM GAME TO SIMULATOR
        //UDP
        public static byte[] DEVICE_DISCOVERY = Encoding.ASCII.GetBytes("YAW_CALLING");
        private static ushort udpLedCounter = 0;

        static void FromShort(ushort number, out byte byte1, out byte byte2) {
            byte2 = (byte)(number >> 8);
            byte1 = (byte)(number & 255);
        }
        public static byte[] UDP_LED_CMD(Color32[] colors) {
            var bytes = new byte[390];
            bytes[0] = CommandIds.UDP_LED_CMD;
            FromShort(udpLedCounter, out bytes[1], out bytes[2]);
               
            for (int i = 3; i < bytes.Length; i += 3) {
                bytes[i] = colors[(i - 3) / 3].g;
                bytes[i + 1] = colors[(i - 3) / 3].r;
                bytes[i + 2] = colors[(i - 3) / 3].b;
            }

            udpLedCounter++;

            return bytes;
            
          
        }
        public static byte[] UDP_LED_CMD(Color32 color) {
            var bytes = new byte[390];
            bytes[0] = CommandIds.UDP_LED_CMD;
            FromShort(udpLedCounter, out bytes[1], out bytes[2]);

            for (int i = 3; i < bytes.Length; i += 3) {
                bytes[i] = color.g;
                bytes[i + 1] = color.r;
                bytes[i + 2] = color.b;
            }
            udpLedCounter++;
            return bytes;


        }


        //example: "Y[000.00]P[359.99]R[180.00]"; - there is no 360.00, just 000.00
        public static byte[] MOTION_DATA(float yaw, float pitch, float roll,Buzzer buzzer)
        {
            var orientationFormat = string.Format("Y[{0}]P[{1}]R[{2}]", FormatRotation(yaw), FormatRotation(pitch), FormatRotation(roll));
            var buzzerFormat = buzzer.isOn && buzzer.hz > 0 ? string.Format("V[{0},{1},{2},{3}]",buzzer.right_amp,buzzer.center_amp,buzzer.left_amp,buzzer.hz) : "V[0,0,0,0]";


            var message = orientationFormat + buzzerFormat;

        //    Debug.Log(message);
            return Encoding.ASCII.GetBytes(message);
        }

        //TCP

        public static byte[] CHECK_IN(int udpListeningPort, string gameName)
        {
            List<byte> message = new List<byte>();

            message.AddRange(IntToByteArray(udpListeningPort));
            message.AddRange(Encoding.ASCII.GetBytes(gameName));
            return AddByteToArray(message.ToArray(), CommandIds.CHECK_IN);
        }

        public static byte[] START = { CommandIds.START };

        public static byte STOP = CommandIds.STOP;

        public static byte[] EXIT = { CommandIds.EXIT };

     

        //MARK: - Helper functions
        private static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        private static string FormatRotation(float f)
        {
            return string.Format("{0}", f.ToString("000.000",CultureInfo.InvariantCulture));
        }

        private static byte[] IntToByteArray(int intValue)
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        private static byte[] FloatToByteArray(float floatValue)
        {
            byte[] floatBytes = BitConverter.GetBytes(floatValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatBytes);
            return floatBytes;
        }

        public static int ByteArrayToInt(byte[] intBytes, int startIndex)
        {
            byte[] intArray = new byte[4] { intBytes[startIndex], intBytes[startIndex + 1], intBytes[startIndex + 2], intBytes[startIndex + 3] };
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intArray);
            }

            int integer = BitConverter.ToInt32(intArray, 0);
            return integer;
        }

        public static float ByteArrayToFloat(byte[] floatBytes, int startIndex)
        {
            byte[] floatArray = new byte[4] { floatBytes[startIndex], floatBytes[startIndex + 1], floatBytes[startIndex + 2], floatBytes[startIndex + 3] };
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatArray);
            }
            float floatNumber = BitConverter.ToSingle(floatArray, 0);
            return floatNumber;
        }
    }
}

