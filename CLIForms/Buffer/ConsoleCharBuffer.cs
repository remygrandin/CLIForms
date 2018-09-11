using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Components;
using CLIForms.Extentions;

namespace CLIForms.Buffer
{
    public class ConsoleCharBuffer
    {
        public ConsoleChar[,] data;

        public IEnumerable<PositionedConsoleChar> dataPositioned
        {
            
            get
            {
                List<PositionedConsoleChar> chars = new List<PositionedConsoleChar>();
                int xDim = data.GetLength(0);
                int yDim = data.GetLength(1);

                for (int x = 0; x < xDim; x++)
                {
                    for (int y = 0; y < yDim; y++)
                    {
                        chars.Add(new PositionedConsoleChar(data[x,y], x, y));
                    }
                }

                return chars;
            }
        }

        public int xOffset;
        public int yOffset;

        public ConsoleCharBuffer(int width, int height)
        {
            data = new ConsoleChar[width, height];
            Clear();
        }


        public ConsoleCharBuffer Clear(ConsoleChar consoleChar = null)
        {
            int xDim = data.GetLength(0);
            int yDim = data.GetLength(1);

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    data[x, y] = consoleChar;
                }
            }

            return this;
        }

        public ConsoleCharBuffer DrawString(DisplayObject owner, string str, bool focussable, int x, int y, ConsoleColor? backgroundColor, ConsoleColor foregroundColor)
        {
            if (str == null)
                return this;

            int xDim = this.data.GetLength(0);
            int yDim = this.data.GetLength(1);

            if (y >= 0 && y < yDim)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    int xPos = x + i;
                    if (xPos >= 0 && xPos < xDim)
                    {
                        if(data[xPos, y] == null)
                            data[xPos, y] = new ConsoleChar(owner, str[i], focussable, backgroundColor, foregroundColor);
                        else
                            data[xPos, y] = data[xPos, y].Merge(new ConsoleChar(owner, str[i], focussable, backgroundColor, foregroundColor));
                    }
                }

            }

            return this;
        }

        public ConsoleCharBuffer Merge(ConsoleCharBuffer slaveBuffer)
        {
            return Merge(slaveBuffer, 0, 0);
        }

        public ConsoleCharBuffer Merge(ConsoleCharBuffer slaveBuffer, int xOffset, int yOffset)
        {
            int xDimMaster = this.data.GetLength(0);
            int yDimMaster = this.data.GetLength(1);

            int xDimSlave = slaveBuffer.data.GetLength(0);
            int yDimSlave = slaveBuffer.data.GetLength(1);

            for (int x = 0; x < xDimSlave; x++)
            {
                int transposedX = x + slaveBuffer.xOffset + xOffset;
                if (transposedX >= 0 && transposedX < xDimMaster - 1)
                {
                    for (int y = 0; y < yDimSlave; y++)
                    {
                        int transposedY = y + slaveBuffer.yOffset + yOffset;
                        if (transposedY >= 0 && transposedY < yDimMaster - 1)
                        {
                            if(this.data[transposedX, transposedY] == null)
                                this.data[transposedX, transposedY] = slaveBuffer.data[x, y];
                            else
                                this.data[transposedX, transposedY] = this.data[transposedX, transposedY].Merge(slaveBuffer.data[x, y]);
                        }
                    }
                }
            }

            return this;
        }

        public List<PositionedConsoleChar> Diff(ConsoleCharBuffer secondBuffer)
        {
            int xDimB1 = this.data.GetLength(0);
            int yDimB1 = this.data.GetLength(1);

            int xDimB2 = secondBuffer.data.GetLength(0);
            int yDimB2 = secondBuffer.data.GetLength(1);

            if (xDimB1 != xDimB2 || yDimB1 != yDimB2)
                throw new Exception("buffers are of different size");

            List<PositionedConsoleChar> outList = new List<PositionedConsoleChar>();
            for (int y = 0; y < yDimB1; y++)
            {
                for (int x = 0; x < xDimB1; x++)
                {
                    if (data[x, y] != secondBuffer.data[x, y])
                        outList.Add(new PositionedConsoleChar(secondBuffer.data[x, y], x, y));
                }
            }

            return outList;
        }

        public static void Display(List<PositionedConsoleChar> chars)
        {

            var result = chars.GroupBy(item =>
                new
                {
                    item.Y,
                    item.Background,
                    item.Foreground
                });

            foreach (var lineChars in result)
            {
                var groupped = lineChars.GroupAdjacentBy((x1, x2) => x1.X + 1 == x2.X);

                foreach (var subGroup in groupped)
                {
                    var orderedSubGroup = subGroup.OrderBy(item => item.X);

                    var firstChar = orderedSubGroup.First();
                    string groupStr = new string(orderedSubGroup.Select(item => item.Char).ToArray());

                    Console.SetCursorPosition(firstChar.X, firstChar.Y);
                    Console.BackgroundColor = firstChar.Background ?? ConsoleColor.Black;
                    Console.ForegroundColor = firstChar.Foreground;
                    Console.Write(groupStr);

                }

            }

            /*
            foreach (PositionedConsoleChar ch in chars)
            {
                Console.SetCursorPosition(ch.X, ch.Y);
                Console.BackgroundColor = ch.Background;
                Console.ForegroundColor = ch.Foreground;
                Console.Write(ch.Char);
            }
            */
        }
    }
}
