using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Styles;

namespace CLIForms.Components.Chart
{
    public class HorizontalChart : DisplayObject
    {
        public ConsoleColor AxesForegroundColor = ConsoleColor.Black;
        public ConsoleColor[] DataForegroundColor = new[] { ConsoleColor.White };

        private ChartType _chartType = ChartType.Line;
        public ChartType ChartType
        {
            get { return _chartType; }
            set
            {
                if (_chartType != value)
                {
                    _chartType = value;
                    Dirty = true;
                }
            }
        }


        private int? _height;
        public int? Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    Dirty = true;
                }
            }
        }

        private float? _minY = 0;
        public float? MinY
        {
            get { return _minY; }
            set
            {
                if (_minY != value)
                {
                    _minY = value;
                    Dirty = true;
                }
            }
        }

        private float? _maxY;
        public float? MaxY
        {
            get { return _maxY; }
            set
            {
                if (_maxY != value)
                {
                    _maxY = value;
                    Dirty = true;
                }
            }
        }

        private bool _showYMinMax = true;
        public bool ShowYMinMax
        {
            get { return _showYMinMax; }
            set
            {
                if (_showYMinMax != value)
                {
                    _showYMinMax = value;
                    Dirty = true;
                }
            }
        }

        private int? _maxData;
        public int? MaxData
        {
            get { return _maxData; }
            set
            {
                if (_maxData != value)
                {
                    _maxData = value;

                    if (_maxData != null)
                    {
                        if (Data.Count > _maxData)
                            Data = Data.Take(_maxData.Value).ToList();
                    }

                    Dirty = true;
                }
            }
        }


        public HorizontalChart(Container parent, IList<float> data, int? maxData = null) : base(parent)
        {
            Data = data.ToList();
            MaxData = maxData;
        }

        private List<float> Data;

        public HorizontalChart AppendData(int data)
        {
            Data.Add(data);
            if (_maxData != null && Data.Count > _maxData)
                Data = Data.Take(_maxData.Value).ToList();

            return this;
        }

        public object templock = new object();


        public override ConsoleCharBuffer Render()
        {
            lock (templock)
            {


                if (!_dirty && displayBuffer != null)
                    return displayBuffer;


                int width = 1 + Data.Count;
                int height;

                float trueMin = MinY ?? Data.Min();
                float trueMax = MaxY ?? Data.Max();

                if (Height.HasValue)
                {
                    height = Height.Value;
                }
                else
                {
                    height = (int)Math.Ceiling(Data.Max() - Data.Min()) + 1;
                }

                int labelLength = 0;

                if (ShowYMinMax)
                {
                    labelLength = Math.Max(trueMin.ToString().Length, trueMax.ToString().Length);
                    width += labelLength;
                }


                ConsoleCharBuffer buffer = new ConsoleCharBuffer(width, height);

                // Axes
                for (int y = 0; y < buffer.Height; y++)
                {
                    buffer.data[labelLength, y] = new ConsoleChar(this,
                        DrawingHelper.GetVerticalBorder(BorderStyle.Thin)[0], false, null, AxesForegroundColor);
                }

                buffer.DrawString(this, new string(DrawingHelper.GetHorizontalBorder(BorderStyle.Thin)[0], width),
                    false, labelLength, height - 1, null, AxesForegroundColor);

                buffer.data[labelLength, height - 1].Char =
                    DrawingHelper.GetBottomLeftCornerBorder(BorderStyle.Thin)[0];

                if (ShowYMinMax)
                {
                    buffer.data[labelLength, 0].Char = DrawingHelper.GetBottomTJunctionBorder(BorderStyle.Thin)[0];
                    buffer.data[labelLength, height - 1].Char =
                        DrawingHelper.GetTopTJunctionBorder(BorderStyle.Thin)[0];

                    buffer.DrawString(this, trueMax.ToString(), false, labelLength - trueMax.ToString().Length, 0, null,
                        AxesForegroundColor);
                    buffer.DrawString(this, trueMin.ToString(), false, labelLength - trueMin.ToString().Length,
                        height - 1, null, AxesForegroundColor);
                }



                int pointRange = (int)Math.Ceiling(trueMax - trueMin);
                int pointOffset = (int)Math.Round(trueMin);

                int displayRange = (height - 1) * 2;
                int origin = (int)Math.Round(pointOffset / pointRange * displayRange * 1.0);

                for (var pointIdx = 0; pointIdx < Data.Count; pointIdx++)
                {
                    float point = Data[pointIdx];

                    int rounded = (int)Math.Round((point - pointOffset) / pointRange * displayRange);

                    if (ChartType == ChartType.Line)
                    {
                        if (rounded == origin)
                        {
                            buffer.data[pointIdx + labelLength + 1, height - 1] = new ConsoleChar(this,
                                DrawingHelper.GetHorizontalBorder(BorderStyle.Thin)[0], false, null,
                                DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                        }
                        else
                        {
                            buffer.data[pointIdx + labelLength + 1, height - 1] = new ConsoleChar(this,
                                DrawingHelper.GetTopTJunctionBorder(BorderStyle.Thin)[0], false, null,
                                DataForegroundColor[pointIdx % DataForegroundColor.Length]);

                            for (int i = 0; i < rounded - 1; i += 2)
                            {
                                buffer.data[pointIdx + labelLength + 1, height - 2 - (i / 2)] = new ConsoleChar(this,
                                    DrawingHelper.GetVerticalBorder(BorderStyle.Thin)[0], false, null,
                                    DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                            }

                            if (rounded % 2 == 0)
                            {
                                buffer.data[pointIdx + labelLength + 1, height - 1 - (rounded / 2)] = new ConsoleChar(
                                    this, DrawingHelper.GetTopRightCornerBorder(BorderStyle.Thin)[0], false, null,
                                    DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                            }
                        }
                    }
                    else if (ChartType == ChartType.Bar)
                    {
                        if (rounded == origin)
                        {
                            buffer.data[pointIdx + labelLength + 1, height - 1] = new ConsoleChar(this,
                                DrawingHelper.GetHorizontalBorder(BorderStyle.Thin)[0], false, null,
                                DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                        }
                        else
                        {
                            buffer.data[pointIdx + labelLength + 1, height - 1] = new ConsoleChar(this,
                                DrawingHelper.GetBottomLeftCornerBorder(BorderStyle.Block)[0], false, null,
                                DataForegroundColor[pointIdx % DataForegroundColor.Length]);

                            for (int i = 0; i < rounded - 1; i += 2)
                            {
                                buffer.data[pointIdx + labelLength + 1, height - 2 - (i / 2)] = new ConsoleChar(this,
                                    DrawingHelper.GetVerticalBorder(BorderStyle.Block)[0], false, null,
                                    DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                            }

                            if (rounded % 2 == 0)
                            {
                                buffer.data[pointIdx + labelLength + 1, height - 1 - (rounded / 2)] = new ConsoleChar(
                                    this, DrawingHelper.GetTopRightCornerBorder(BorderStyle.Block)[0], false, null,
                                    DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                            }
                        }
                    }
                    else if (ChartType == ChartType.Point)
                    {
                        buffer.data[pointIdx + labelLength + 1, height - 1 - rounded / 2] = new ConsoleChar(this,
                            '*', false, null, DataForegroundColor[pointIdx % DataForegroundColor.Length]);
                    }
                }

                Dirty = false;

                return buffer;
            }
        }
    }
}
