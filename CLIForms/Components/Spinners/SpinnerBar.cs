using System;
using System.Linq;
using System.Timers;
using CLIForms.Buffer;
using CLIForms.Components.Containers;

namespace CLIForms.Components.Spinners
{
    public class SpinnerBar : DisplayObject
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.Black;
        public ConsoleColor ForegroundColor = ConsoleColor.White;

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

        private int _width = 20;
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    Dirty = true;
                }
            }
        }

        private bool _autoReverse = false;
        public bool AutoReverse
        {
            get { return _autoReverse; }
            set
            {
                if (_autoReverse != value)
                {
                    _autoReverse = value;

                    if (!_autoReverse)
                        LtoRDirection = true;

                    Dirty = true;
                }
            }
        }

        private bool LtoRDirection = true;
        private int offset = 0;

        public SpinnerBar(Container parent) : base(parent)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 100;
            timer.Enabled = true;
        }

        private string pattern = "░░▒▒▓▓██";

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            

            if (LtoRDirection)
            {
                offset++;
                if (offset > _width)
                {
                    if (AutoReverse)
                    {
                        LtoRDirection = false;
                    }
                    else
                    {
                        offset = 0 - pattern.Length;
                    }
                }
                
            }
            else
            {
                offset--;
                if (offset < 0 - pattern.Length)
                {
                    if (AutoReverse)
                    {
                        LtoRDirection = true;
                    }
                }
            }

            

            Dirty = true;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(Width, 1);

            baseBuffer.DrawString(this, LtoRDirection ? pattern : new string(pattern.Reverse().ToArray()), false,
                offset, 0, BackgroundColor, ForegroundColor);

            _dirty = false;

            return baseBuffer;
        }
    }
}
