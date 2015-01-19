using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insilico {

    public static partial class Layouts {

        #region Default
        public static DisplayLayout DefaultLayout = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = true,
            bShowScores = true,
            bShowValues = true,
            bShowBorder = true,
            bUpsidedown = false,
            bShowPoints = false,
            bShowMeanLine = true,

            pointSize = 8,

            backgroundColor = Cached.BrushGray,
            lineColor = Cached.BrushWhite,
            lineThickness = 1,
            labelColor = Cached.BrushWhite,
            valueColor = Cached.BrushWhite,
            scoreColor = Cached.BrushWhite,
            textColor = Cached.BrushWhite,
            barColor = Cached.BrushWhite,
            pointColor = Cached.BrushWhite
        };

        public static DisplayLayout Spartan = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = false,
            bShowScores = true,
            bShowValues = true,
            bShowBorder = true,
            bUpsidedown = false,
            bShowPoints = false,
            bShowMeanLine = true,

            pointSize = 8,

            backgroundColor = Cached.BrushTransparent,
            lineColor = Cached.BrushWhite,
            lineThickness = 1,
            labelColor = Cached.BrushWhite,
            valueColor = Cached.BrushWhite,
            scoreColor = Cached.BrushWhite,
            textColor = Cached.BrushWhite,
            barColor = Cached.BrushWhite,
            pointColor = Cached.BrushWhite
        };

        public static DisplayLayout RadioactiveChocolate = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = true,
            bShowScores = true,
            bShowValues = true,
            bShowBorder = true,
            bUpsidedown = false,
            bShowPoints = false,
            bShowMeanLine = true,

            pointSize = 8,

            backgroundColor = Cached.BrushGray,
            lineColor = Cached.BrushLimeGreen,
            lineThickness = 1,
            labelColor = Cached.BrushLimeGreen,
            valueColor = Cached.BrushLimeGreen,
            scoreColor = Cached.BrushLimeGreen,
            textColor = Cached.BrushLimeGreen,
            barColor = Cached.BrushLimeGreen,
            pointColor = Cached.BrushLimeGreen
        };

        #endregion

        #region Histogram
        public static DisplayLayout HistogramLimeGreen = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = true,
            bShowBorder = true,
            bShowScores = true,
            bShowValues = true,
            bUpsidedown = false,

            backgroundColor = Cached.BrushLimeGreen,
            barColor = Cached.BrushLimeGreen,
            labelColor = Cached.BrushLimeGreen,
            valueColor = Cached.BrushLimeGreen,
            scoreColor = Cached.BrushLimeGreen,
            textColor = Cached.BrushWhite,
            borderColor = Cached.BrushNeonGreen
        };
        #endregion


        #region LinePlot
        public static DisplayLayout LinePlotLimeGreen = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = true,
            bShowScores = true,
            bShowValues = true,
            bUpsidedown = false,
            bShowPoints = false,
            bShowMeanLine = true,

            pointSize = 8,

            backgroundColor = Cached.BrushLimeGreen,
            lineColor = Cached.BrushLimeGreen,
            lineThickness = 1,
            labelColor = Cached.BrushLimeGreen,
            valueColor = Cached.BrushLimeGreen,
            scoreColor = Cached.BrushLimeGreen,
            textColor = Cached.BrushWhite
        };

        public static DisplayLayout LinePlotBlue = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = true,
            bShowScores = true,
            bShowValues = true,
            bUpsidedown = false,
            bShowPoints = true,
            bShowMeanLine = false,

            pointSize = 8,

            backgroundColor = Cached.BrushDodgerBlue,
            lineColor = Cached.BrushDodgerBlue,
            lineThickness = 1,
            labelColor = Cached.BrushDodgerBlue,
            valueColor = Cached.BrushDodgerBlue,
            scoreColor = Cached.BrushDodgerBlue,
            textColor = Cached.BrushDodgerBlue
        };

        public static DisplayLayout LinePlotRed = new DisplayLayout {
            bAnimated = true,
            bHorizontalFlip = false,
            bShowAxes = true,
            bShowBackground = true,
            bShowScores = true,
            bShowValues = true,
            bUpsidedown = false,
            bShowPoints = true,
            bShowMeanLine = false,

            pointSize = 4,

            backgroundColor = Cached.BrushDarkRed,
            lineColor = Cached.BrushRed,
            lineThickness = 1,
            labelColor = Cached.BrushDarkRed,
            valueColor = Cached.BrushDarkRed,
            scoreColor = Cached.BrushDarkRed,
            textColor = Cached.BrushDarkRed
        };
        #endregion
    }
}
