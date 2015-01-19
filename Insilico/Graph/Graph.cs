using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Insilico {
    public class Graph : BaseDisplay {

        public ConcurrentDictionary<string, Vertex> vertices;
        public List<Edge> edges;
        public List<Vertex> topLevelVertices = new List<Vertex>();
        public VertexStyleTemplate DefaultVertexStyleTemplate;
        public EdgeStyleTemplate DefaultEdgeStyleTemplate;
        public Vertex Root;
        public bool drawEdgeArrows;
        public float forceConstant = 8;
        public float minDist = 4;
        public float maxDist = 90;
        public float minAcc = 1;
        public float charge = 100;
        public float mass = 150;

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

            // Set vertex coordinates
            var nodes = vertices.Values.ToList();
            for (int i = 0; i < nodes.Count(); i++) {
                Point c = nodes[i].transCoords.X == 0.0 && nodes[i].transCoords.Y == 0.0 ? nodes[i].coordinates : nodes[i].transCoords;
                if (nodes[i].style.type == VertexShapeType.CombinedEllipse) {
                    Canvas.SetLeft(nodes[i].box, c.X  - nodes[i].coordinates.X);
                    Canvas.SetTop(nodes[i].box, c.Y - nodes[i].coordinates.Y);
                }
                // Set edge coordinates
                foreach (Edge e in nodes[i].outgoingEdges.Values) {
                    Point o = e.origin.transCoords.X == 0.0 && e.origin.transCoords.Y == 0.0 ? e.origin.coordinates : e.origin.transCoords;
                    Point d = e.destination.transCoords.X == 0.0 && e.destination.transCoords.Y == 0.0 ? e.destination.coordinates : e.destination.transCoords;
                    e.line.X1 = o.X;
                    e.line.Y1 = o.Y;
                    e.line.X2 = d.X;
                    e.line.Y2 = d.Y;
                    Canvas.SetZIndex(e.line, 1); // Put line in rear
                }
            }
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
            elements.Add(v.box);
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

            Line newLine = new Line();
            newLine.Stroke = Cached.BrushLimeGreen;
            newLine.StrokeThickness = 4;
            elements.Add(newLine);
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

                Line newLine = new Line();
                newLine.Stroke = Cached.BrushLimeGreen;
                newLine.StrokeThickness = 4;
                newLine.Opacity = 0.5;
                elements.Add(newLine);
                newEdge.line = newLine;
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
    }
}
