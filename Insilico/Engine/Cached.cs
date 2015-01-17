using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Insilico {
    public static partial class Cached {

        #region Vars
        public static Random rnd = new Random();
        public static Point lastPoint;
        public static Point lastOffset;
        public static Point lastClickPoint;
        public static Point dragStartPoint;
        public static Point dragEndPoint;
        public static Point rightDragPrevOffset;
        public static Point leftDragPrevOffset;
        public static Point ZeroPoint = new Point(0, 0);
        public static Vertex lastClickedVertex;
        public static Graph graph;
        public static float edgeLengthFudge = 1.0f;
        public static float arcLengthSpacingFudge = 10.0f;
        public static float nodeRadiusFudge = 1.0f;
        public static float arrowPercentage = 0.5f;
        public static DateTime startTime = DateTime.Now;
        public static bool stickyDrag = false;
        public static bool rightClicked = false;

        public static float maxX;
        public static float maxY;
        public static float minX;
        public static float minY;

        #endregion

        #region Cached Objects (improves rendering performance)
        public static Typeface typeface = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal); 

        // UIElements
        public static List<Border> cachedBorders = new List<Border>();
        public static List<Line> cachedLines = new List<Line>();
        public static List<Grid> cachedGrids = new List<Grid>();
        public static CornerRadius StandardCornerRadius = new CornerRadius(5);
        public static int borderThickness = 4;
        public static Thickness StandardBorderThickness = new Thickness(4, 4, 4, 4);
        public static DoubleCollection StandardDashedLine = new DoubleCollection() { 1, 1 };

        // Standard Aigo colors
        public static SolidColorBrush BrushSysNode_ = new SolidColorBrush(Color.FromArgb(255, 255, 136, 255));
        public static SolidColorBrush BrushError = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush BrushWordNode = new SolidColorBrush(Colors.White);
        public static SolidColorBrush BrushNameNode = new SolidColorBrush(Colors.LightGray);
        public static SolidColorBrush BrushRelNode = new SolidColorBrush(Color.FromArgb(255, 175, 238, 236));
        public static SolidColorBrush BrushRuleNode = new SolidColorBrush(Colors.Gray);
        public static SolidColorBrush BrushArtDetNode_ = new SolidColorBrush(Color.FromArgb(255, 255, 235, 205));
        public static SolidColorBrush BrushNounNode_ = new SolidColorBrush(Color.FromArgb(255, 106, 192, 255));
        public static SolidColorBrush BrushverbNode_ = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0));
        public static SolidColorBrush BrushAdverbNode_ = new SolidColorBrush(Color.FromArgb(255, 255, 192, 203));
        public static SolidColorBrush BrushPrepNode_ = new SolidColorBrush(Color.FromArgb(255, 181, 151, 236));
        public static SolidColorBrush BrushPronounNode_ = new SolidColorBrush(Color.FromArgb(255, 175, 238, 238));
        public static SolidColorBrush BrushTASNode = new SolidColorBrush(Colors.Brown);
        public static SolidColorBrush BrushAdjNode_ = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
        public static SolidColorBrush BrushConjNode_ = new SolidColorBrush(Color.FromArgb(255, 173, 255, 47));
        public static SolidColorBrush BrushauxiliaryNode_ = new SolidColorBrush(Color.FromArgb(255, 192, 192, 192));

        // Misc brushes
        public static SolidColorBrush BrushTransparent = new SolidColorBrush(Colors.Transparent);
        public static SolidColorBrush BrushWhite = new SolidColorBrush(Colors.White);
        public static SolidColorBrush BrushBlack = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush BrushBlue = new SolidColorBrush(Colors.Blue);
        public static SolidColorBrush BrushRed = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush BrushGreen = new SolidColorBrush(Colors.Green);
        public static SolidColorBrush BrushCyan = new SolidColorBrush(Colors.Cyan);
        public static SolidColorBrush BrushMagenta = new SolidColorBrush(Colors.Magenta);
        public static SolidColorBrush BrushYellow = new SolidColorBrush(Colors.Yellow);
        public static SolidColorBrush BrushOrange = new SolidColorBrush(Colors.Orange);
        public static SolidColorBrush BrushLimeGreen = new SolidColorBrush(Colors.LimeGreen);
        public static SolidColorBrush BrushNeonGreen = new SolidColorBrush(Color.FromArgb(255, 173, 255, 47));
        public static SolidColorBrush BrushDarkBlue = new SolidColorBrush(Colors.DarkBlue);
        public static SolidColorBrush BrushLightGray = new SolidColorBrush(Color.FromArgb(255, 192, 192, 192));
        public static SolidColorBrush BrushGray = new SolidColorBrush(Colors.Gray);
        public static SolidColorBrush BrushDarkGray = new SolidColorBrush(Colors.DarkGray);
        public static SolidColorBrush BrushDarkMagenta = new SolidColorBrush(Colors.DarkMagenta);
        public static SolidColorBrush BrushDarkGreen = new SolidColorBrush(Colors.DarkGreen);
        public static SolidColorBrush BrushDodgerBlue = new SolidColorBrush(Colors.DodgerBlue);
        public static SolidColorBrush BrushDeepSkyBlue = new SolidColorBrush(Colors.DeepSkyBlue);
        public static SolidColorBrush BrushSalmon = new SolidColorBrush(Colors.Salmon);
        public static SolidColorBrush BrushLightYellow = new SolidColorBrush(Colors.LightYellow);
        public static SolidColorBrush BrushOrangeRed = new SolidColorBrush(Colors.OrangeRed);
        public static SolidColorBrush BrushDarkRed = new SolidColorBrush(Colors.DarkRed);
        public static SolidColorBrush BrushBeige = new SolidColorBrush(Colors.Beige);
        public static SolidColorBrush BrushDarkKhaki = new SolidColorBrush(Colors.DarkKhaki);
        public static SolidColorBrush BrushLightGoldenrodYellow = new SolidColorBrush(Colors.LightGoldenrodYellow);
        public static SolidColorBrush BrushDarkOrange = new SolidColorBrush(Colors.DarkOrange);
        public static SolidColorBrush BrushDarkerOrange = new SolidColorBrush(Color.FromArgb(255, 220, 115, 0));
        public static SolidColorBrush BrushLightTeal = new SolidColorBrush(Color.FromArgb(255, 91, 200, 140));
        public static SolidColorBrush BrushBubbleGum = new SolidColorBrush(Color.FromArgb(255, 255, 136, 255));
        public static SolidColorBrush BrushDarkYellow = new SolidColorBrush(Color.FromArgb(255, 250, 192, 0));
        public static SolidColorBrush BrushJavaPurple = new SolidColorBrush(Color.FromArgb(255, 175, 100, 255));
        public static SolidColorBrush DarkestBrown = new SolidColorBrush(Color.FromArgb(255, 20, 15, 0));
        #endregion
    }
}
