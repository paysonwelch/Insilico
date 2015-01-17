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
    public partial class Engine : BaseThread {

        // FIXME: This code should be updated, and moved into the Compute() method of the Graph object
        public void RenderGraph(Graph n, float offsetX, float offsetY) {
            if (canvas == null) return;
            // Offsets used for dragging (locally renamed for readability)
            float xo = offsetX;
            float yo = offsetY;
            // Prepare set of cached borders (use pre-existing and add extra as needed)
            if (Cached.cachedBorders.Count() < n.vertices.Count()) {
                int numEl = n.vertices.Count() - Cached.cachedBorders.Count();
                for (int i = 0; i < numEl; i++) { Cached.cachedBorders.Add(new Border()); }
            }

            // Loop through all vertices in network
            int cachBorderIdx = 0; // For border caching
            foreach (Vertex v in n.vertices.Values) {
                //if (v.bNegated) v.style.haloColor = Shared.BrushRed; // Adjust color for negation

                #region Vertex and label
                // Determine appropriate color based on AnimationStyleTemplate
                Point c = v.transCoords.X == 0.0 && v.transCoords.Y == 0.0 ? v.coordinates : v.transCoords;
                SolidColorBrush vertexColor = v.animation != null && v.animation.bPulseOn && v.animation.pulsePrimaryColor != Cached.BrushTransparent ? v.animation.pulsePrimaryColor : v.style.vertexColor;
                SolidColorBrush textColor = v.animation != null && v.animation.bPulseOn && v.animation.pulsePrimaryTextColor != Cached.BrushTransparent ? v.animation.pulsePrimaryTextColor : v.style.vertexTextColor;
                SolidColorBrush haloColor = v.animation != null && v.animation.bPulseOn && v.animation.pulseHaloColor != Cached.BrushTransparent ? v.animation.pulseHaloColor : v.style.haloColor;

                // Coordinate label
                if (bShowObjectCoordinates) {
                    Typeface typeface = new Typeface(v.style.fontFamily, FontStyles.Normal, v.style.vertexFontWeight, FontStretches.Normal);
                    if (v.coordLabel != null) canvas.Children.Remove(v.coordLabel);
                    string coordString = v.type == 5 ?
                        "min[" + Math.Round(v.minrelX, 0) + ", " + Math.Round(v.minrelY, 0) + "]\n" + "max[" + Math.Round(v.maxrelX, 0) + ", " + Math.Round(v.maxrelY, 0) + "]\n" + "(" + Math.Round(c.X + xo, 0) + ", " + Math.Round(c.Y + yo, 0) + ")" :
                        "(" + Math.Round(c.X + xo, 0) + ", " + Math.Round(c.Y + yo, 0) + ")";

                    v.coordLabel = Primitives.CreateTextBlock(coordString, typeface, 10, Cached.BrushDarkGreen, Cached.BrushTransparent, c.X + (v.labelBlockSize.Width * 0.7) + xo, c.Y - (v.labelBlockSize.Height * 0.75) + yo, false);
                    if (!canvas.Children.Contains(v.coordLabel)) canvas.Children.Add(v.coordLabel);
                }

                if (v.style.type == VertexShapeType.CombinedEllipse) {
                    if (v.box == null) {
                        float radius = (float)Math.Max(v.labelBlockSize.Width, v.labelBlockSize.Height)+10;
                        v.box = Primitives.CreateCombinedEllipse(new TextBlock(),
                            c.X + xo,
                            c.Y + yo,
                            radius,
                            radius,
                            vertexColor, haloColor, v.style.vertexOpacity, v.tooltip);
                    }
                    else {
                        if (c.X == v.coordinates.X && c.Y == v.coordinates.Y) {
                            Canvas.SetLeft(v.box, c.X + xo);
                            Canvas.SetTop(v.box, c.Y + yo);
                        }
                        else {
                            Canvas.SetLeft(v.box, c.X + xo - v.coordinates.X);
                            Canvas.SetTop(v.box, c.Y + yo - v.coordinates.Y);
                        }
                    }
                    if (!canvas.Children.Contains(v.box)) canvas.Children.Add(v.box);

                    if (v.labelBlock != null) {
                        Canvas.SetLeft(v.labelBlock, (c.X + xo) - (v.labelBlockSize.Width / 2));
                        Canvas.SetTop(v.labelBlock, (c.Y + yo) - (v.labelBlockSize.Height / 2));
                        v.labelBlock.Foreground = textColor;
                        if (!canvas.Children.Contains(v.labelBlock)) canvas.Children.Add(v.labelBlock);
                    }
                }

                // COMBINED RECTANGLE
                if (v.style.type == VertexShapeType.CombinedRectangle) {
                    if (v.box == null) {
                        v.box = Primitives.CreateCombinedRectangle(v.labelBlock, c.X + xo, c.Y + yo,
                        (v.labelBlockSize.Width + 10), (v.labelBlockSize.Height + 10),
                        vertexColor, v.style.vertexOpacity, v.style.haloOpacity, haloColor, Cached.StandardCornerRadius, Cached.cachedBorders[cachBorderIdx], v.tooltip);
                    }
                    else {
                        Canvas.SetLeft(v.box, c.X + xo - v.coordinates.X);
                        Canvas.SetTop(v.box, c.Y + yo - v.coordinates.Y);
                    }
                    // Draw required Region
                    if (bShowDensityRegions && v.reqRegion != null) {
                        Canvas.SetLeft(v.reqRegion, c.X + xo - v.coordinates.X);
                        Canvas.SetTop(v.reqRegion, c.Y + yo - v.coordinates.Y);
                        if (!canvas.Children.Contains(v.reqRegion)) canvas.Children.Add(v.reqRegion);
                    }
                    // Label
                    if (!canvas.Children.Contains(v.box)) canvas.Children.Add(v.box); // Box
                    if (v.labelBlock != null) {
                        Canvas.SetLeft(v.labelBlock, (c.X + xo) - (v.labelBlockSize.Width / 2));
                        Canvas.SetTop(v.labelBlock, (c.Y + yo) - (v.labelBlockSize.Height / 2));
                        v.labelBlock.Foreground = textColor;
                        if (!canvas.Children.Contains(v.labelBlock)) canvas.Children.Add(v.labelBlock);
                    }
                }

                if (v.attachedDisplay != null) { // Slow FIXME
                    if (v.attachedDisplay is Histogram) {
                        ((Histogram)v.attachedDisplay).xo = (int)c.X;
                        ((Histogram)v.attachedDisplay).yo = (int)c.Y;
                    }
                    if (v.attachedDisplay is LinePlot) {
                        ((LinePlot)v.attachedDisplay).xo = (int)c.X;
                        ((LinePlot)v.attachedDisplay).yo = (int)c.Y;
                    }
                }

                #endregion

                #region Edges
                if (bShowEdges) {
                    // Edges
                    foreach (Edge e in v.outgoingEdges.Values) {
                        // Determine whether or not we need to use the transformed coordinates
                        Point o = e.origin.transCoords.X == 0.0 && e.origin.transCoords.Y == 0.0 ? e.origin.coordinates : e.origin.transCoords;
                        Point d = e.destination.transCoords.X == 0.0 && e.destination.transCoords.Y == 0.0 ? e.destination.coordinates : e.destination.transCoords;
                        if (e.style != null) {
                            e.line.StrokeDashArray = e.style.StrokeDashArray;
                        }
                        if (e.line == null) e.line = new Line();
                        e.line.X1 = o.X + xo;
                        e.line.Y1 = o.Y + yo;
                        e.line.X2 = d.X + xo;
                        e.line.Y2 = d.Y + yo;
                        if (e.line.X1 == -9999 || e.line.Y1 == -9999 || e.line.X2 == -9999 || e.line.Y2 == -9999) continue;
                        Canvas.SetZIndex(e.line, 1); // Put line in rear

                        #region Get edge color from connecting vertices
                        if (e.type == 0) {
                            if (e.origin.type == 106 || e.origin.type == 108) { e.line.Stroke = e.destination.style.haloColor; }
                            else { e.line.Stroke = e.origin.style.haloColor; }
                        }
                        else {
                            EdgeStyleTemplate eStyle = layout.EdgeStyleRules[e.type];
                            e.line.Stroke = eStyle.linePrimaryColor;
                        }
                        #endregion

                        e.line.StrokeThickness = 3;
                        if (e.tooltip != "") e.line.ToolTip = e.tooltip;
                        if (!canvas.Children.Contains(e.line)) canvas.Children.Add(e.line);
                        if (layout.EdgeStyleRules.ContainsKey(e.type)) e.style = layout.EdgeStyleRules[e.type];

                        // Edge-node
                        #region Edgenode (defunct)
                        /*
                        if (e.style != null && e.style.bShowEdgeNode) {
                            double mid_x = (e.line.X1 + e.line.X2) / 2;
                            double mid_y = (e.line.Y1 + e.line.Y2) / 2;

                            if (e.edgeNode == null) {
                                e.edgeNode = Helpers.GenerateCombinedEllipse(new TextBlock(), mid_x, mid_y, 22, 22, e.style.edgeNodeColor, e.style.edgeNodeColor, 0.9, v.tooltip);
                            }
                            else {
                                Canvas.SetLeft(e.edgeNode, mid_x + xo - v.coordinates.X);
                                Canvas.SetTop(e.edgeNode, mid_y + yo - v.coordinates.Y);
                            }
                            if (e.edgeNodeLabel == null) {
                                Typeface typeface = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                                e.edgeNodeLabel = Helpers.GenerateTextBlock("OR", typeface, 12, e.style.edgeNodeTextColor, Shared.BrushTransparent, mid_x, mid_y, true, v.tooltip);
                            }
                            else {
                                Canvas.SetLeft(e.edgeNodeLabel, mid_x + xo - v.coordinates.X);
                                Canvas.SetTop(e.edgeNodeLabel, mid_y + yo - v.coordinates.Y);
                            }
                            
                            if(!canvas.Children.Contains(e.edgeNode)) canvas.Children.Add(e.edgeNode);
                            if(!canvas.Children.Contains(e.edgeNodeLabel)) canvas.Children.Add(e.edgeNodeLabel);
                        }
                         *  * */
                        if (e.style != null && e.style.bShowArrow && n.drawEdgeArrows) {
                            e.ComputeArrow(9, 5);
                            if (!canvas.Children.Contains(e.arrowLine0)) canvas.Children.Add(e.arrowLine0);
                            if (!canvas.Children.Contains(e.arrowLine1)) canvas.Children.Add(e.arrowLine1);
                        }

                        #endregion
                    }
                }
                #endregion

                cachBorderIdx++;
            }
        }
    }
}
