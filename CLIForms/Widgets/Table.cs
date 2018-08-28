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

        private TableStyle _tableStyle;

        public TableStyle TableStyle;

        private int _columnsCount = 2;

        public int ColumnsCount
        {
            get
            {
                return _columnsCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("ColumnsCount must be positive");

                _columnsCount = value;
                ResizeDataArray();
            }
        }

        private int _linesCount = 2;

        public int LinesCount
        {
            get
            {
                return _linesCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("LinesCount must be positive");

                _linesCount = value;
                ResizeDataArray();
            }
        }

        private void ResizeDataArray()
        {
            string[,] newData = new string[_columnsCount, _linesCount];

            for (int i = 0; i < _columnsCount; i++)
                for (int j = 0; j < _linesCount; j++)
                {
                    newData[i, j] = "";
                }

            for (int i = 0; i < _columnsCount; i++)
            {
                if (i < data.GetLength(0))
                    for (int j = 0; j < _linesCount; j++)
                    {
                        if (j < data.GetLength(1) && data[i, j] != null)
                            newData[i, j] = data[i, j];
                    }
            }

            data = newData;
            Draw();
        }

        private string[,] data = new string[2,2];

        // TODO : column width validation
        private int[] _columnsWidth = new[] { 5, 5 };
        public int[] ColumnsWidth
        {
            get { return ColumnsWidth; }
            set
            {
                ColumnsWidth = value;
                Draw();
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



            switch (TableStyle)
            {
                case TableStyle.Full:
                    RenderFull();
                    break;

            }
        }

        private void RenderFull()
        {
            int fullWidth = _columnsWidth.Sum() + (_columnsCount - 1) + 2;

            // top border

            int offsetX = 0;

            Console.SetCursorPosition(DisplayLeft, DisplayTop);

            Console.Write(ConsoleHelper.GetTopLeftCornerBorder(Border));
            Console.Write(String.Join(ConsoleHelper.GetBotomTJunctionBorder(Border),
                _columnsWidth.Select(
                    colWidth => new string(ConsoleHelper.GetHorizontalBorder(Border)[0], colWidth))));
            Console.Write(ConsoleHelper.GetTopRightCornerBorder(Border));

            for (int line = 0; line < LinesCount - 1; line++)
            {
                offsetX++;

                Console.SetCursorPosition(DisplayLeft, DisplayTop + offsetX);

                Console.Write(ConsoleHelper.GetVerticalBorder(Border));
                for (int col = 0; col < ColumnsCount; col++)
                {
                    string cellData = data[col, line];

                    if (cellData.Length > _columnsWidth[col])
                        cellData = cellData.Substring(0, _columnsWidth[col] - 3) + "...";

                    cellData = cellData.PadLeft(_columnsWidth[col], ' ');

                    Console.Write(cellData);
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
            for (int col = 0; col < ColumnsCount; col++)
            {
                string cellData = data[col, LinesCount - 1];

                if (cellData.Length > _columnsWidth[col])
                    cellData = cellData.Substring(0, _columnsWidth[col] - 3) + "...";

                cellData = cellData.PadLeft(_columnsWidth[col], ' ');

                Console.Write(cellData);
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
    }
}
