using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Insilico {
    public class Graph : BaseDisplay {

        public ConcurrentDictionary<string, Vertex> vertices;
        public List<Edge> edges;
        public List<Vertex> topLevelVertices = new List<Vertex>();
        public VertexStyleTemplate DefaultVertexStyleTemplate;
        public EdgeStyleTemplate DefaultEdgeStyleTemplate;
        public Vertex Root;
        public bool drawEdgeArrows;
        public float forceConstant = 20;
        public float minDist = 6;
        public float maxDist = 100;
        public float minAcc = 1;
        public float charge = 100;
        public float mass = 130;

        public Graph(Graph n = null) {
            zOrder = 10;
            vertices = n != null ? n.vertices : new ConcurrentDictionary<string, Vertex>();
            edges = n != null ? n.edges : new List<Edge>();
        }

        List<Vertex> v;
        HashSet<int> pairs = new HashSet<int>();

        public override void ComputeDecorations() { }
        public override void ComputeActiveElements() { }
        public override void ComputeMetrics() { }

        /// <summary>
        /// Computes the interaction between all pairs of vertices (repulsive force, acceleration, etc)
        /// </summary>
        public override void Compute() {
            int count = 0;
            int actualCount = 0;
            v = vertices.Values.ToList();

            pairs.Clear();
            for (int i = 0; i < v.Count(); i++) {
                if (v[i].adding) {
                    //v[i].neighbors.Clear();
                    for (int j = 0; j < v.Count(); j++) {
                        count++;
                        if (j != i && !pairs.Contains(j * 1000 + i * 10)) {
                            pairs.Add(i * 1000 + j * 10);
                            actualCount++;
                            ComputePairInteraction(v[i], v[j]);
                            //v[i].adding = !v[i].adding;
                        }
                    }
                }
                else {
                    for (int j = 0; j < v[i].neighbors.Count(); j++) {
                        if (j != i && !pairs.Contains(j * 1000 + i * 10)) {
                            pairs.Add(i * 1000 + j * 10);
                            actualCount++;
                            ComputePairInteraction(v[i], v[j]);
                            v[i].adding = !v[i].adding;
                        }
                    }
                }
            }
            //int skipped = count - actualCount;
            //float percentageSkipped = (float)actualCount / (float)count;
        }

        /// <summary>
        /// Compute the force a pair of vertices exhibit on each other, and their resultant accelerations
        /// </summary>
        public void ComputePairInteraction(Vertex a, Vertex b) {
            float ax;
            float ay;
            float bx;
            float by;
            float d;

            if (a.transCoords.X == 0.0 && a.transCoords.Y == 0.0) {
                ax = (float)a.coordinates.X;
                ay = (float)a.coordinates.Y;
            }
            else {
                ax = (float)a.transCoords.X;
                ay = (float)a.transCoords.Y;
            }

            if (b.transCoords.X == 0.0 && b.transCoords.Y == 0.0) {
                bx = (float)b.coordinates.X;
                by = (float)b.coordinates.Y;
            }
            else {
                bx = (float)b.transCoords.X;
                by = (float)b.transCoords.Y;
            }

            float dx = (float)(bx - ax);
            float dy = (float)(by - ay);

            if (dx < maxDist || dy < maxDist) { // Indicates that it's *possible* the distance could be within our bounds
                float preDist = (float)((dx * dx) + (dy * dy));

                if (!float.IsNaN(preDist)) {
                    d = (float)Math.Sqrt(preDist);
                    if (d > minDist && d < maxDist) {

                        float f = (float)((forceConstant * charge * charge) / (d * d)); // F_e = k*q_1*q_2 / r^2

                        float dxd = dx / d;
                        float fxcomp = (float)(f * Math.Abs(Math.Acos(dxd)));
                        float aix = -(float)(fxcomp / mass);
                        float ajx = aix;

                        float dyd = dy / d;
                        float fycomp = (float)(f * Math.Abs(Math.Acos(dyd)));
                        float aiy = -(float)(fycomp / mass);
                        float ajy = aiy;

                        a.transCoords.X = ax - aix;
                        a.transCoords.Y = ay - aiy;
                        b.transCoords.X = bx + ajx;
                        b.transCoords.Y = by + ajy;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a random vertex from the graph
        /// </summary>
        public Vertex GetRandomVertex() {
            Random rand = new Random();
            return vertices.Any() ? vertices.Values.ToList()[rand.Next(0, vertices.Count)] : null;
        }

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        public void Add(Vertex v) {
            if (v.style == null) v.style = DefaultVertexStyleTemplate;
            if (!vertices.ContainsKey(v.label)) {
                vertices.TryAdd(v.label, v); // Handle Exception???
            }
        }

        /// <summary>
        /// Removes a vertex from the graph
        /// </summary>
        public void Remove(Vertex v) {
            Edge outEdge;
            foreach (Edge e in v.incomingEdges.Values) { e.origin.outgoingEdges.TryRemove(e, out outEdge); }
            foreach (Edge e in v.outgoingEdges.Values) { e.origin.incomingEdges.TryRemove(e, out outEdge); }

            vertices.TryRemove(v.label, out v);
        }

        /// <summary>
        /// Creates a unidirectional link between two vetices
        /// </summary>
        public bool CreateUnidirectionalEdge(string vA, string vB, EdgeStyleTemplate style = null, string tooltip = "") {
            Vertex nA = vertices[vA];
            Vertex nB = vertices[vB];
            Edge newEdge = new Edge(nA, nB, tooltip);
            newEdge.style = newEdge.style == null ? style : DefaultEdgeStyleTemplate;
            nA.children.Add(nB);
            nA.outgoingEdges.TryAdd(newEdge, newEdge);
            nB.incomingEdges.TryAdd(newEdge, newEdge);
            return true;
        }

        /// <summary>
        /// Creates a bidirectional link between two vetices
        /// </summary>
        public bool CreateBidirectionalEdge(string vA, string vB, EdgeStyleTemplate style = null, string tooltip = "") {
            Vertex nA = vertices[vA];
            Vertex nB = vertices[vB];
            Edge newEdge = new Edge(nA, nB, tooltip);
            newEdge.style = newEdge.style == null ? style : DefaultEdgeStyleTemplate;
            nA.children.Add(nB);
            nA.outgoingEdges.TryAdd(newEdge, newEdge);
            nB.incomingEdges.TryAdd(newEdge, newEdge);
            nB.outgoingEdges.TryAdd(newEdge, newEdge);
            nA.incomingEdges.TryAdd(newEdge, newEdge);
            return true;
        }

        public bool CreateUnidirectionalEdge(Vertex vA, Vertex vB, EdgeStyleTemplate style = null, string tooltip = "") {
            Edge newEdge = new Edge(vA, vB, tooltip);
            newEdge.style = newEdge.style == null ? style : DefaultEdgeStyleTemplate;
            if (vA != null && vB != null) {
                vA.children.Add(vB);
                vA.outgoingEdges.TryAdd(newEdge, newEdge);
                vB.incomingEdges.TryAdd(newEdge, newEdge);
                return true;
            }
            return false;
        }

        public bool CreateBidirectionalEdge(Vertex vA, Vertex vB, EdgeStyleTemplate style = null, string tooltip = "") {
            Edge newEdge = new Edge(vA, vB, tooltip);
            newEdge.style = newEdge.style == null ? style : DefaultEdgeStyleTemplate;
            if (vA != null && vB != null) {
                vA.children.Add(vB);
                vA.outgoingEdges.TryAdd(newEdge, newEdge);
                vB.incomingEdges.TryAdd(newEdge, newEdge);
                vB.outgoingEdges.TryAdd(newEdge, newEdge);
                vA.incomingEdges.TryAdd(newEdge, newEdge);
                return true;
            }
            return false;
        }




        /*
         * 
         * public void GenerateLabels() {
            //labels.Clear();
            #region Compute Label size (must be done before the density survey)
            foreach (Vertex v in graph.vertices.Values) {
                // Select style based on rules
                GenerateLabel(v);
            }
            #endregion
        }

        public void GenerateLabel(Vertex v) {
            VertexStyleTemplate style = null;
            if (layout.VertexStyleRules.ContainsKey(v.type)) { style = layout.VertexStyleRules[v.type]; }
            v.style = style == null ? v.style : style;
            // We pre-render this so the spacing algorithm knows how much space to allocate for this vertex
            int fSize = (int)Math.Ceiling((Cached.nodeRadiusFudge) * 14);
            if (v.labelBlock == null) {
                Point p = v.transCoords.X == 0.0 && v.transCoords.Y == 0.0 ? v.coordinates : v.transCoords;

                Typeface typeface = new Typeface(v.style.fontFamily, FontStyles.Normal, v.style.vertexFontWeight, FontStretches.Normal); // FIXME possibly cache various sized typefaces?
                v.labelBlock = Primitives.CreateTextBlock(v.label, typeface, fSize, v.style.vertexTextColor, v.style.vertexColor, p.X, p.Y, true, v.tooltip);
                v.labelBlock.Background = Cached.BrushTransparent;
                labels.Add(v.labelBlock);

                if (bSizeVertexToText) {
                    if (bUseStandardTypeface && standardTypeface != null) {
                        v.labelBlockSize = Primitives.MeasureString(v.labelBlock, standardTypeface);
                    }
                    else {
                        typeface = new Typeface(v.labelBlock.FontFamily, v.labelBlock.FontStyle, v.labelBlock.FontWeight, v.labelBlock.FontStretch);
                        v.labelBlockSize = Primitives.MeasureString(v.labelBlock, typeface);
                    }
                }
            }
        }*/





        /*
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
         * */
    }
}
