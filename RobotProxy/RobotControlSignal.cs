using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotProxy
{
    public enum SignalType : uint
    {
        Walk = 1,
        Camera = 2,
        WalkAndCamera = 3
    };

    public class RobotControlSignal
    {
        public uint SignalFlag { get; set; }
        public double Degree { get; set; }
        public double Speed { get; set; }
        public double Distance { get; set; }
        public double Pan { get; set; }
        public double Tilt { get; set; }

        public RobotControlSignal()
        {
            SignalFlag = (uint)SignalType.WalkAndCamera;
        }
    }
}
