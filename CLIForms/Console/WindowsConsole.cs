using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CLIForms.Buffer;
using CLIForms.Engine.Events;
using CLIForms.Extentions;
using Microsoft.Win32.SafeHandles;

namespace CLIForms.Console
{
    public class WindowsConsole : IConsole
    {

        public int Width
        {
            get { return System.Console.WindowWidth; }
            set
            {
                System.Console.WindowWidth = value;

                ReApplySizing();
            }
        }

        public int Height
        {
            get { return System.Console.WindowHeight; }
            set
            {
                System.Console.WindowHeight = value;

                ReApplySizing();
            }
        }

        private void ReApplySizing()
        {
            System.Console.SetBufferSize(Width, Height + 1);
        }

        private bool _draw = false;

        public bool Draw
        {
            get => _draw;
            set => _draw = value;
        }

        /*
        public void Display(ConsoleCharBuffer buffer)
        {
            if (_draw)
            {
                for (int y = 0; y < buffer.Height; y++)
                    for (int x = 0; x < buffer.Width; x++)
                    {
                        System.Console.SetCursorPosition(x, y);
                        System.Console.BackgroundColor = buffer.data[x, y].Background ?? ConsoleColor.Black;
                        System.Console.ForegroundColor = buffer.data[x, y].Foreground;
                        System.Console.Write(buffer.data[x, y].Char);
                    }

                System.Console.SetWindowPosition(0, 0);
                System.Console.SetCursorPosition(0, 0);
            }
        }
        */
        public void Display(ConsoleCharBuffer buffer)
        {
            Display(buffer.dataPositioned);
        }

        private object _drawLock = new object();

        public void Display(IEnumerable<PositionedConsoleChar> chars)
        {
            if (_draw)
            {
                lock (_drawLock)
                {

                    System.Console.SetCursorPosition(0, 0);
                    System.Console.SetWindowPosition(0, 0);

                    var result = chars.GroupBy(item =>
                            item.Y
                        );

                    foreach (var lineChars in result)
                    {
                        var groupped = lineChars.GroupAdjacentBy((x1, x2) => x1.X + 1 == x2.X && x1.Background == x2.Background && x1.Foreground == x2.Foreground);

                        foreach (var subGroup in groupped)
                        {
                            var orderedSubGroup = subGroup.OrderBy(item => item.X);

                            var firstChar = orderedSubGroup.First();
                            string groupStr = new string(orderedSubGroup.Select(item => item.Char).ToArray());

                            System.Console.SetCursorPosition(firstChar.X, firstChar.Y);
                            System.Console.BackgroundColor = firstChar.Background ?? ConsoleColor.Black;
                            System.Console.ForegroundColor = firstChar.Foreground;
                            System.Console.Write(groupStr);
                        }

                    }


                    System.Console.SetCursorPosition(0, 0);
                    System.Console.SetWindowPosition(0, 0);

                    System.Console.BackgroundColor = _initBackgroundColor;
                    System.Console.ForegroundColor = _initForegroundColor;

                    //ReApplySizing();
                }


            }
        }

        public event Engine.KeyboardEvent KeyboardEvent;
        public event Engine.MouseEvent MouseEvent;

        private ConsoleColor _initBackgroundColor;
        private ConsoleColor _initForegroundColor;

        private int _initXPos;
        private int _initYPos;

        private Encoding _initOutputEncoding;
        private bool _initCursorVisible;

        private int _initMode;

        public void Init()
        {

            _initXPos = System.Console.CursorLeft;
            _initYPos = System.Console.CursorTop;

            _initBackgroundColor = System.Console.BackgroundColor;
            _initForegroundColor = System.Console.ForegroundColor;

            _initOutputEncoding = System.Console.OutputEncoding;
            _initCursorVisible = System.Console.CursorVisible;

            System.Console.OutputEncoding = System.Text.Encoding.Unicode;
            System.Console.CursorVisible = false;

            System.Console.SetWindowPosition(0, 0);
            System.Console.SetCursorPosition(0, 0);

            _draw = true;

            var handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            if (!NativeMethods.GetConsoleMode(handle, ref _initMode)) { throw new Win32Exception(); }

            _initMode |= NativeMethods.ENABLE_MOUSE_INPUT;
            _initMode &= ~NativeMethods.ENABLE_QUICK_EDIT_MODE;
            _initMode |= NativeMethods.ENABLE_EXTENDED_FLAGS;

            if (!NativeMethods.SetConsoleMode(handle, _initMode)) { throw new Win32Exception(); }
        }


        public void End()
        {
            _draw = false;

            System.Console.CursorLeft = _initXPos;
            System.Console.CursorTop = _initYPos;

            System.Console.BackgroundColor = _initBackgroundColor;
            System.Console.ForegroundColor = _initForegroundColor;

            System.Console.OutputEncoding = _initOutputEncoding;
            System.Console.CursorVisible = _initCursorVisible;
        }

