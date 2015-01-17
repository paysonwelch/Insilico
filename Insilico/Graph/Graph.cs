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

        public override void ComputeDecorations() { }
        public override void ComputeActiveElements() { }

        public Graph(Graph n = null) {
            zOrder = 10;
            vertices = n != null ? n.vertices : new ConcurrentDictionary<string, Vertex>();
            edges = n != null ? n.edges : new List<Edge>();
        }

        public float forceConstant = 20;
        public float minDist = 6;
        public float maxDist = 100;
        public float minAcc = 1;
        public float charge = 100;
        public float mass = 130;

        List<Vertex> v;
        HashSet<int> pairs = new HashSet<int>();

        public static int factorial(int i) {
            return ((i <= 1) ? 1 : (i * factorial(i - 1)));
        }


        public ulong Factorial(ulong n) {
            ulong Result = 1;
            for (ulong i = 1; i <= n; i++) {
                Result *= i;
            }
            return Result;
        }

        public override void ComputeMetrics() { }

        public override void Compute() {
            /*
            //Check which objects are inside the rectangle:
            var objects = tree.Contains(rect);

            //Count how many items in the RTree:
            var i = tree.Count;

            //Check which objects intersect with the rectangle:
            var objects = tree.Intersects(rect);

            //Create a point:
            RTree.Point point = new RTree.Point(1, 2, 3);

            //Get a list of rectangles close to the point with maximum distance:
            var objects = tree.Nearest(point, 10);
            */


            int count = 0;
            int actualCount = 0;
            v = vertices.Values.ToList();

            /*
            int vcount = v.Count;
            int reserve = (int)(Factorial((ulong)vcount) / Factorial((ulong)(vcount - 2)));
            */

            //List<int> PreallocationList = Enumerable.Range(0, 500000).ToList();


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

        public void ComputePairInteraction(Vertex a, Vertex b) {
            
                float ax;
                float ay;
                float bx;
                float by;

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

                        float d = (float)Math.Sqrt(preDist);

                        if (d > minDist && d < maxDist) {

                            //if (a.adding && !a.neighbors.Contains(b)) a.neighbors.Add(b);
                            //if (b.adding && !b.neighbors.Contains(a)) a.neighbors.Add(a);

                            float f = (float)((forceConstant * charge * charge) / (d * d)); // F_e = k*q_1*q_2 / r^2

                            //if (f > 0.9 * mass || f > 0.9 * mass) {

                            float dxd = dx / d;
                            float fxcomp = (float)(f * Math.Abs(Math.Acos(dxd)));
                            float aix = -(float)(fxcomp / mass);
                            //float ajx = -(float)(fxcomp / mass);
                            float ajx = aix;

                            float dyd = dy / d;
                            float fycomp = (float)(f * Math.Abs(Math.Acos(dyd)));
                            float aiy = -(float)(fycomp / mass);
                            //float ajy = -(float)(fycomp / mass);
                            float ajy = aiy;

                            //if (Math.Abs(aix) > minAcc || Math.Abs(aiy) > minAcc || Math.Abs(ajx) > minAcc || Math.Abs(ajy) > minAcc) {
                            a.transCoords.X = ax - aix;
                            a.transCoords.Y = ay - aiy;
                            b.transCoords.X = bx + ajx;
                            b.transCoords.Y = by + ajy;
                        }
                    }
                }
            }


        public Vertex GetRandom() {
            Random rand = new Random();
            return vertices.Any() ? vertices.Values.ToList()[rand.Next(0, vertices.Count)] : null;
        }

        // Add a vertex to the network
        public void Add(Vertex v) {
            if (v.style == null) v.style = DefaultVertexStyleTemplate;
            if (!vertices.ContainsKey(v.label)) {
                vertices.TryAdd(v.label, v); // Handle Exception???
            }

            //Create a rectangle:
            float x1 = (float)v.coordinates.X;
            float y1 = (float)v.coordinates.Y;
            float x2 = (float)v.coordinates.X;
            float y2 = (float)v.coordinates.Y;
            //RTree.Rectangle rect = new RTree.Rectangle(x1, y1,     x2, y2,     0, 0);
            //Add a new rectangle to the RTree:
            //tree.Add(rect, v);
        }

        public void Remove(Vertex v) {
            Edge outEdge;
            foreach (Edge e in v.incomingEdges.Values) { e.origin.outgoingEdges.TryRemove(e, out outEdge); }
            foreach (Edge e in v.outgoingEdges.Values) { e.origin.incomingEdges.TryRemove(e, out outEdge); }

            vertices.TryRemove(v.label, out v);
        }


        public void Dispose() {
            edges.Clear();
            vertices.Clear();
        }

        // Create a link between two nodes
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
    }
}
