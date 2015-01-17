using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Insilico {
    public class Histogram : BaseDisplay {
        #region Parameters
        public float min = 0;
        public float max = 50;
        public int pointCount;
        public int barSpacing = 2;
        public string xlabel = "x-axis";
        public string ylabel = "y-axis";
        float barWidthMax;
        float barHeightMax;
        #endregion

        #region Cached objects
        // Even though all UIElements are stored in Display.elements, we maintain this list for speed's sake
        public List<Rectangle> bars = new List<Rectangle>();
        #endregion

        public Histogram(int numElements) {
            pointCount = numElements;
            oData = new float[pointCount];
            for (int i = 0; i < pointCount; i++) { oData[i] = 0; }
            Compute();
        }

        public override void ComputeMetrics() {
            widthConsumed = (barSpacing * (oData.Length - 1));
            widthRemaining = interiorWidth - widthConsumed;
            barWidthMax = widthRemaining / oData.Length;
            heightConsumed = 0;
            heightRemaining = interiorHeight - heightConsumed;
            barHeightMax = heightRemaining;
        }

        public override void ComputeActiveElements() {
            bars.Clear();
            for (int i = 0; i < oData.Count(); i++) {
                float x = (i * (barWidthMax + barSpacing)) + (requiredHorizonalMargin / 2.0f);
                float y = 0;
                float percentage = (float)(oData[i] / max);
                float thisBarHeight = percentage * barHeightMax;
                //SolidColorBrush barColor = displayLayout.valueColorScheme.GetColor(percentage);
                //barColor = barColor == null ? displayLayout.barColor : barColor;
                thisBarHeight = float.IsNaN(thisBarHeight) ? 1 : thisBarHeight;
                if (bars.Count() != pointCount) {
                    Rectangle newBar = Primitives.CreateRectangle(xo + x, y + height, barWidthMax, 1, displayLayout.barColor);
                    elements.Add(newBar);
                    Canvas.SetZIndex(newBar, zOrder);
                    bars.Add(newBar);
                }
            }
        }

        public override void ComputeDecorations() { }

        public override void Compute() { 
            if (oData != null && oData.Length > 0) {
                float max = oData.Max();
                max = float.IsNaN(max) ? 1 : max;
                float min = oData.Min();

                for (int i = 0; i < oData.Count(); i++) {
                    float x = (i * (barWidthMax + barSpacing));
                    float percentage = (float)(oData[i] / max);
                    float thisBarHeight = percentage * barHeightMax;
                    //SolidColorBrush barColor = displayLayout.valueColorScheme.GetColor(percentage);
                    //barColor = barColor == null ? displayLayout.barColor : barColor;
                    thisBarHeight = float.IsNaN(thisBarHeight) ? 1 : thisBarHeight;

                    if (bars.Count() != pointCount) {
                        Rectangle newBar = Primitives.CreateRectangle(xo + x, yo + height - this.displayLayout.interiorPadding * 2, barWidthMax, 1, displayLayout.barColor);
                        elements.Add(newBar);
                        Canvas.SetZIndex(newBar, zOrder);
                        bars.Add(newBar);
                    }
                    else { 
                        bars[i].Height = thisBarHeight;
                        Canvas.SetTop(bars[i], yo - thisBarHeight + height - this.displayLayout.interiorPadding * 2);
                    }
                }
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
