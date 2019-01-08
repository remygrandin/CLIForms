using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Engine.Events;
using CLIForms.Extentions;

namespace CLIForms.Components.Texts
{
    public class Textbox : InteractiveObject
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

        private bool _isPassword;
        public bool IsPassword
        {
            get => _isPassword;
            set
            {
                if (_isPassword != value)
                {
                    _isPassword = value;
                    Dirty = true;
                }
            }
        }

        private char _passwordChar = '*';
        public char PasswordChar
        {
            get => _passwordChar;
            set
            {
                if (_passwordChar != value)
                {
                    _passwordChar = value;
                    Dirty = true;
                }
            }
        }

        public Textbox(Container parent, string text = "", string placeHolderText = "", int? maxLength = null, int width = 10) : base(parent)
        {
            _text = text;
            _maxLength = maxLength;
            _placeHolderText = placeHolderText;
            _width = width;

            KeyDown += Textbox_KeyDown;
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
                buffer.Clear(new ConsoleChar(this, ' ', true, Focused ? FocusedBackgroundColor : BackgroundColor, PlaceHolderForegroundColor));

                buffer.DrawString(this, _placeHolderText.Truncate(Width), true, 0, 0, Focused ? FocusedBackgroundColor : BackgroundColor, PlaceHolderForegroundColor);
            }
            else
            {
                buffer.Clear(new ConsoleChar(this, ' ', true, Focused ? FocusedBackgroundColor : BackgroundColor, Focused ? FocusedForegroundColor : ForegroundColor));

                string displayStr = "";

                if (_isPassword)
                {
                    displayStr = (new string(PasswordChar, _text.Length) + new string(' ', _width)).Substring(_displayOffset, _width);
                }
                else
                {
                    displayStr = (_text + new string(' ', _width)).Substring(_displayOffset, _width);
                }

                buffer.DrawString(this, displayStr, true, 0, 0, Focused ? FocusedBackgroundColor : BackgroundColor,
                    Focused ? FocusedForegroundColor : ForegroundColor);

                ConsoleChar cursor = buffer.data[_cursorOffset - _displayOffset, 0];
                cursor.Background = CursorBackGroundColor;
                cursor.Foreground = CursorForegroundColor;
                buffer.data[_cursorOffset - _displayOffset, 0] = cursor;
            }

            Dirty = false;

            return buffer;
        }

        private void Textbox_KeyDown(Engine.Events.KeyboardEvent evt)
        {

            if (evt.CharCode.IsPrintable())
            {
                if (MaxLength != null && _text.Length >= MaxLength)
                {
                    evt.PreventDefault();
                    return;
                }

                _text = _text.Insert(_cursorOffset, evt.CharCode.ToString());

                _cursorOffset++;

                if (_cursorOffset - _displayOffset >= Width)
                {
                    _displayOffset++;
                }

                Dirty = true;

                evt.PreventDefault();
                return;
            }

            switch (evt.VirtualKeyCode)
            {
                case VirtualKey.Left:
                    if (_cursorOffset > 0)
                    {
                        _cursorOffset--;

                        if (_cursorOffset < _displayOffset)
                            _displayOffset = _cursorOffset;

                        Dirty = true;

                        evt.PreventDefault();
                        return;
                    }

                    break;
                case VirtualKey.Right:
                    if (_cursorOffset < _text.Length)
                    {
                        _cursorOffset++;

                        if (_cursorOffset >= _displayOffset + _width)
                            _displayOffset++;

                        Dirty = true;

                        evt.PreventDefault();
                        return;
                    }
                    break;
                case VirtualKey.Home:
                    _cursorOffset = 0;
                    _displayOffset = 0;

                    Dirty = true;

                    evt.PreventDefault();
                    return;
                case VirtualKey.End:
                    _cursorOffset = _text.Length;
                    _displayOffset = 0;

                    if (_text.Length >= Width)
                        _displayOffset = _text.Length - Width + 1;

                    Dirty = true;
                    evt.PreventDefault();
                    return;
                case VirtualKey.Back:
                    if (_cursorOffset > 0)
                    {
                        _text = _text.Remove(_cursorOffset - 1, 1);

                        _cursorOffset--;

                        if (_cursorOffset < _displayOffset)
                            _displayOffset = _cursorOffset;

                        Dirty = true;
                    }

                    evt.PreventDefault();
                    return;
                case VirtualKey.Delete:
                    if (_cursorOffset < _text.Length)
                    {
                        _text = _text.Remove(_cursorOffset, 1);

                        if (_cursorOffset < _displayOffset)
                            _displayOffset = _cursorOffset;

                        Dirty = true;
                    }
                    evt.PreventDefault();
                    return;
            }
        }
    }
}
