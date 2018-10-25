using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                int xDim = Width;
                int yDim = Height;

                for (int x = 0; x < xDim; x++)
                {
                    for (int y = 0; y < yDim; y++)
                    {
                        chars.Add(new PositionedConsoleChar(data[x, y], x, y));
                    }
                }

                return chars;
            }
        }

        public string StringRender
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int xDim = Width;
                int yDim = Height;

                for (int y = 0; y < yDim; y++)
                {
                    for (int x = 0; x < xDim; x++)
                    {

                        if (data[x, y] == null)
                            sb.Append(" ");
                        else
                            sb.Append(data[x, y].Char);
                    }
                    sb.Append("\r\n");
                }

                return sb.ToString();
            }
        }

        public int xOffset;
        public int yOffset;

        public ConsoleCharBuffer(int width, int height)
        {
            data = new ConsoleChar[width, height];
            Clear();
        }

        public int Width
        {
            get => data.GetLength(0);
        }

        public int Height
        {
            get => data.GetLength(1);
        }

        public ConsoleCharBuffer Clear(ConsoleChar consoleChar = null)
        {
            int xDim = Width;
            int yDim = Height;

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

            int xDim = Width;
            int yDim = Height;

            if (y >= 0 && y < yDim)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    int xPos = x + i;
                    if (xPos >= 0 && xPos < xDim)
                    {
                        if (data[xPos, y] == null)
                            data[xPos, y] = new ConsoleChar(owner, str[i], focussable, backgroundColor, foregroundColor);
                        else
                            data[xPos, y] = data[xPos, y].Merge(new ConsoleChar(owner, str[i], focussable, backgroundColor, foregroundColor));
                    }
                }

            }

            return this;
        }

        public ConsoleCharBuffer Merge(ConsoleCharBuffer slaveBuffer, int xOffset = 0, int yOffset = 0)
        {
            int xDimMaster = this.Width;
            int yDimMaster = this.Height;

            int xDimSlave = slaveBuffer.Width;
            int yDimSlave = slaveBuffer.Height;

            for (int x = 0; x < xDimSlave; x++)
            {
                int transposedX = x + slaveBuffer.xOffset + xOffset;
                if (transposedX >= 0 && transposedX < xDimMaster)
                {
                    for (int y = 0; y < yDimSlave; y++)
                    {
                        int transposedY = y + slaveBuffer.yOffset + yOffset;
                        if (transposedY >= 0 && transposedY < yDimMaster)
                        {
                            if (this.data[transposedX, transposedY] == null)
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
            int xDimB1 = this.Width;
            int yDimB1 = this.Height;

            int xDimB2 = secondBuffer.Width;
            int yDimB2 = secondBuffer.Height;

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
    }
}