        public void StartCapture()
        {
            NativeMethods.ConsoleHandle handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            NativeMethods.INPUT_RECORD record = new NativeMethods.INPUT_RECORD();
            uint recordLen = 0;

            while (true)
            {
                if (!(NativeMethods.ReadConsoleInput(handle, ref record, 1, ref recordLen))) { throw new Win32Exception(); }


                switch (record.EventType)
                {
                    case NativeMethods.MOUSE_EVENT:
                        {


                        }
                        break;
                    case NativeMethods.KEY_EVENT:
                        {
                            NativeMethods.ControlKeyState states = (NativeMethods.ControlKeyState)record.KeyEvent.dwControlKeyState;

                            KeyboardEvent evt = new KeyboardEvent(EventPhase.Root,
                            null,
                            null,
                            record.KeyEvent.UnicodeChar,
                            record.KeyEvent.AsciiChar,
                            (VirtualKey)record.KeyEvent.wVirtualKeyCode,
                            record.KeyEvent.bKeyDown,
                            altKey: states.HasFlag(NativeMethods.ControlKeyState.LEFT_ALT_PRESSED) || states.HasFlag(NativeMethods.ControlKeyState.RIGHT_ALT_PRESSED),
                            controlKey: states.HasFlag(NativeMethods.ControlKeyState.LEFT_CTRL_PRESSED) || states.HasFlag(NativeMethods.ControlKeyState.RIGHT_CTRL_PRESSED),
                            shiftKey: states.HasFlag(NativeMethods.ControlKeyState.SHIFT_PRESSED));

                            KeyboardEvent?.Invoke(evt);
                        }
                        break;
                }
            }
        }

        private class NativeMethods
        {

            public const Int32 STD_INPUT_HANDLE = -10;

            public const Int32 ENABLE_MOUSE_INPUT = 0x0010;
            public const Int32 ENABLE_QUICK_EDIT_MODE = 0x0040;
            public const Int32 ENABLE_EXTENDED_FLAGS = 0x0080;

            public const Int32 KEY_EVENT = 1;
            public const Int32 MOUSE_EVENT = 2;


            [DebuggerDisplay("EventType: {EventType}")]
            [StructLayout(LayoutKind.Explicit)]
            public struct INPUT_RECORD
            {
                [FieldOffset(0)]
                public Int16 EventType;
                [FieldOffset(4)]
                public KEY_EVENT_RECORD KeyEvent;
                [FieldOffset(4)]
                public MOUSE_EVENT_RECORD MouseEvent;
            }

            [DebuggerDisplay("{dwMousePosition.X}, {dwMousePosition.Y}")]
            public struct MOUSE_EVENT_RECORD
            {
                public COORD dwMousePosition;
                public Int32 dwButtonState;
                public Int32 dwControlKeyState;
                public Int32 dwEventFlags;
            }

            [DebuggerDisplay("{X}, {Y}")]
            public struct COORD
            {
                public UInt16 X;
                public UInt16 Y;
            }

            [DebuggerDisplay("KeyCode: {wVirtualKeyCode}")]
            [StructLayout(LayoutKind.Explicit)]
            public struct KEY_EVENT_RECORD
            {
                [FieldOffset(0)]
                [MarshalAsAttribute(UnmanagedType.Bool)]
                public Boolean bKeyDown;
                [FieldOffset(4)]
                public UInt16 wRepeatCount;
                [FieldOffset(6)]
                public UInt16 wVirtualKeyCode;
                [FieldOffset(8)]
                public UInt16 wVirtualScanCode;
                [FieldOffset(10)]
                public Char UnicodeChar;
                [FieldOffset(10)]
                public Byte AsciiChar;
                [FieldOffset(12)]
                public Int32 dwControlKeyState;
            };

            [Flags]
            public enum ControlKeyState : Int32
            {
                RIGHT_ALT_PRESSED = 0x0001,
                LEFT_ALT_PRESSED = 0x0002,
                RIGHT_CTRL_PRESSED = 0x0004,
                LEFT_CTRL_PRESSED = 0x0008,
                SHIFT_PRESSED = 0x0010,
                NUMLOCK_ON = 0x0020,
                SCROLLLOCK_ON = 0x0040,
                CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100
            }

            public class ConsoleHandle : SafeHandleMinusOneIsInvalid
            {
                public ConsoleHandle() : base(false) { }

                protected override bool ReleaseHandle()
                {
                    return true; //releasing console handle is not our business
                }
            }


            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean GetConsoleMode(ConsoleHandle hConsoleHandle, ref Int32 lpMode);

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            public static extern ConsoleHandle GetStdHandle(Int32 nStdHandle);

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean ReadConsoleInput(ConsoleHandle hConsoleInput, ref INPUT_RECORD lpBuffer, UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean SetConsoleMode(ConsoleHandle hConsoleHandle, Int32 dwMode);

        }
    }
}
