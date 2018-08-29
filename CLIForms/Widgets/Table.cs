using System;
using System.Linq;
using System.Text;

namespace CLIForms.Widgets
{
    public enum TableStyle
    {
        Full,
        Compact,
        CompactWithHeader,
        FullNoExtBorder,
        CompactNoExtBorder,
        CompactWithHeaderNoExtBorder,
    }

    public enum ColumnAlignment
    {
        Left,
        Center,
        Right
    }

    public class Table : Widget
    {
        internal Table() { }
        public Table(Widget parent) : base(parent)
        {
            Background = parent.Background;
            Foreground = parent.Foreground;
            Border = parent.Border;

            ResizeDataArray();
        }

        private TableStyle _tableStyle = TableStyle.Full;

        public TableStyle TableStyle {
            get { return _tableStyle; }
            set {
                if (value != _tableStyle)
                {
                    _tableStyle = value;
                    Parent.Draw();
                }
            }
        }

        private int _columnCount = 2;

        public int ColumnCount
        {
            get
            {
                return _columnCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("ColumnCount must be positive");

                _columnCount = value;
                ResizeDataArray();
            }
        }

        private int _lineCount = 2;

        public int LineCount
        {
            get
            {
                return _lineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("LineCount must be positive");

                _lineCount = value;
                ResizeDataArray();
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
                ColumnAlignment[] newAlignments = Enumerable.Repeat(ColumnAlignment.Left, _columnCount).ToArray();

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
            Parent.Draw();
        }

        private string[,] data = new string[2,2];

        // TODO : column width validation
        private int[] _columnsWidth = new[] { 5, 5 };
        public int[] ColumnsWidth
        {
            get { return _columnsWidth; }
            set
            {
                _columnsWidth = value;
                Parent.Draw();
            }
        }

        private ColumnAlignment[] _columnsAlignments = new[] { ColumnAlignment.Left, ColumnAlignment.Left };
        public ColumnAlignment[] ColumnsAlignments
        {
            get { return _columnsAlignments; }
            set
            {
                _columnsAlignments = value;
                Parent.Draw();
            }
        }



        public string this[int column, int line]
        {
            get { return data[column, line]; }
            set
            {
                data[column, line] = value;
                Draw();
            }
        }


        internal override void Render()
        {
            Console.ForegroundColor = Foreground;
            Console.BackgroundColor = Background;



            switch (_tableStyle)
            {
                case TableStyle.Full:
                    RenderFull();
                    break;
                case TableStyle.Compact:
                    RenderCompact();
                    break;
                case TableStyle.CompactWithHeader:
                    RenderCompactWithHeader();
                    break;
            }
        }

        private string RenderCell(string cellData, int cellSize, ColumnAlignment alignment)
        {
            if (cellData.Length > cellSize)
                cellData = cellData.Substring(0, cellSize - 3) + "...";

            switch (alignment)
            {
                case ColumnAlignment.Left:
                    cellData = cellData.PadRight(cellSize, ' ');
                    break;
                case ColumnAlignment.Center:
                    int centerOffset = cellSize - cellData.Length / 2;
                    cellData = new string(' ', centerOffset) + cellData + new string(' ', cellSize - centerOffset - cellData.Length);
                    break;
                case ColumnAlignment.Right:
                    cellData = cellData.PadRight(cellSize, ' ');
                    break;
            }

            return cellData;
        }

        private void RenderFull()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;

            // top border

            int offsetX = 0;

            Console.SetCursorPosition(DisplayLeft, DisplayTop);

            Console.Write(ConsoleHelper.GetTopLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetBotomTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetTopRightCornerBorder(Border));

            for (int line = 0; line < _lineCount - 1; line++)
            {
                offsetX++;

                Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

                Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                for (int col = 0; col < _columnCount; col++)
                {
                    var a = data[col, line];
                    var b = _columnsWidth[col];
                    var c = _columnsAlignments[col];

                    Console.Write(RenderCell(data[col, line], _columnsWidth[col], _columnsAlignments[col]));
                    Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                }

                // separator

                offsetX++;

                Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

                Console.Write(ConsoleHelper.GetRightTJunctionBorder(Border));
                Console.Write(String.Join(ConsoleHelper.GetCrossJunctionBorder(Border),
                    _columnsWidth.Select(
                        colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
                Console.Write(ConsoleHelper.GetLeftTJunctionBorder(Border));

            }

            // last line

            offsetX++;

            Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

            Console.Write(ConsoleHelper.GetVerticalBorder(Border));
            for (int col = 0; col < _columnCount; col++)
            {
                Console.Write(RenderCell(data[col, _lineCount - 1], _columnsWidth[col], _columnsAlignments[col]));
                Console.Write(ConsoleHelper.GetVerticalBorder(Border));
            }

            // bottom border

            offsetX++;

            Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

            Console.Write(ConsoleHelper.GetBottomLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetTopTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetBottomRightCornerBorder(Border));

        }

        private void RenderCompact()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;

            // top border

            int offsetX = 0;

            Console.SetCursorPosition(DisplayLeft, DisplayTop);

            Console.Write(ConsoleHelper.GetTopLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetBotomTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetTopRightCornerBorder(Border));

            for (int line = 0; line < _lineCount; line++)
            {
                offsetX++;

                Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

                Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                for (int col = 0; col < _columnCount; col++)
                {
                    Console.Write(RenderCell(data[col, line], _columnsWidth[col], _columnsAlignments[col]));
                    Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                }

            }

            // bottom border

            offsetX++;

            Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

            Console.Write(ConsoleHelper.GetBottomLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetTopTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetBottomRightCornerBorder(Border));

        }

        private void RenderCompactWithHeader()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnCount - 1) + 2;

            // top border

            int offsetX = 0;

            Console.SetCursorPosition(DisplayLeft, DisplayTop);

            Console.Write(ConsoleHelper.GetTopLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetBotomTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetTopRightCornerBorder(Border));

            // Header
            offsetX++;

            Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

            Console.Write(ConsoleHelper.GetVerticalBorder(Border));
            for (int col = 0; col < _columnCount; col++)
            {
                Console.Write(RenderCell(data[col, 0], _columnsWidth[col], _columnsAlignments[col]));
                Console.Write(ConsoleHelper.GetVerticalBorder(Border));
            }

            // separator

            offsetX++;

            Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

            Console.Write(ConsoleHelper.GetRightTJunctionBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetCrossJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetLeftTJunctionBorder(Border));


            for (int line = 1; line < _lineCount; line++)
            {
                offsetX++;

                Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

                Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                for (int col = 0; col < _columnCount; col++)
                {
                    Console.Write(RenderCell(data[col, line], _columnsWidth[col], _columnsAlignments[col]));
                    Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                }

            }

            // bottom border

            offsetX++;

            Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

            Console.Write(ConsoleHelper.GetBottomLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetTopTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetBottomRightCornerBorder(Border));

        }
    }
}
