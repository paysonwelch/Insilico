using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Insilico {
    public class Lattice : Display {
        #region Parameters
        public float[] data;
        public int xo;
        public int yo;
        public int width = 800;
        public int height = 400;
        public int leftMargin = 10;
        public int rightMargin = 10;
        public int topMargin = 10;
        public int bottomMargin = 10;
        public float opacity = 0.5f;

        public int elements;

        public int barSpacing = 10;
        public float stepCount = 20;
        public float stepsRemaining = 0;

        public string xlabel = "x-axis";
        public string ylabel = "y-axis";
        #endregion

        #region Cached objects
        public List<Rectangle> bars = new List<Rectangle>();
        #endregion

        public Lattice(int numElements) {
            elements = numElements;
            data = new float[elements];
            for (int i = 0; i < elements; i++) {
                data[i] = 0;
            }
        }

        public override void ComputeMetrics() { }
        public override void ComputeActiveElements() { }
        public override void ComputeDecorations() { }

        public override void Compute() {
            if (data != null && data.Length > 0) {
                bars.Clear();
                float barWidthMax = (width - (leftMargin + rightMargin)) / (data.Length);
                float barHeightMax = height - (topMargin + bottomMargin);

                float max = data.Max();
                max = float.IsNaN(max) ? 1 : max;
                float min = data.Min();

                for (int i = 0; i < data.Count(); i++) {
                    float x = (i * (barWidthMax + barSpacing));
                    float y = 50;
                    float thisBarHeight = (float)((data[i] / max) * 100.0);
                    thisBarHeight = float.IsNaN(thisBarHeight) ? 1 : thisBarHeight;

                    //bars.Add(Helpers.GenerateNewRectangle(x + xo, y + yo, barWidthMax, thisBarHeight, Shared.BrushLimeGreen, opacity, false, ""));
                }
            }
        }

        public float[] nData;
        public float[] dData;

        public bool SetData(float[] newData) {
            if (data != null && data.Length == newData.Length) {
                this.nData = newData;
                dData = data.Zip(nData, (a, b) => (b - a)).ToArray();
                stepsRemaining = stepCount;
                return true;
            }
            return false;
        }

        public void Step() {
            if (data != null && dData != null) {
                if (stepsRemaining > 0) {
                    stepsRemaining--;
                    float stepSize = (1.0f / (float)stepCount);
                    for (int i = 0; i < data.Length; i++) {
                        data[i] += stepSize * dData[i];
                        data[i] = (float)Math.Round(data[i], 2);
                    }
                }
            }
        }
    }
}
