using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms.Components.Texts
{
    public class SingleLineTextbox : DisplayObject, IAcceptInput, IFocusable
    {

        public ConsoleColor? BackgroudColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? FocusedBackgroudColor = ConsoleColor.DarkMagenta;
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
                    _displayOffset = 0;
                    _cursorOffset = 0;
                    _text = value;
                    Dirty = true;
                }
            }
        }

        private string _placeHolderText;
        public string PlaceHolderText
        {
            get { return _placeHolderText; }
            set
            {
                if (_placeHolderText != value)
                {
                    _placeHolderText = value;
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
                _displayOffset = 0;
                _cursorOffset = 0;
                _width = value;
                Dirty = true;
            }
        }

        public SingleLineTextbox(Container parent, string text = "", string placeHolderText = "", int? maxLength = null, int width = 10)
        {
            _text = text;
            _maxLength = maxLength;
            _placeHolderText = placeHolderText;
            _width = width;

            Parent = parent;

            parent.AddChild(this);
        }

        private int _displayOffset = 0;
        private int _cursorOffset = 0;

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width, 1);

            

            if (string.IsNullOrEmpty(_text))
            {
                buffer.Clear(new ConsoleChar(this, ' ',true, Focused ? FocusedBackgroudColor : BackgroudColor, PlaceHolderForegroundColor));

                buffer.DrawString(this, _placeHolderText.Truncate(Width), true, 0, 0, Focused ? FocusedBackgroudColor : BackgroudColor, PlaceHolderForegroundColor);
            }
            else
            {
                buffer.Clear(new ConsoleChar(this, ' ', true, Focused ? FocusedBackgroudColor : BackgroudColor, Focused ? FocusedForegroundColor : ForegroundColor));

                string displayStr = (_text + new string(' ', _width)).Substring(_displayOffset,_width);

                buffer.DrawString(this, displayStr, true, 0, 0, Focused ? FocusedBackgroudColor : BackgroudColor,
                    Focused ? FocusedForegroundColor : ForegroundColor);

                ConsoleChar cursor = buffer.data[_cursorOffset - _displayOffset, 0];
                cursor.Background = CursorBackGroundColor;
                cursor.Foreground = CursorForegroundColor;
                buffer.data[_cursorOffset - _displayOffset, 0] = cursor;
            }

            Dirty = false;

            return buffer;
        }

        public bool FireKeypress(ConsoleKeyInfo key)
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
                    break;
                case ConsoleKey.End:
                    _cursorOffset = _text.Length;
                    _displayOffset = 0;

                    if (_text.Length >= Width)
                        _displayOffset = _text.Length - Width + 1;

                    Dirty = true;

                    return true;
                    break;
            }

            return false;
        }

        public event KeyPressEventHandler Keypress;

        private bool _focused = false;
        public bool Focused
        {
            get { return _focused; }
            set
            {
                if (_focused != value)
                {
                    _focused = value;
                    Dirty = true;
                }
            }
        }

        public event FocusEventHandler FocusIn;
        public event FocusEventHandler FocusOut;
        public void FireFocusIn(ConsoleKeyInfo? key)
        {
            
        }

        public void FireFocusOut(ConsoleKeyInfo? key)
        {
            
        }
    }
}
