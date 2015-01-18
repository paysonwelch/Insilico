using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Insilico {

    /// <summary>EEG-style display object</summary>
    public class EEG: BaseDisplay {
        #region Parameters
        public float min = 0;
        public float max = 1;
        public int pointCount;
        public int pointSpacing;
        public string xlabel = "x-axis";
        public string ylabel = "y-axis";
        #endregion

        #region Cached objects
        List<Line> lines = new List<Line>();
        List<Ellipse> points = new List<Ellipse>();
        #endregion
        
        /// <summary>
        /// Constructor. Designates the number of data points for the EEG
        /// </summary>
        /// <param name="numElements"></param>
        public EEG(int numElements) {
            pointCount = numElements;
        }

        public override void ComputeActiveElements() {
            oData = new float[pointCount];
            for (int i = 0; i < pointCount; i++) {
                oData[i] = 0;
                float x = xo + (requiredHorizonalMargin / 2.0f) + (i * pointSpacing);
                float y = yo + height; 
                y = float.IsNaN(y) ? 0 : y;
                if (displayLayout.bShowPoints) {
                    Ellipse newPoint = Primitives.CreateEllipse(x, 0, displayLayout.pointSize, displayLayout.pointSize, this.displayLayout.pointColor);
                    points.Add(newPoint);
                    elements.Add(newPoint);
                }

                if (i < pointCount - 1) {
                    Line line = new Line();
                    line.X1 = x;
                    line.Y1 = y;
                    line.X2 = x + pointSpacing;
                    line.Y2 = y;
                    line.Stroke = displayLayout.lineColor;
                    lines.Add(line);
                    elements.Add(line);
                }
            }
        }

        public override void ComputeMetrics() {
            pointSpacing = ((interiorWidth - (pointCount * displayLayout.pointSize)) / (pointCount-1)) + displayLayout.pointSize;
        }

        public override void ComputeDecorations() { }

        public float newestValue;

        public override void Compute() {
            // Shift data 
            for (int i = 0; i < pointCount-1; i++) {
                oData[i] = oData[i + 1];
            }
            oData[pointCount - 1] = newestValue;
            newestValue = 0;

            // Adjust lines/points
            float last_y = 0;
            float range = Math.Abs(max - min);
            for (int i = 0; i < pointCount; i++) {
                float yVal = ((oData[i] / range) * height);
                float y = yo + height - yVal - (displayLayout.pointSize / 2.0f) - +((height) / 2.0f);
                y = float.IsNaN(y) ? 0 : y;
                if(displayLayout.bShowPoints) Canvas.SetTop(points[i], y);
                if (i > 0 && i < pointCount) {
                    lines[i-1].Y1 = last_y;
                    lines[i - 1].Y2 = y; ;
                }
                last_y = y;
            }
        }
        
        public void PushDatum(float newestValue){
            this.newestValue = newestValue;
        }

        public void Step() { }
    }
}
