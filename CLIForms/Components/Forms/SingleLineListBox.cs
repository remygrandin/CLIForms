﻿using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Engine.Events;
using CLIForms.Extentions;
using CLIForms.Styles;

namespace CLIForms.Components.Forms
{
    public class SingleLineListBox : InteractiveObject
    {

        public ConsoleColor? BackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor PlaceholderForegroundColor = ConsoleColor.Gray;

        public ConsoleColor? FocusedBackgroundColor = ConsoleColor.DarkMagenta;
        public ConsoleColor FocusedForegroundColor = ConsoleColor.Black;

        public ConsoleColor? ItemBackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor ItemForegroundColor = ConsoleColor.Black;

        public ConsoleColor? ItemActiveBackgroundColor = ConsoleColor.DarkBlue;
        public ConsoleColor ItemActiveForegroundColor = ConsoleColor.White;

        public ConsoleColor? ItemFocusedBackgroundColor = ConsoleColor.DarkMagenta;
        public ConsoleColor ItemFocusedForegroundColor = ConsoleColor.White;

        public ConsoleColor ArrowForegroundColor = ConsoleColor.White;

        public ShadowStyle DropdownShadow = ShadowStyle.Medium;

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

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    Dirty = true;
                }
            }
        }

        private object[] _items;
        public object[] Items
        {
            get => _items;
            set
            {
                _items = value;

                _selectedItems = value.Where(item => _items.Contains(_selectedItems)).ToArray();

                if (FocusedItem != null || !_items.Contains(FocusedItem))
                    FocusedItem = null;

                Dirty = true;
            }
        }

        private object[] _selectedItems = new object[0];
        public object[] SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (value == null)
                    _selectedItems = new object[0];
                else
                    _selectedItems = value.Where(item => _items.Contains(item)).ToArray();
                Dirty = true;
            }
        }

        private object _focusedItem = null;
        private object FocusedItem
        {
            get => _focusedItem;
            set
            {
                if (_focusedItem != value)
                {
                    _focusedItem = value;
                    Dirty = true;
                }
            }
        }


        private bool _multiSelectEnabled;
        public bool MultiSelectEnabled
        {
            get => _multiSelectEnabled;
            set
            {
                _multiSelectEnabled = value;
                Dirty = true;
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                Dirty = true;
            }
        }

        private string _displayProperty = null;
        public string DisplayProperty
        {
            get => _displayProperty;
            set
            {
                _displayProperty = value;
                Dirty = true;
            }
        }

        public SingleLineListBox(Container parent, object[] items, string placeHolderText = "", int width = 10, bool multiSelectEnabled = false) : base(parent)
        {
            _placeHolderText = placeHolderText;
            _width = width;
            _multiSelectEnabled = multiSelectEnabled;
            Items = items;

            KeyDown += SingleLineListBox_KeyDown;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer;

            string displayStr = "";

            if (_selectedItems.Length != 0)
            {
                List<string> displayStrs = new List<string>();
                foreach (object obj in SelectedItems)
                {
                    if (_displayProperty == null)
                        displayStrs.Add(obj.ToString());
                    else
                        displayStrs.Add(obj.GetType().GetProperty(_displayProperty)?.GetValue(obj).ToString());

                }

                displayStr = String.Join(",", displayStrs);
            }

            if (!_isOpen)
            {
                buffer = new ConsoleCharBuffer(Width, 1);
            }
            else
            {
                List<string> items = new List<string>();

                foreach (object obj in _items)
                {
                    string objVal = "";
                    if (_displayProperty == null)
                        objVal = obj.ToString();
                    else
                        objVal = obj.GetType().GetProperty(_displayProperty)?.GetValue(obj).ToString() ?? "";
                    
                    items.Add(objVal);
                }

                int maxWidth = items.Max(item => item.Length);

                int bufferMaxWidth = Math.Max(Width, maxWidth);

                buffer = new ConsoleCharBuffer(bufferMaxWidth + 1, 1 + _items.Length + 1);

                int yOffset = 1;
                foreach (object obj in _items)
                {
                    string objVal = "";
                    if (_displayProperty == null)
                        objVal = obj.ToString();
                    else
                        objVal = obj.GetType().GetProperty(_displayProperty)?.GetValue(obj).ToString() ?? "";


                    ConsoleColor? background = ItemBackgroundColor;
                    ConsoleColor foreground = ItemForegroundColor;

                    if (_selectedItems != null && _selectedItems.Contains(obj))
                    {
                        background = ItemActiveBackgroundColor;
                        foreground = ItemActiveForegroundColor;
                    }

                    if (obj == FocusedItem)
                    {
                        background = ItemFocusedBackgroundColor;
                        foreground = ItemFocusedForegroundColor;
                    }


                    buffer.DrawString(this, objVal.PadRight(maxWidth, ' '), true, 0, yOffset, background, foreground);

                    yOffset++;
                }

                buffer = DrawingHelper.DrawShadow(buffer, this, false, 0, 1, maxWidth, _items.Length,
                    ConsoleColor.Black, DropdownShadow);

            }


            buffer.DrawString(this, new string(' ', Width), true, 0, 0,
                Focused ? FocusedBackgroundColor : BackgroundColor,
                Focused ? FocusedForegroundColor : ForegroundColor);

            buffer.DrawString(this, displayStr.Truncate(Width - 1), true, 0, 0,
                Focused ? FocusedBackgroundColor : BackgroundColor,
                Focused ? FocusedForegroundColor : ForegroundColor);

            if (String.IsNullOrWhiteSpace(displayStr))
                buffer.DrawString(this, PlaceHolderText.Truncate(Width - 1), true, 0, 0,
                    Focused ? FocusedBackgroundColor : BackgroundColor, PlaceholderForegroundColor);


            buffer.data[Width - 1, 0].Char = _isOpen ? '▲' : '▼';
            buffer.data[Width - 1, 0].Foreground = ArrowForegroundColor;


            Dirty = false;

            return buffer;
        }

        private void SingleLineListBox_KeyDown(Engine.Events.KeyboardEvent evt)
        {
            switch (evt.VirtualKeyCode)
            {
                case VirtualKey.Enter:
                case VirtualKey.Space:
                    if (!_isOpen)
                    {
                        FocusedItem = _items.FirstOrDefault();
                        IsOpen = true;
                        evt.PreventDefault();
                        return;
                    }
                    else
                    {
                        if (MultiSelectEnabled)
                        {
                            if (SelectedItems == null || SelectedItems.Length == 0)
                            {
                                SelectedItems = new object[] { FocusedItem };

                            }
                            else
                            {
                                if (SelectedItems.Contains(SelectedItems))
                                {
                                    List<object> newSelectedList = SelectedItems.ToList();
                                    newSelectedList.Remove(FocusedItem);

                                    SelectedItems = newSelectedList.ToArray();
                                }
                                else
                                {
                                    List<object> newSelectedList = SelectedItems.ToList();
                                    newSelectedList.Add(FocusedItem);

                                    SelectedItems = newSelectedList.ToArray();
                                }
                            }
                        }
                        else
                        {
                            if (SelectedItems == null || SelectedItems.Length == 0)
                            {
                                SelectedItems = new object[] {FocusedItem};
                                IsOpen = false;
                            }
                            else
                            {
                                if (FocusedItem == SelectedItems[0])
                                    SelectedItems = new object[0];
                                else
                                    SelectedItems = new object[] { FocusedItem };

                                IsOpen = false;
                            }
                        }

                        evt.PreventDefault();
                        return;
                    }
                case VirtualKey.Down:
                    if (_isOpen)
                    {
                        int focusIndex = _items.ToList().IndexOf(FocusedItem);

                        focusIndex++;

                        if (focusIndex == _items.Length)
                        {
                            IsOpen = false;
                            FocusedItem = null;
                            return;
                        }

                        FocusedItem = _items[focusIndex];

                        evt.PreventDefault();
                        return;
                    }
                    break;
                case VirtualKey.Up:
                    if (_isOpen)
                    {
                        int focusIndex = _items.ToList().IndexOf(FocusedItem);

                        focusIndex--;

                        if (focusIndex == -1)
                        {
                            IsOpen = false;

                            FocusedItem = null;
                            evt.PreventDefault();
                            return;
                        }

                        FocusedItem = _items[focusIndex];

                        evt.PreventDefault();
                        return;
                    }
                    break;
            }
        }
    }
}
