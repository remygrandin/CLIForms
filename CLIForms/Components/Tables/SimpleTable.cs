﻿using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Styles;

namespace CLIForms.Components.Tables
{
    public class SimpleTable : DisplayObject
    {
        public ConsoleColor? BorderBackgroundColor = ConsoleColor.Gray;
        public ConsoleColor BorderForegroundColor = ConsoleColor.DarkGray;

        public ConsoleColor? CellBackgroundColor = ConsoleColor.Gray;
        public ConsoleColor CellForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public ShadowStyle Shadow = ShadowStyle.Light;

        public SimpleTable(Container parent) : base(parent)
        {


            ResizeDataArray();
        }

        private TableStyle _tableStyle = TableStyle.Full;

        public TableStyle TableStyle
        {
            get { return _tableStyle; }
            set
            {
                if (value != _tableStyle)
                {
                    _tableStyle = value;
                    Dirty = true;
                }
            }
        }

        private int _columnCount = 2;

        public int ColumnCount
        {
            get => _columnCount;
            set
            {
                if (value < 0)
                    throw new ArgumentException("ColumnCount must be positive");

                _columnCount = value;
                ResizeDataArray();
                Dirty = true;
            }
        }

        private int _lineCount = 2;

        public int LineCount
        {
            get => _lineCount;
            set
            {
                if (value < 0)
                    throw new ArgumentException("LineCount must be positive");

                _lineCount = value;
                ResizeDataArray();
                Dirty = true;
            }
        }

        private void ResizeDataArray()
        {
            string[,] newData = new string[_columnCount, _lineCount];

            for (int i = 0; i < _columnCount; i++)
            for (int j = 0; j < _lineCount; j++)
            {
                newData[i, j] = "";
            }

            for (int i = 0; i < _columnCount; i++)
            {
                if (i < data.GetLength(0))
                    for (int j = 0; j < _lineCount; j++)
                    {
                        if (j < data.GetLength(1) && data[i, j] != null)
                            newData[i, j] = data[i, j];
                    }
            }

            if (_columnsAlignments.Length > _columnCount)
            {
                _columnsAlignments = _columnsAlignments.Take(_columnCount).ToArray();
            }
            else if (_columnsAlignments.Length < _columnCount)
            {
                AlignmentStyle[] newAlignments = Enumerable.Repeat(AlignmentStyle.Left, _columnCount).ToArray();

                Array.Copy(_columnsAlignments, newAlignments, _columnsAlignments.Length);

                _columnsAlignments = newAlignments;
            }

            if (_columnsWidth.Length > _columnCount)
            {
                _columnsWidth = _columnsWidth.Take(_columnCount).ToArray();
            }
            else if (_columnsWidth.Length < _columnCount)
            {
                int[] newWidths = Enumerable.Repeat(5, _columnCount).ToArray();

                Array.Copy(_columnsWidth, newWidths, _columnsWidth.Length);

                _columnsWidth = newWidths;
            }

            data = newData;
            Dirty = true;
        }

        private string[,] data = new string[2, 2];

        // TODO : column width validation
        private int[] _columnsWidth = new[] {5, 5};

        public int[] ColumnsWidth
        {
            get { return _columnsWidth; }
            set
            {
                _columnsWidth = value;
                Dirty = true;
            }
        }

        private AlignmentStyle[] _columnsAlignments = new[] {AlignmentStyle.Left, AlignmentStyle.Left};

        public AlignmentStyle[] ColumnsAlignments
        {
            get { return _columnsAlignments; }
            set
            {
                _columnsAlignments = value;
                Dirty = true;
            }
        }



        public string this[int column, int line]
        {
            get
            {
                if (column < 0 || line < 0 || column > data.GetLength(0) || line > data.GetLength(1))
                    throw new Exception("column or line out of range");

                return data[column, line];
            }
            set
            {
                data[column, line] = value;
                Dirty = true;
            }
        }

        public string[] this[int line]
        {
            get
            {
                if (line < 0 || line > data.GetLength(1))
                    throw new Exception("line out of range");

                List<string> lineData = new List<string>(data.GetLength(0));

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    lineData.Add(data[i, line]);
                }

                return lineData.ToArray();
            }
            set
            {
                if (line < 0 || line > data.GetLength(1))
                    throw new Exception("line out of range");

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    data[i, line] = value[i];
                }

