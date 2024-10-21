using System;
using System.Collections.Generic;
using Helper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.Game
{
    internal class Memory
    {
        // General game stuff
        public GameVersion Version { get; }

        // Important addresses and offsets
        public IntPtr BaseEngineAddress { get; }
        public IntPtr BaseFocusAddress { get; }

        public int Offset_Application { get; }
        public int Offset_GameMode { get; }
        public int Offset_GameModeExtension { get; }

        public Memory(ProcessMemory process)
        {
            var MainModule = process.MainModule;
            var is64Bit = process.Is64Bit;

            Version = GameVersion.Unknown;
            BaseEngineAddress = IntPtr.Zero;

            Offset_Application = 0x80;
            Offset_GameMode = 0x78;
            Offset_GameModeExtension = 0xB0;
        }

        /// <summary>
        /// Recovers the RTTI name
        /// </summary>
        public bool RTTILookup(ProcessMemory process, IntPtr instanceAddress, out string value)
        {
            if (instanceAddress == IntPtr.Zero)
            {
                value = string.Empty;
                return false;
            }

            if (!process.ReadPointer(instanceAddress - 0x8, out IntPtr addr))
            {
                value = string.Empty;
                return false;
            }

            addr += 0xC;

            if (!process.Read<int>(addr, out int val))
            {
                value = string.Empty;
                return false;
            }

            return process.ReadString(process.MainModule.BaseAddress + val + 0x10 + 0x4, 128, out value);
        }
    }
}
