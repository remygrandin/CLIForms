using System;
using System.Timers;
using System.Xml.Serialization;

namespace CLIForms.Widgets
{
    public class HorizontalWaiter : Widget
    {
        internal HorizontalWaiter()
        {
            timer = new Timer(100);
            timer.Elapsed += timer_Elapsed;
            Width = 10;
        }

        public HorizontalWaiter(Widget parent)
            : base(parent) 
        {
            Background = Parent.Background;
            Foreground = ConsoleColor.White;
            timer = new Timer(100);
            timer.Elapsed += timer_Elapsed;
            Width = 10;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Draw();
        }

        private int CyclePos = 0;
        private string Chars = @"░░▒▒▓▓██";
        private string FullLine;
        private Timer timer;

        private int _width;
        public override int Width { get {
                return _width;
            }
            set {
                _width = value;

                FullLine = new string(' ', value) + Chars + new string(' ', value);
            }
        }

        internal override void Render()
        {
            CyclePos--;

            if (CyclePos < 0)
                CyclePos = FullLine.Length;
            
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, FullLine.Substring(CyclePos, _width));
        }

        private bool _spinning = false;
        [XmlAttribute]
        public bool Spinning
        {
            get
            {
                return _spinning;
            }
            set
            {
                _spinning = value;
                if (value) { timer.Start(); } else { timer.Stop(); }
            }
        }
    }
}
