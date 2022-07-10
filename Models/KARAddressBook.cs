using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonToolkit.Models
{
    public static class KARAddressBook
    {
        // --- All addresses are offsets from 0x80000000 ---


        public const uint P1_PLAYER_DATA_POINTER = 0x0055AA2C;
        public const uint P1_VEHICLE_DATA_POINTER = 0x0055AA30;

        // Offset from plater vehicle pointer
        public const uint PLAYER_VEHICLE_MAX_PULL_UP_ANGLE = 0x638; // Resets on hit?


        // Offset from player data pointer
        /* Default Value: 1
         * Size: 32 bits 0xYYYYYYYY */
        public const uint PLAYER_SIZE = 0x328;
        public const uint PLAYER_STARTING_STAR = 0x70; // Broken?
        public const uint PLAYER_LEFT_FOOT_ATTACH_HEIGHT = 0x00F8E358;
        public const uint PLAYER_RIGHT_FOOT_ATTACH_HEIGHT = 0x00F8E4F8;
        public const float PLAYER_FOOT_ATTACH_MIN = 0.5f;
        public const float PLAYER_FOOT_ATTACH_MAX = 3.0f;

        public const uint GLOBAL_ITEM_PCT = 0x00addda4;
        // Offsets from RTOC
        public const uint CT_DATA_POINTER = 0x610;
        
    }
}
