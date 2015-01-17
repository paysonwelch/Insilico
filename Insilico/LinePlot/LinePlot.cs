using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Insilico {
    public class LinePlot: Display {
        #region Parameters

        public float min = 0;
        public float max = 100;
        public float opacity = 1.0f;
        public int pointCount;

        public string xlabel = "x-axis";
        public string ylabel = "y-axis";
        #endregion

        #region Cached objects
        public Rectangle randomFuckingPoint;
        public List<Line> lines = new List<Line>();
        public List<Ellipse> points = new List<Ellipse>();
        #endregion

        public LinePlot(int numElements) {
            pointCount = numElements;
            oData = new float[pointCount];
            for (int i = 0; i < pointCount; i++) { oData[i] = 0; }
            if (this.displayLayout.bShowBackground) { ComputeDecorations(); }
        }

        public override void ComputeActiveElements() { }
        public override void ComputeDecorations() { }

        public Line meanLine;
        public TextBlock meanLineTB;

        public override void ComputeMetrics() { // FIXME meanLine will be slow and should be computed by a displacement from the last mean as opposed to a full re-computation
            if (displayLayout.bShowMeanLine)
            {
                if (meanLine == null) {
                    meanLine = new Line();
                    meanLine.Stroke = displayLayout.lineColor;
                    meanLine.StrokeThickness = 2;
                    elements.Add(meanLine);
                    Canvas.SetZIndex(meanLine, zOrder);
                }
                else {
                    float yVal = (oData.Average() / oData.Max())* height;
                    meanLine.X1 = xo;
                    meanLine.Y1 = yo + yVal;
                    meanLine.X2 = xo + width;
                    meanLine.Y2 = yo + yVal;

                    if (meanLineTB == null) {
                        meanLineTB = Primitives.GenerateTextBlock(Math.Round(yVal, 2) + "", Cached.typeface, 12, displayLayout.textColor, Cached.BrushTransparent, 0, 0);
                        elements.Add(meanLineTB);
                        Canvas.SetZIndex(meanLineTB, zOrder);
                    }
                    else {
                        meanLineTB.Text = Math.Round(yVal, 2) + "";
                        Canvas.SetLeft(meanLineTB, xo+width+5);
                        Canvas.SetTop(meanLineTB, yo+yVal-7);
                    }
                }
            }
        }

        public override void Compute() {
            if (oData != null && oData.Length > 0) {
                float barWidthMax = (width - (leftMargin + rightMargin)) / (oData.Length-1);
                float barHeightMax = height - (topMargin + bottomMargin);
                float max = oData.Max();
                max = float.IsNaN(max) ? 1 : max;
                float min = oData.Min();

                for (int i = 0; i < oData.Count(); i++) {

                    float xcurr = xo + i * (barWidthMax);
                    float ycurr = max == 0 ? yo - 0 : yo + (oData[i] / max) * height;

                    if (lines.Count() < pointCount - 1) {
                        Line line = new Line();
                        if (i > 0) {
                            line.X1 = xo + (i - 1) * (barWidthMax);
                            line.Y1 = max == 0 ? yo - 0 : yo + (oData[i - 1] / max) * height;
                            line.X2 = xcurr;
                            line.Y2 = ycurr;
                            line.Stroke = displayLayout.lineColor;
                            lines.Add(line);
                            elements.Add(line);
                            
                        }
                        Ellipse point = Primitives.FastEllipse(0, 0, displayLayout.pointSize, displayLayout.pointSize, displayLayout.lineColor, 1.0);
                        points.Add(point);
                        elements.Add(point);
                    }
                    else{
                        if (i < pointCount - 1) {
                            lines[i].X1 = xcurr;
                            lines[i].Y1 = ycurr;
                            lines[i].X2 = xo + (i+1) * (barWidthMax);
                            lines[i].Y2 = max == 0 ? yo - 0 : yo + (oData[i + 1] / max) * height;
                            lines[i].Stroke = displayLayout.lineColor;
                        }
                        Canvas.SetLeft(points[i], xcurr);
                        Canvas.SetTop(points[i], ycurr);
                    }
                }
                ComputeMetrics();
            }
        }

        /// <summary>
        /// Provides the histogram with new data to represent
        /// </summary>
        public bool SetData(float[] newData) {
            if (oData != null && oData.Length == newData.Length) {
                this.nData = newData;
                dData = oData.Zip(nData, (a, b) => (b - a)).ToArray();
                stepsRemaining = stepCount;
                return true;
            }
            return false;
        }

        float prev;
        public void Step() {
            if (oData != null && dData != null) {
                if (stepsRemaining > 0) {
                    stepsRemaining--;
                    for (int i = 0; i < oData.Length; i++) {
                        prev = oData[i];
                        oData[i] += stepSize * dData[i];
                        if (oData[i] < min) oData[i] = min; // Hack to deal with floating-point errors FIXME
                        if (oData[i] > max) oData[i] = max;
                        oData[i] = (float)Math.Round(oData[i], 2);
                    }
                }
            }
        }
    }
}
