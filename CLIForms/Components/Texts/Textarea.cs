using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;

namespace CLIForms.Components.Texts
{
    public class Textarea : InteractiveObject
    {

        public ConsoleColor? BackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? FocusedBackgroundColor = ConsoleColor.DarkMagenta;
        public ConsoleColor FocusedForegroundColor = ConsoleColor.Black;

        public ConsoleColor? CursorBackGroundColor = ConsoleColor.White;
        public ConsoleColor CursorForegroundColor = ConsoleColor.Black;

        public ConsoleColor PlaceHolderForegroundColor = ConsoleColor.Gray;

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _displayXOffset = 0;
                    _displayYOffset = 0;
                    _cursorXOffset = 0;
                    _cursorYOffset = 0;
                    _text = value;
                    Dirty = true;
                }
            }
        }

        private int? _maxLength;
        public int? MaxLength
        {
            get { return _maxLength; }
            set
            {
                if (_maxLength != value)
                {
                    _maxLength = value;
                    Dirty = true;
                }
            }
        }

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _displayXOffset = 0;
                _displayYOffset = 0;
                _cursorXOffset = 0;
                _cursorYOffset = 0;
                _width = value;
                Dirty = true;
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                _displayXOffset = 0;
                _displayYOffset = 0;
                _cursorXOffset = 0;
                _cursorYOffset = 0;
                _height = value;
                Dirty = true;
            }
        }

        private bool _autoLineBreak = true;
        public bool AutoLineBreak
        {
            get => _autoLineBreak;
            set
            {
                _displayXOffset = 0;
                _displayYOffset = 0;
                _cursorXOffset = 0;
                _cursorYOffset = 0;
                _autoLineBreak = value;
                Dirty = true;
            }
        }


        public Textarea(Container parent, string text = "", int? maxLength = null, int width = 20, int height = 10) : base(parent)
        {
            _text = text;
            _maxLength = maxLength;
            _width = width;
            _height = height;
        }

        private int _displayXOffset = 0;
        private int _displayYOffset = 0;
        private int _cursorXOffset = 0;
        private int _cursorYOffset = 0;

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width, Height);
            buffer.Clear(new ConsoleChar(this, ' ', true, Focused ? FocusedBackgroundColor : BackgroundColor, Focused ? FocusedForegroundColor : ForegroundColor));

            if (Text != null)
            {
                string[] lines = Text.Split('\n');

                for (int y = 0; y < lines.Length; y++)
                {
                    buffer.DrawString(this, lines[y], true, 0, y, Focused ? FocusedBackgroundColor : BackgroundColor, Focused ? FocusedForegroundColor : ForegroundColor);
                }
            }

            Dirty = false;

            return buffer;
        }

        public bool KeyPressed(ConsoleKeyInfo key)
        {
            return false;
        }

        /*
        public bool KeyPressed(ConsoleKeyInfo key)
        {
            if (key.IsPrintable())
            {
                if (MaxLength != null && _text.Length >= MaxLength)
                    return true;

                _text = _text.Insert(_cursorOffset, key.KeyChar.ToString());

                _cursorOffset++;

                if (_cursorOffset - _displayOffset >= Width)
                {
                    _displayOffset++;
                }

                Dirty = true;

                return true;
            }

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (_cursorOffset > 0)
                    {
                        _cursorOffset--;

                        if (_cursorOffset < _displayOffset)
                            _displayOffset = _cursorOffset;

                        Dirty = true;

                        return true;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (_cursorOffset < _text.Length)
                    {
                        _cursorOffset++;

                        if (_cursorOffset >= _displayOffset + _width)
                            _displayOffset++;

                        Dirty = true;

                        return true;
                    }
                    break;
                case ConsoleKey.Home:
                    _cursorOffset = 0;
                    _displayOffset = 0;

                    Dirty = true;

                    return true;
                case ConsoleKey.End:
                    _cursorOffset = _text.Length;
                    _displayOffset = 0;

                    if (_text.Length >= Width)
                        _displayOffset = _text.Length - Width + 1;

                    Dirty = true;
                    return true;
                case ConsoleKey.Backspace:
                    if (_cursorOffset > 0)
                    {
                        _text = _text.Remove(_cursorOffset - 1, 1);

                        _cursorOffset--;

                        if (_cursorOffset < _displayOffset)
                            _displayOffset = _cursorOffset;

                        Dirty = true;
                    }

                    return true;
                case ConsoleKey.Delete:
                    if (_cursorOffset < _text.Length)
                    {
                        _text = _text.Remove(_cursorOffset, 1);

                        if (_cursorOffset < _displayOffset)
                            _displayOffset = _cursorOffset;

                        Dirty = true;
                    }
                    return true;
            }

            return false;
        }
        */
        
    }
}
