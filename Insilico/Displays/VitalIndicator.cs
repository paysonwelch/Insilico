using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Insilico {
    public class VitalIndicator: BaseDisplay {

        public Line primaryLine;
        List<TextBlock> labels = new List<TextBlock>();

        //int numLabels = 2;
        public double min = 0;
        public double max = 1;
        public double value = 0.7;

        private double newValue;
        private double dValue;

        public TextBlock minLabel;
        public TextBlock maxLabel;
        public TextBlock valueLabel;
        public Rectangle cursor;

        public VitalIndicator() {
            stepSize = (1.0f / (float)stepCount);
        }

        public override void ComputeMetrics() { }
        public override void ComputeActiveElements() { }

        public override void ComputeDecorations() {
            elements.Clear();

            primaryLine = new Line();
            primaryLine.X1 = xo;
            primaryLine.Y1 = yo;
            primaryLine.X2 = xo;
            primaryLine.Y2 = yo + height;
            primaryLine.Stroke = Cached.BrushWhite;
            elements.Add(primaryLine);

            float yVal = (float)((value / max) * height);
            cursor = Primitives.CreateRectangle(xo + 7, yo - yVal, 10, 10, displayLayout.pointColor);
            elements.Add(cursor);

            minLabel = Primitives.CreateTextBlock(Math.Round(min, 2) + "", Cached.typeface, 12, displayLayout.textColor, Cached.BrushTransparent, xo-10, yo-10 + height);
            elements.Add(minLabel);
            Canvas.SetZIndex(minLabel, zOrder);

            maxLabel = Primitives.CreateTextBlock(Math.Round(max, 2) + "", Cached.typeface, 12, displayLayout.textColor, Cached.BrushTransparent, xo - 10, yo);
            elements.Add(maxLabel);
            Canvas.SetZIndex(maxLabel, zOrder);

            valueLabel = Primitives.CreateTextBlock(Math.Round(value, 2) + "", Cached.typeface, 12, displayLayout.textColor, Cached.BrushTransparent, xo + 20, yo - yVal + height - 2);
            elements.Add(valueLabel);
            Canvas.SetZIndex(valueLabel, zOrder);
        }

        public override void Compute() {
            float yVal = (float)((value / max) * height);
            Canvas.SetTop(cursor, (yo - yVal) + height);
            valueLabel.Text = "" + Math.Round(value, 2);
            Canvas.SetTop(valueLabel, (yo - yVal + height) - 2);
        }

        public void SetData(double v) {
            stepsRemaining = stepCount;
            newValue = v;
            dValue = v - value;
        }

        public void Step() {
            if (stepsRemaining > 0) {
                stepsRemaining--;
                value += stepSize * dValue;
                if (value < min) value = min; // Hack to deal with floating-point errors FIXME
                if (value > max) value = max;
                value = (float)Math.Round(value, 2);
            }
        }
    }
}