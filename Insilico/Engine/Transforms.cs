using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Insilico {
    public partial class Engine : BaseThread {
        /// <summary>
        /// Applies a coordinate offset to a subgraph (useful for dragging entire clusters)
        /// </summary>
        /// <param name="parent"> The origin vertex for the translation </param>
        /// <param name="offsetX"> Initial X-coord offset </param>
        /// <param name="offsetY"> Initial Y-coord offset </param>
        /// <param name="childStepDec"> </param>
        /// <param name="walk"> Current subgraph depth </param>
        public void TranslateSubtree(Vertex parent, float offsetX, float offsetY, float childStepDec = 0, int walk = 0) {
            if (offsetX == 0 && offsetY == 0) { // Restore original coords
                parent.transCoords.X = parent.transCoords.Y = 0;
            }
            else { // Transform coords
                parent.transCoords.X = parent.coordinates.X + offsetX - (offsetX * childStepDec * walk);
                parent.transCoords.Y = parent.coordinates.Y + offsetY - (offsetY * childStepDec * walk);
                if (childStepDec != 0) { // Adjust opacity per level (for simulated perspective)
                    double adjOpacity = 1.0 / ((walk + 0.0001) * 0.5);
                    parent.box.Opacity = adjOpacity;
                    foreach (Edge e in parent.incomingEdges.Values.Union(parent.outgoingEdges.Values)) {
                        e.line.Opacity = 1.0 / (walk + 0.0001);
                        e.arrowLine0.Opacity = adjOpacity;
                        e.arrowLine1.Opacity = adjOpacity;
                    }
                    Canvas.SetZIndex(parent.box, int.MaxValue - walk);
                    Canvas.SetZIndex(parent.labelBlock, int.MaxValue - walk);
                    foreach (Edge e in parent.incomingEdges.Values.Union(parent.outgoingEdges.Values)) {
                        Canvas.SetZIndex(e.line, int.MaxValue - walk);
                        Canvas.SetZIndex(e.arrowLine0, int.MaxValue - walk);
                        Canvas.SetZIndex(e.arrowLine1, int.MaxValue - walk);
                    }
                }
            }
            foreach (Vertex child in parent.children) {
                if (!child.bIsolated) TranslateSubtree(child, offsetX, offsetY, childStepDec, walk + 1);
            }
        }

        /// <summary>
        /// Applies a coordinate transform to a subgraph (cheap zoom)
        /// </summary>
        /// <param name="current"> The origin vertex of the transform </param>
        /// <param name="factor"> By how much we wish to transform the graph </param>
        /// <param name="parentX"> Final X-coord for parent vertex </param>
        /// <param name="parentY"> Final Y-coord for parent vertex </param>
        /// <param name="chOffX"> X-coord offset for parent vertex </param>
        /// <param name="chOffY"> Y-coord offset for parent vertex </param>
        /// <param name="walk"> How deep we are in the subgraph </param>
        public void TransformSubtree(Vertex current, float factor, float parentX, float parentY, float chOffX, float chOffY, int walk = 1) {
            float dx = (float)(current.coordinates.X - parentX);
            float dy = (float)(current.coordinates.Y - parentY);
            // Adjust zoom factor so children don't interfere with parents
            float f = factor;
            // Compute component contributions
            float xc = (dx * f) + (-dx);
            float yc = (dy * f) + (-dy);
            // Add inherited offset and contributions from each component
            current.transCoords.X = current.coordinates.X + (xc) + chOffX;
            current.transCoords.Y = current.coordinates.Y + (yc) + chOffY;
            foreach (Vertex child in current.children) {
                Point c = current.transCoords.X == 0.0 && current.transCoords.Y == 0.0 ? current.coordinates : current.transCoords;
                TransformSubtree(child, factor, (float)current.coordinates.X, (float)current.coordinates.Y, xc + chOffX, yc + chOffY, walk + 1);
            }
        }
    }
}
