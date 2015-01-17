using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Insilico {
    public class DisplayLayout {

        #region General
        public bool bUpsidedown;
        public bool bHorizontalFlip;
        public bool bShowBackground;
        public bool bShowBorder;
        public int borderThickness = 1;
        public int interiorPadding = 4;
        public bool bShowAxes;
        public bool bShowScores;
        public bool bShowValues;
        public bool bAnimated;
        public bool bShowMeanLine;

        public ValueColorScheme valueColorScheme;
        public DisplayLayout(ValueColorScheme vcs = null) {
            if (vcs == null) {
                valueColorScheme = Scheme.SimpleRYG;
            }
        }
        #endregion


        #region LinePlot
        public bool bShowPoints;
        public SolidColorBrush lineColor;
        public double lineThickness;
        public int pointSize;
        #endregion


        #region Histogram
        public SolidColorBrush backgroundColor;
        public SolidColorBrush barColor;
        public SolidColorBrush labelColor;
        public SolidColorBrush scoreColor;
        public SolidColorBrush valueColor;
        public SolidColorBrush textColor;
        public SolidColorBrush borderColor;
        public SolidColorBrush pointColor;
        #endregion

    }
}
