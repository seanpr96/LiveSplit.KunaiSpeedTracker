using System;
using System.Diagnostics;
using System.Drawing;

namespace LiveSplit.KunaiSpeedTracker
{
    public class KunaiMemory : IDisposable
    {
        private DateTime _nextHookAttempt = DateTime.MinValue;

        private readonly ProgramPointer
            _playerSystem = new ProgramPointer("BA????????8BC0E8????????8B400C8945CC", 1, 1);

        public Process Program { get; private set; }

        public bool IsHooked => Program != null && !Program.HasExited;

        public void Hook()
        {
            if (IsHooked || DateTime.Now < _nextHookAttempt)
            {
                return;
            }

            _nextHookAttempt = DateTime.Now.AddSeconds(1);

            Process[] processes = Process.GetProcessesByName("KUNAI");
            if (processes.Length == 0)
            {
                return;
            }

            Program = processes[0];
            MemoryReader.Update64Bit(Program);
        }

        public PointF GetPlayerSpeed()
        {
            float x = _playerSystem.Read<float>(Program, 0x24, 0x4, 0x0, 0xC, 0x10, 0x30, 0x94);
            float y = _playerSystem.Read<float>(Program, 0x24, 0x4, 0x0, 0xC, 0x10, 0x30, 0x98);

            x = (float) Math.Round(x, 2);
            y = (float) Math.Round(y, 2);

            return new PointF(x, y);
        }

        public void Dispose()
        {
            Program?.Dispose();
        }
    }
}
