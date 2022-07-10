using DragoonToolkit.Dolphin;
using static DragoonToolkit.Models.KARAddressBook;
using static DragoonToolkit.Dolphin.MemoryUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonToolkit.Services
{
    public static class KARPlayerService
    {
        /* Must be 4 byte hex string */
        public static string ResizeP1Character(DolphinProcess Game, string HexString)
        {
            byte[] buffer = new byte[4];
            Game.ReadFromMemory(P1_PLAYER_DATA_POINTER, buffer, 4);
            uint sizeAddress = uint.Parse(Convert.ToHexString(buffer), System.Globalization.NumberStyles.HexNumber) + PLAYER_SIZE;
            return Game.WriteToMemory8(sizeAddress, ConvertHexStringToByteArray(HexString), 4);
        }

        /* Must be 4 byte array */
        public static string ResizeP1Character(DolphinProcess Game, byte[] HexAddress)
        {
            byte[] buffer = new byte[4];
            Game.ReadFromMemory(P1_PLAYER_DATA_POINTER, buffer, 4);
            uint sizeAddress = uint.Parse(Convert.ToHexString(buffer), System.Globalization.NumberStyles.HexNumber) + PLAYER_SIZE;
            return Game.WriteToMemory8(sizeAddress, HexAddress, 4);
        }

        /* Must be 4 byte hex string */
        public static string ChangeP1MaxLiftAngle(DolphinProcess Game, string NewMax)
{
            byte[] buffer = new byte[4];
            Game.ReadFromMemory(P1_VEHICLE_DATA_POINTER, buffer, 4);
            uint sizeAddress = uint.Parse(Convert.ToHexString(buffer), System.Globalization.NumberStyles.HexNumber) + PLAYER_VEHICLE_MAX_PULL_UP_ANGLE;
            return Game.WriteToMemory8(sizeAddress, ConvertHexStringToByteArray(NewMax), 4);
        }

        /* Must be 4 byte hex string */
        public static string ChangeP1Feet(DolphinProcess Game, string LHexString, string RHexString)
        {
            return Game.WriteToMemory(PLAYER_LEFT_FOOT_ATTACH_HEIGHT, ConvertHexStringToByteArray(LHexString), 4) + "|" + Game.WriteToMemory(PLAYER_RIGHT_FOOT_ATTACH_HEIGHT, ConvertHexStringToByteArray(RHexString), 4);
        }
    }
}
