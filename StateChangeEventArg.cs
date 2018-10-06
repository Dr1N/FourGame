using System;

namespace FourGame
{
    public class StateChangeEventArg : EventArgs
    {
        public int Moves { get; set; }

        public string CurrentColor { get; set; }

        public string Winner { get; set; }
    }
}