                Dirty = true;
            }
        }

        public string GetCell(int column, int line)
        {
            return data[column, line];
        }

        public void SetCell(int column, int line, string value)
        {
            data[column, line] = value;
        }


        public override ConsoleCharBuffer Render()
        {
            ConsoleCharBuffer buffer = null;

            switch (_tableStyle)
            {
                case TableStyle.Full:
                    buffer = RenderFull();
                    break;
                case TableStyle.Compact:
                    buffer = RenderCompact();
                    break;
                case TableStyle.CompactWithHeader:
                    buffer = RenderCompactWithHeader();
                    break;
                case TableStyle.FullNoExtBorder:
                    buffer = RenderFullNoExtBorder();
                    break;
                case TableStyle.CompactNoExtBorder:
                    buffer = RenderCompactNoExtBorder();
                    break;
                case TableStyle.CompactWithHeaderNoExtBorder:
                    buffer = RenderCompactWithHeaderNoExtBorder();
                    break;
            }

            return buffer;
        }

        private string RenderCell(string cellData, int cellSize, AlignmentStyle alignment)
        {
            if (cellData.Length > cellSize)
                cellData = cellData.Substring(0, cellSize - 3) + "...";

            switch (alignment)
            {
                case AlignmentStyle.Left:
                    cellData = cellData.PadRight(cellSize, ' ');
                    break;
                case AlignmentStyle.Center:
                    int centerOffset = (cellSize - cellData.Length) / 2;
                    cellData = new string(' ', centerOffset) + cellData + new string(' ', cellSize - (cellData.Length + centerOffset));
                    break;
                case AlignmentStyle.Right:
                    cellData = cellData.PadLeft(cellSize, ' ');
                    break;
            }

            return cellData;
        }

        private ConsoleCharBuffer RenderFull()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;
            int fullHeight = LineCount * 2 + 1;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(fullWidth, fullHeight);

            // top border

            baseBuffer.DrawString(this, DrawingHelper.GetTopLeftCornerBorder(Border), false, 0, 0, BorderBackgroundColor, BorderForegroundColor);

            int xOffset = 1;

            for (int i = 0; i < ColumnCount; i++)
            {
                baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[i]), false, xOffset, 0, BorderBackgroundColor, BorderForegroundColor);
                xOffset += ColumnsWidth[i];

                baseBuffer.DrawString(this, DrawingHelper.GetBottomTJunctionBorder(Border), false, xOffset, 0, BorderBackgroundColor, BorderForegroundColor);
                xOffset += 1;
            }

            baseBuffer.DrawString(this, DrawingHelper.GetTopRightCornerBorder(Border), false, fullWidth - 1, 0, BorderBackgroundColor, BorderForegroundColor);

            for (int i = 0; i < LineCount; i++)
            {
                int yOffset = i * 2 + 1;

                baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, yOffset, BorderBackgroundColor, BorderForegroundColor);
                baseBuffer.DrawString(this, DrawingHelper.GetRightTJunctionBorder(Border), false, 0, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                xOffset = 1;

                for (int j = 0; j < ColumnCount; j++)
                {
                    baseBuffer.DrawString(this, RenderCell(data[j, i], ColumnsWidth[j], ColumnsAlignments[j]), false, xOffset, yOffset, CellBackgroundColor, CellForegroundColor);
                    baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[j]), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                    xOffset += ColumnsWidth[j];

                    baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, xOffset, yOffset, BorderBackgroundColor, BorderForegroundColor);
                    baseBuffer.DrawString(this, DrawingHelper.GetCrossJunctionBorder(Border), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                    xOffset += 1;
                }


                baseBuffer.DrawString(this, DrawingHelper.GetLeftTJunctionBorder(Border), false, fullWidth - 1, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);
            }

            // bottom border

            baseBuffer.DrawString(this, DrawingHelper.GetBottomLeftCornerBorder(Border), false, 0, LineCount * 2, BorderBackgroundColor, BorderForegroundColor);

            xOffset = 1;

            for (int i = 0; i < ColumnCount; i++)
            {
                baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[i]), false, xOffset, LineCount * 2, BorderBackgroundColor, BorderForegroundColor);
                xOffset += ColumnsWidth[i];
                baseBuffer.DrawString(this, DrawingHelper.GetTopTJunctionBorder(Border), false, xOffset, LineCount * 2, BorderBackgroundColor, BorderForegroundColor);
                xOffset += 1;
            }

            baseBuffer.DrawString(this, DrawingHelper.GetBottomRightCornerBorder(Border), false, fullWidth - 1, LineCount * 2, BorderBackgroundColor, BorderForegroundColor);

            return baseBuffer;
        }

        private ConsoleCharBuffer RenderCompact()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;
            int fullHeight = LineCount + 2;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(fullWidth, fullHeight);

            // top border

            baseBuffer.DrawString(this, DrawingHelper.GetTopLeftCornerBorder(Border), false, 0, 0, BorderBackgroundColor, BorderForegroundColor);

            int xOffset = 1;

            for (int i = 0; i < ColumnCount; i++)
            {
                baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[i]), false, xOffset, 0, BorderBackgroundColor, BorderForegroundColor);
                xOffset += ColumnsWidth[i];

                baseBuffer.DrawString(this, DrawingHelper.GetBottomTJunctionBorder(Border), false, xOffset, 0, BorderBackgroundColor, BorderForegroundColor);
                xOffset += 1;
            }

            baseBuffer.DrawString(this, DrawingHelper.GetTopRightCornerBorder(Border), false, fullWidth - 1, 0, BorderBackgroundColor, BorderForegroundColor);

            for (int i = 0; i < LineCount; i++)
            {
                int yOffset = i + 1;

                baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, yOffset, BorderBackgroundColor, BorderForegroundColor);

                xOffset = 1;

                for (int j = 0; j < ColumnCount; j++)
                {
                    baseBuffer.DrawString(this, RenderCell(data[j, i], ColumnsWidth[j], ColumnsAlignments[j]), false, xOffset, yOffset, CellBackgroundColor, CellForegroundColor);


                    xOffset += ColumnsWidth[j];

                    baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, xOffset, yOffset, BorderBackgroundColor, BorderForegroundColor);


                    xOffset += 1;
                }


                baseBuffer.DrawString(this, DrawingHelper.GetLeftTJunctionBorder(Border), false, fullWidth - 1, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);
            }

            // bottom border

            baseBuffer.DrawString(this, DrawingHelper.GetBottomLeftCornerBorder(Border), false, 0, LineCount + 1, BorderBackgroundColor, BorderForegroundColor);

            xOffset = 1;

            for (int i = 0; i < ColumnCount; i++)
            {
                baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[i]), false, xOffset, LineCount + 1, BorderBackgroundColor, BorderForegroundColor);
                xOffset += ColumnsWidth[i];
                baseBuffer.DrawString(this, DrawingHelper.GetTopTJunctionBorder(Border), false, xOffset, LineCount + 1, BorderBackgroundColor, BorderForegroundColor);
                xOffset += 1;
            }

            baseBuffer.DrawString(this, DrawingHelper.GetBottomRightCornerBorder(Border), false, fullWidth - 1, LineCount + 1, BorderBackgroundColor, BorderForegroundColor);

            return baseBuffer;

        }

        private ConsoleCharBuffer RenderCompactWithHeader()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;
            int fullHeight = LineCount - 1 + 2 + 2;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(fullWidth, fullHeight);

            // top border

            baseBuffer.DrawString(this, DrawingHelper.GetTopLeftCornerBorder(Border), false, 0, 0, BorderBackgroundColor, BorderForegroundColor);

            int xOffset = 1;

            for (int i = 0; i < ColumnCount; i++)
            {
                baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[i]), false, xOffset, 0, BorderBackgroundColor, BorderForegroundColor);
                xOffset += ColumnsWidth[i];

                baseBuffer.DrawString(this, DrawingHelper.GetBottomTJunctionBorder(Border), false, xOffset, 0, BorderBackgroundColor, BorderForegroundColor);
                xOffset += 1;
            }

            baseBuffer.DrawString(this, DrawingHelper.GetTopRightCornerBorder(Border), false, fullWidth - 1, 0, BorderBackgroundColor, BorderForegroundColor);

            for (int i = 0; i < LineCount; i++)
            {

                int yOffset = i + 2;

                if (i == 0)
                    yOffset = 1;

                baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, yOffset, BorderBackgroundColor, BorderForegroundColor);

                if (i == 0)
                    baseBuffer.DrawString(this, DrawingHelper.GetRightTJunctionBorder(Border), false, 0, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                xOffset = 1;

                for (int j = 0; j < ColumnCount; j++)
                {
                    baseBuffer.DrawString(this, RenderCell(data[j, i], ColumnsWidth[j], ColumnsAlignments[j]), false, xOffset, yOffset, CellBackgroundColor, CellForegroundColor);
                    if (i == 0)
                        baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[j]), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                    xOffset += ColumnsWidth[j];

                    baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, xOffset, yOffset, BorderBackgroundColor, BorderForegroundColor);
                    if (i == 0)
                        baseBuffer.DrawString(this, DrawingHelper.GetCrossJunctionBorder(Border), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                    xOffset += 1;
                }

                if (i == 0)
                    baseBuffer.DrawString(this, DrawingHelper.GetLeftTJunctionBorder(Border), false, fullWidth - 1, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);
            }

            // bottom border

            baseBuffer.DrawString(this, DrawingHelper.GetBottomLeftCornerBorder(Border), false, 0, LineCount - 1 + 2 + 1, BorderBackgroundColor, BorderForegroundColor);

            xOffset = 1;

            for (int i = 0; i < ColumnCount; i++)
            {
                baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[i]), false, xOffset, LineCount - 1 + 2 + 1, BorderBackgroundColor, BorderForegroundColor);
                xOffset += ColumnsWidth[i];
                baseBuffer.DrawString(this, DrawingHelper.GetTopTJunctionBorder(Border), false, xOffset, LineCount - 1 + 2 + 1, BorderBackgroundColor, BorderForegroundColor);
                xOffset += 1;
            }

            baseBuffer.DrawString(this, DrawingHelper.GetBottomRightCornerBorder(Border), false, fullWidth - 1, LineCount - 1 + 2 + 1, BorderBackgroundColor, BorderForegroundColor);

            return baseBuffer;
        }

        private ConsoleCharBuffer RenderFullNoExtBorder()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1);
            int fullHeight = LineCount * 2 + 1;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(fullWidth, fullHeight);


            for (int i = 0; i < LineCount; i++)
            {
                int yOffset = i * 2 + 1;

                int xOffset = 0;

                for (int j = 0; j < ColumnCount; j++)
                {
                    baseBuffer.DrawString(this, RenderCell(data[j, i], ColumnsWidth[j], ColumnsAlignments[j]), false, xOffset, yOffset, CellBackgroundColor, CellForegroundColor);

                    if (i != LineCount - 1)
                        baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[j]), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                    xOffset += ColumnsWidth[j];
                    if (j != ColumnCount - 1)
                    {
                        baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, xOffset, yOffset, BorderBackgroundColor, BorderForegroundColor);
                        if (i != LineCount - 1)
                            baseBuffer.DrawString(this, DrawingHelper.GetCrossJunctionBorder(Border), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                        xOffset += 1;
                    }
                }
            }


            return baseBuffer;
        }

        private ConsoleCharBuffer RenderCompactNoExtBorder()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1);
            int fullHeight = LineCount;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(fullWidth, fullHeight);


            for (int i = 0; i < LineCount; i++)
            {
                int yOffset = i + 1;

                baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, yOffset, BorderBackgroundColor, BorderForegroundColor);

                int xOffset = 0;

                for (int j = 0; j < ColumnCount; j++)
                {
                    baseBuffer.DrawString(this, RenderCell(data[j, i], ColumnsWidth[j], ColumnsAlignments[j]), false, xOffset, yOffset, CellBackgroundColor, CellForegroundColor);


                    xOffset += ColumnsWidth[j];

                    if (j != ColumnCount - 1)
                    {
                        baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, xOffset, yOffset, BorderBackgroundColor, BorderForegroundColor);

                        xOffset += 1;
                    }
                }

            }


            return baseBuffer;
        }

        private ConsoleCharBuffer RenderCompactWithHeaderNoExtBorder()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;
            int fullHeight = LineCount - 1 + 2 + 2;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(fullWidth, fullHeight);


            for (int i = 0; i < LineCount; i++)
            {

                int yOffset = i + 2;

                if (i == 0)
                    yOffset = 1;


                int xOffset = 0;

                for (int j = 0; j < ColumnCount; j++)
                {
                    baseBuffer.DrawString(this, RenderCell(data[j, i], ColumnsWidth[j], ColumnsAlignments[j]), false, xOffset, yOffset, CellBackgroundColor, CellForegroundColor);
                    if (i == 0)
                        baseBuffer.DrawString(this, new String(DrawingHelper.GetHorizontalBorder(Border)[0], ColumnsWidth[j]), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                    xOffset += ColumnsWidth[j];

                    if (j != ColumnCount - 1)
                    {
                        baseBuffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, xOffset, yOffset, BorderBackgroundColor, BorderForegroundColor);
                        if (i == 0)
                            baseBuffer.DrawString(this, DrawingHelper.GetCrossJunctionBorder(Border), false, xOffset, yOffset + 1, BorderBackgroundColor, BorderForegroundColor);

                        xOffset += 1;
                    }
                }

            }


            return baseBuffer;

        }
    }
}
