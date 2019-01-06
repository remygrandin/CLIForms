using System;
using System.Timers;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;

namespace CLIForms.Components.Spinners
{
    public class Spinner : DisplayObject
    {
        public ConsoleColor? BackgroundColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        private string _states = @"    ░▒▓█";
        public string States
        {
            get => _states;
            set
            {
                if (_states != value)
                {
                    _states = value;

                    if (statePos >= _states.Length)
                        statePos = 0;

                    Dirty = true;
                }
            }
        }

        private Timer timer = new Timer();

        public double SpeedInterval
        {
            get { return timer.Interval; }
            set
            {
                if (timer.Interval != value)
                {
                    timer.Interval = value;
                }
            }
        }

        public bool Spinning
        {
            get => timer.Enabled;
            set
            {
                if (timer.Enabled != value)
                {
                    timer.Enabled = value;

                    Dirty = true;
                }
            }
        }

        public Spinner(Container parent) : base(parent)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 100;
            timer.Enabled = true;
        }

        private int statePos = 0;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            statePos++;

            if (statePos >= _states.Length)
                statePos = 0;

            Dirty = true;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(3, 3);

            string drawStr = _states.Substring(statePos) + _states.Substring(0, statePos);

            baseBuffer.data[0, 0] = new ConsoleChar(this, drawStr[7], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[1, 0] = new ConsoleChar(this, drawStr[6], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[2, 0] = new ConsoleChar(this, drawStr[5], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[2, 1] = new ConsoleChar(this, drawStr[4], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[2, 2] = new ConsoleChar(this, drawStr[3], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[1, 2] = new ConsoleChar(this, drawStr[2], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[0, 2] = new ConsoleChar(this, drawStr[1], false, BackgroundColor, ForegroundColor);
            baseBuffer.data[0, 1] = new ConsoleChar(this, drawStr[0], false, BackgroundColor, ForegroundColor);

            _dirty = false;

            return baseBuffer;
        }
    }
}
