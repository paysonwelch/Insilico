using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Insilico {
    public class Edge {
        public Line line = new Line();
        public Vertex origin;
        public Vertex destination;
        public EdgeStyleTemplate style = Styles.GreenGlass_EdgeStyle;
        public string tooltip = "";
        public Line arrowLine0 = new Line();
        public Line arrowLine1 = new Line();
        public Grid edgeNode;
        public TextBlock edgeNodeLabel;
        public int type = 0;

        public Edge(Vertex origin, Vertex destination, string tooltip) {
            this.origin = origin;
            this.destination = destination;
            this.tooltip = tooltip;
        }

        public void ComputeArrow(double headWidth, double headHeight) {
            float X1 = (float)line.X1;
            float X2 = (float)line.X2;
            float Y1 = (float)line.Y1;
            float Y2 = (float)line.Y2;

            float theta = (float)Math.Atan2(Y1 - Y2, X1 - X2);
            float sint = (float)Math.Sin(theta);
            float cost = (float)Math.Cos(theta);

            float percentage = Cached.arrowPercentage;
            float end_x = (X1 + percentage * (X2 - X1));
            float end_y = (Y1 + percentage * (Y2 - Y1));

            var endPoint = new Point(end_x, end_y);

            var sidePoint1 = new Point(
                endPoint.X + (headWidth * cost - headHeight * sint),
                endPoint.Y + (headWidth * sint + headHeight * cost));

            var sidePoint2 = new Point(
                endPoint.X + (headWidth * cost + headHeight * sint),
                endPoint.Y - (headHeight * cost - headWidth * sint));

            this.arrowLine0.X1 = endPoint.X;
            this.arrowLine0.Y1 = endPoint.Y;
            this.arrowLine0.X2 = sidePoint1.X;
            this.arrowLine0.Y2 = sidePoint1.Y;
            this.arrowLine0.Stroke = line.Stroke;
            this.arrowLine0.StrokeThickness = 2;

            this.arrowLine1.X1 = endPoint.X;
            this.arrowLine1.Y1 = endPoint.Y;
            this.arrowLine1.X2 = sidePoint2.X;
            this.arrowLine1.Y2 = sidePoint2.Y;
            this.arrowLine1.Stroke = line.Stroke;
            this.arrowLine1.StrokeThickness = 2;
        }
    }
}
