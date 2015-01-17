using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Insilico {

    public enum VertexShapeType { Ellipse, Rectangle, RoundedRectangle, CombinedRectangle, CombinedEllipse };

    public class VertexStyleTemplate {
        public double vertexOpacity;
        public SolidColorBrush vertexColor;
        public VertexShapeType type;
        public double haloThickness;
        public SolidColorBrush haloColor;
        public double haloOpacity;
        public SolidColorBrush vertexTextColor;
        public FontWeight vertexFontWeight = FontWeights.DemiBold;
        public FontFamily fontFamily = new FontFamily("Calibri");
        public bool bRoundedRectangleCorners;
        public bool bShowHalo = true;
        public VertexStyleTemplate tooltipTemplate;
        public bool bShowLabel = true;
        public float vertexRadius;
        public bool bShowExtraHalo = false;
    }

    public class VertexAnimationTemplate {
        public double lastPulse = 0;
        public bool bPulseOn;
        public int pulseLength;
        public int pulseDelayLength;
        public SolidColorBrush pulsePrimaryColor;
        public SolidColorBrush pulseSecondaryColor;
        public SolidColorBrush pulsePrimaryTextColor;
        public SolidColorBrush pulseSecondaryTextColor;
        public SolidColorBrush pulseHaloColor;

        public VertexAnimationTemplate(VertexAnimationTemplate vat) {
            pulseLength = vat.pulseLength;
            pulseDelayLength = vat.pulseDelayLength;
            pulsePrimaryColor = vat.pulsePrimaryColor;
            pulseSecondaryColor = vat.pulseSecondaryColor;
            pulsePrimaryTextColor = vat.pulsePrimaryTextColor;
            pulseSecondaryTextColor = vat.pulseSecondaryTextColor;
            pulseHaloColor = vat.pulseHaloColor;
        }
        public VertexAnimationTemplate() { }
    }

    public class EdgeStyleTemplate {
        public bool bShowEdgeNode;
        public bool bShowArrow = true;
        public double lineThickness;
        public SolidColorBrush linePrimaryColor;
        public SolidColorBrush lineTextColor;
        public DoubleCollection StrokeDashArray;
        public SolidColorBrush edgeNodeColor;
        public SolidColorBrush edgeNodeTextColor;
        public double edgeNodeOpacity;
    }

    public class CanvasThemeTemplate {
        public Brush background;
        public Color comboBoxBackground;
        public Color comboBoxForeground;
        public Color comboBoxItemSelected;
    }
}
