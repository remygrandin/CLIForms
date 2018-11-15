using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLIForms_FIGFonts
{
    public class FIGBuffer
    {
        private static IEnumerable<T> Flatten<T>(T[,] map)
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    yield return map[row, col];
                }
            }
        }

        public FIGSubChar[,] data;

        public IEnumerable<FIGSubChar> dataPositioned
        {
            get { return FIGBuffer.Flatten(this.data); }
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

        public FIGBuffer(int width, int height)
        {
            data = new FIGSubChar[width, height];
            Clear();
        }

        public FIGBuffer(FIGSubChar[,] array)
        {
            data = array;
            Clear();
        }

        public int Width
        {
            get => data.GetLength(0);
            set
            {
                if (value != Width)
                {
                    Resize(value, Height);
                }
            }
        }

        public int Height
        {
            get => data.GetLength(1);
            set
            {
                if (value != Height)
                {
                    Resize(Width, value);
                }
            }
        }

        public void Clear(char ch = ' ')
        {
            foreach (FIGSubChar sub in Flatten(data))
            {
                sub.Char = ch;
            }
        }

        public void Replace(char oldCh, char newCh)
        {
            foreach (FIGSubChar sub in Flatten(data).Where(item => item.Char == oldCh))
            {
                sub.Char = newCh;
            }

        }

        public IList<int> GetLeftSpaceMask()
        {
            int xDim = Width;
            int yDim = Height;

            List<int> mask = new List<int>();

            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    if (data[x, y].Char != ' ')
                    {
                        mask.Add(x);
                        break;
                    }
                }
            }

            return mask;
        }

        public IList<int> GetRightSpaceMask()
        {
            int xDim = Width;
            int yDim = Height;

            List<int> mask = new List<int>();

            for (int y = 0; y < yDim; y++)
            {
                for (int x = xDim - 1; x > 0; x--)
                {
                    if (data[x, y].Char != ' ')
                    {
                        mask.Add(xDim - 1 - x);
                        break;
                    }
                }
            }

            return mask;
        }

        public void Resize(int width, int height)
        {
            FIGSubChar[,] newData = new FIGSubChar[width, height];

            int xDim = Width;
            int yDim = Height;

            int xDimNew = newData.GetLength(0);
            int yDimNew = newData.GetLength(1);

            for (int x = 0; x < xDimNew; x++)
            {
                for (int y = 0; y < yDimNew; y++)
                {
                    newData[x, y] = new FIGSubChar(' ', x, y);
                }
            }

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (x < xDimNew && y < yDimNew)
                        newData[x, y] = data[x, y];
                }
            }
        }

        public bool TestHorizMerge(FIGBuffer secondaryBuffer, FullLayoutEnum layout, char hardBlank, IList<Tuple<int, int>> smushPoints = null,
            int xOffset = 0, int yOffset = 0)
        {
            if (layout.HasFlag(FullLayoutEnum.Horz_Smush) &&
                !layout.HasFlag(FullLayoutEnum.Horz_Smush_R1) &&
                !layout.HasFlag(FullLayoutEnum.Horz_Smush_R2) &&
                !layout.HasFlag(FullLayoutEnum.Horz_Smush_R3) &&
                !layout.HasFlag(FullLayoutEnum.Horz_Smush_R4) &&
                !layout.HasFlag(FullLayoutEnum.Horz_Smush_R5) &&
                !layout.HasFlag(FullLayoutEnum.Horz_Smush_R6)) // universal smushing
                return true;

            if (smushPoints == null)
                return true;

            foreach (Tuple<int, int> smushPoint in smushPoints)
            {
                char pChar = data[smushPoint.Item1, smushPoint.Item2].Char;

                char sChar = secondaryBuffer.data[smushPoint.Item1 - xOffset, smushPoint.Item2 - yOffset].Char;


                if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R1)) // EQUAL CHARACTER SMUSHING
                {
                    if (pChar == sChar)
                        continue;
                }

                if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R2)) // UNDERSCORE SMUSHING 
                {
                    if (pChar == '_' && "|/\\[]{}()<>".Contains(sChar))
                        continue;

                    if (sChar == '_' && "|/\\[]{}()<>".Contains(pChar))
                        continue;
                }

                if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R3)) // HIERARCHY SMUSHING
                {
                    string[] classes = new[] { "|", "/\\", "[]", "{}", "()", "<>" };

                    bool found = false;

                    for (int i = 0; i < classes.Length - 1; i++)
                    {
                        if (classes[i].Contains(pChar) && String.Join("", classes.Skip(i + 1)).Contains(sChar))
                        {
                            found = true;
                            break;
                        }
                        if (classes[i].Contains(sChar) && String.Join("", classes.Skip(i + 1)).Contains(pChar))
                        {
                            found = true;
                            break;
                        }
                    }

                    if(found)
                        continue;
                }


                if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R4)) // OPPOSITE PAIR SMUSHING
                {
                    string[] classes = new[] { "[]", "{}", "()" };

                    bool found = false;

                    foreach (string cls in classes)
                    {
                        if ((cls[0] == pChar && cls[1] == sChar) || (cls[0] == sChar && cls[1] == pChar))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        continue;
                }

                if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R5)) // BIG X SMUSHING
                {
                    if (pChar == '/' && sChar == '\\')
                        continue;
                    if (pChar == '\\' && sChar == '/')
                        continue;
                    if (pChar == '>' && sChar == '<')
                        continue;
                }

                if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R6)) // HARDBLANK SMUSHING
                {
                    if (pChar == sChar && pChar == hardBlank)
                        continue;
                }

                return false;
            }

            return true;
        }

        public void HorizMerge(FIGBuffer secondaryBuffer, FullLayoutEnum layout, char hardBlank, IList<Tuple<int, int>> smushPoints, int xOffset = 0, int yOffset = 0)
        {
            int xDim = Width;
            int yDim = Height;

            int xDimSec = secondaryBuffer.data.GetLength(0);
            int yDimSec = secondaryBuffer.data.GetLength(1);


            for (int x = 0; x < xDimSec; x++)
            {
                int mainX = x + xOffset;

                if (mainX < 0 || mainX >= xDim)
                    continue;

                for (int y = 0; y < yDimSec; y++)
                {
                    int mainY = y + yOffset;

                    if (mainY < 0 || mainY >= yDim)
                        continue;

                    if (smushPoints == null || !smushPoints.Any(item => item.Item1 == mainX && item.Item2 == mainY))
                    {
                        data[mainX, mainY].Char = secondaryBuffer.data[x, y].Char;
                        continue;

                    }

                    char primaryChar = data[mainX, mainY].Char;

                    char secondaryChar = secondaryBuffer.data[x, y].Char;

                    char resultingChar = secondaryChar == ' ' ? primaryChar : secondaryChar; // default to universal smushing


                    if (layout.HasFlag(FullLayoutEnum.Horz_Smush))
                    {
                        resultingChar = HSmush(primaryChar, secondaryChar, layout, hardBlank);

                    }


                    data[mainX, mainY].Char = resultingChar;
                }
            }
        }

        private char HSmush(char pChar, char sChar, FullLayoutEnum layout, char hardBlank)
        {
            if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R1)) // EQUAL CHARACTER SMUSHING
            {
                if (pChar == sChar)
                    return pChar;
            }

            if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R2)) // UNDERSCORE SMUSHING 
            {
                if (pChar == '_' && "|/\\[]{}()<>".Contains(sChar))
                    return sChar;

                if (sChar == '_' && "|/\\[]{}()<>".Contains(pChar))
                    return pChar;
            }

            if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R3)) // HIERARCHY SMUSHING
            {
                string[] classes = new[] { "|", "/\\", "[]", "{}", "()", "<>" };

                for (int i = 0; i < classes.Length - 1; i++)
                {
                    if (classes[i].Contains(pChar) && String.Join("", classes.Skip(i + 1)).Contains(sChar))
                        return sChar;
                    if (classes[i].Contains(sChar) && String.Join("", classes.Skip(i + 1)).Contains(pChar))
                        return pChar;
                }
            }


            if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R4)) // OPPOSITE PAIR SMUSHING
            {
                string[] classes = new[] { "[]", "{}", "()" };

                foreach (string cls in classes)
                {
                    if ((cls[0] == pChar && cls[1] == sChar) || (cls[0] == sChar && cls[1] == pChar))
                        return '|';
                }
            }

            if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R5)) // BIG X SMUSHING
            {
                if (pChar == '/' && sChar == '\\')
                    return '|';
                if (pChar == '\\' && sChar == '/')
                    return 'Y';
                if (pChar == '>' && sChar == '<')
                    return 'X';
            }

            if (layout.HasFlag(FullLayoutEnum.Horz_Smush_R6)) // HARDBLANK SMUSHING
            {
                if (pChar == sChar && pChar == hardBlank)
                    return pChar;
            }

            throw new Exception("Un-smushable chars");
            return ' ';
        }
    }
}