using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Insilico {
    public static class Styles {
        public static VertexStyleTemplate Green_VertexStyle = new VertexStyleTemplate {
            haloThickness = 4,
            haloOpacity = 0.0,
            vertexRadius = 20,
            haloColor = Cached.BrushLimeGreen,
            vertexTextColor = Cached.BrushWhite,
            vertexFontWeight = FontWeights.Bold,
            vertexColor = Cached.BrushDarkGreen,
            vertexOpacity = 0.85,
            type = VertexShapeType.CombinedEllipse,
            bRoundedRectangleCorners = true,
            bShowHalo = true
        };

        public static VertexStyleTemplate Purple_VertexStyle = new VertexStyleTemplate {
            haloThickness = 4,
            haloOpacity = 0.0,
            haloColor = Cached.BrushMagenta,
            vertexTextColor = Cached.BrushWhite,
            vertexFontWeight = FontWeights.Bold,
            vertexColor = Cached.BrushDarkMagenta,
            vertexOpacity = 0.85,
            type = VertexShapeType.CombinedRectangle,
            bRoundedRectangleCorners = true,
            bShowHalo = true
        };

        public static EdgeStyleTemplate GreenGlass_EdgeStyle = new EdgeStyleTemplate {
            edgeNodeColor = Cached.BrushBlack,
            edgeNodeTextColor = Cached.BrushWhite,
            edgeNodeOpacity = 1.0,
            //StrokeDashArray = new DoubleCollection() { 2, 4 },
            linePrimaryColor = Cached.BrushGray,
            bShowArrow = true
        };

        public static EdgeStyleTemplate Red_EdgeStyle = new EdgeStyleTemplate {
            edgeNodeColor = Cached.BrushBlack,
            edgeNodeTextColor = Cached.BrushWhite,
            edgeNodeOpacity = 0.85,
            StrokeDashArray = new DoubleCollection() { 1, 0.5 },
            linePrimaryColor = Cached.BrushRed
        };

        public static EdgeStyleTemplate Conjunction_EdgeStyle = new EdgeStyleTemplate {
            edgeNodeColor = Cached.BrushBubbleGum,
            edgeNodeTextColor = Cached.BrushWhite,
            edgeNodeOpacity = 1.0,
            StrokeDashArray = new DoubleCollection() { 1, 0.5 },
            linePrimaryColor = Cached.BrushBubbleGum,
            bShowEdgeNode = false,
            bShowArrow = false
        };
    }
}
