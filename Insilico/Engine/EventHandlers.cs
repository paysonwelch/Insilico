using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Insilico {
    public partial class Engine : BaseThread {

        /// <summary>
        /// Handles Left-Button mouse click events
        /// </summary>
        public void OnLeftMouseDown(object sender, MouseButtonEventArgs e) {
            Point p = Mouse.GetPosition(this.canvas);
            Cached.lastPoint = p;
            foreach (Vertex v in Cached.graph.vertices.Values.ToList().Where(q => q.box.IsMouseOver || q.labelBlock.IsMouseOver)) { Cached.lastClickedVertex = v; }
            if (Cached.lastClickedVertex == null) Cached.dragStartPoint = p;
        }

        /// <summary>
        /// Handles Right-Button mouse click events
        /// </summary>
        public void OnRightMouseDown(object sender, MouseButtonEventArgs e) {
            Point p = Mouse.GetPosition(this.canvas);
            Cached.lastPoint = p;
            foreach (Vertex v in Cached.graph.vertices.Values.ToList().Where(q => q.box.IsMouseOver || q.labelBlock.IsMouseOver)) {
                Cached.lastClickedVertex = v;
                Cached.rightClicked = true;
            }
        }

        /// <summary>
        /// Handles mouse movement events
        /// </summary>
        public void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            float xo, yo;
            Point p = Mouse.GetPosition(this.canvas);
            // If we're holding the mouse button down and we've previously clicked on a vertex
            if (e.LeftButton == MouseButtonState.Pressed && Cached.graph.Root != null) {
                if (Cached.lastPoint != p && Cached.lastClickedVertex != null) {
                    Cached.dragEndPoint = p;
                    xo = (float)(Cached.dragEndPoint.X + Cached.rightDragPrevOffset.X - Cached.lastClickedVertex.coordinates.X - Cached.leftDragPrevOffset.X);
                    yo = (float)(Cached.dragEndPoint.Y + Cached.rightDragPrevOffset.Y - Cached.lastClickedVertex.coordinates.Y - Cached.leftDragPrevOffset.Y);
                    Cached.lastClickedVertex.transCoords.X = Cached.lastClickedVertex.coordinates.X + xo;
                    Cached.lastClickedVertex.transCoords.Y = Cached.lastClickedVertex.coordinates.Y + yo;
                    RenderGraph(Cached.graph, (float)Cached.leftDragPrevOffset.X, (float)Cached.leftDragPrevOffset.Y);
                }
                else if (Cached.lastClickedVertex == null) {
                    Cached.dragEndPoint = p;
                    xo = (float)(Cached.dragEndPoint.X - Cached.dragStartPoint.X + Cached.leftDragPrevOffset.X);
                    yo = (float)(Cached.dragEndPoint.Y - Cached.dragStartPoint.Y + Cached.leftDragPrevOffset.Y);
                    RenderGraph(Cached.graph, xo, yo);
                }
            }
            if (e.RightButton == MouseButtonState.Pressed && Cached.graph.Root != null) { // Drag entire cluster
                if (Cached.lastPoint != p && Cached.lastClickedVertex != null) {
                    Cached.dragEndPoint = p;
                    xo = (float)(Cached.dragEndPoint.X + Cached.rightDragPrevOffset.X - Cached.lastClickedVertex.coordinates.X - Cached.leftDragPrevOffset.X);
                    yo = (float)(Cached.dragEndPoint.Y + Cached.rightDragPrevOffset.Y - Cached.lastClickedVertex.coordinates.Y - Cached.leftDragPrevOffset.Y);
                    TranslateSubtree(Cached.lastClickedVertex, (float)(xo), (float)(yo));
                    RenderGraph(Cached.graph, (float)Cached.leftDragPrevOffset.X, (float)Cached.leftDragPrevOffset.Y);
                }
            }
            /* 
            // PERSPECTIVE DRAG
            if (Shared.stickyDrag && e.RightButton == MouseButtonState.Pressed && myNetworkPlot != null && Shared.network.Root != null) { // Drag entire cluster
                if (Shared.lastPoint != p && Shared.lastClickedVertex != null) {
                    Shared.dragEndPoint = p;
                    xo = (float)(Shared.dragEndPoint.X + Shared.rightDragPrevOffset.X - Shared.lastClickedVertex.coordinates.X - Shared.leftDragPrevOffset.X);
                    yo = (float)(Shared.dragEndPoint.Y + Shared.rightDragPrevOffset.Y - Shared.lastClickedVertex.coordinates.Y - Shared.leftDragPrevOffset.Y);
                    myNetworkPlot.TranslateSubtree(Shared.lastClickedVertex, (float)(xo), (float)(yo), 0.15f);
                    myNetworkPlot.Render_Network(Shared.network, (float)Shared.leftDragPrevOffset.X, (float)Shared.leftDragPrevOffset.Y);
                }
            }*/
        }

        /// <summary>
        /// Handles mouse button release events
        /// </summary>
        public void OnClickUp(object sender, RoutedEventArgs e) {
            Point p = Mouse.GetPosition(this.canvas);
            if (Cached.lastClickedVertex == null) {
                if (Cached.lastClickedVertex == null) { // Save drag location state (for next drag)
                    Cached.leftDragPrevOffset.X += (Cached.dragEndPoint.X - Cached.dragStartPoint.X);
                    Cached.leftDragPrevOffset.Y += (Cached.dragEndPoint.Y - Cached.dragStartPoint.Y);
                }
            }
            if (Cached.lastClickedVertex != null) {
                if (Cached.rightClicked) {
                    Cached.rightClicked = false;
                    RequestSnapBack(Cached.lastClickedVertex, Cached.lastPoint);

                }
            }
            Cached.dragStartPoint = Cached.ZeroPoint;
            Cached.lastClickedVertex = null;
            Cached.lastPoint = p;
        }

        /// <summary>
        /// Handles mouse-wheel events
        /// </summary>
        public void OnMouseWheel(object sender, MouseWheelEventArgs e) {
            /*
            float multiplier = (float)(0.001 * e.Delta);
            if (Shared.edgeLengthFudge + multiplier >= 0.5) Shared.edgeLengthFudge += (float)(multiplier * 1.2); // Increase radii slightly faster than vertex size
            if (Shared.nodeRadiusFudge + multiplier >= 3.0) Shared.nodeRadiusFudge += (float)(multiplier*0.1);
            PlotSurface.Children.Clear();

            Point p = Mouse.GetPosition(this);
            Point q = myNetworkPlot.rev_coord(p, (float)PlotSurface.ActualWidth, (float)PlotSurface.ActualHeight);
            float xo = (float)(initialX + Shared.leftDragPrevOffset.X);
            float yo = (float)(initialY + Shared.leftDragPrevOffset.Y);
            int completelyArbitraryNumber = 20;
            if (multiplier < 0) {
                if (Math.Abs(q.X - initialX) > completelyArbitraryNumber && Math.Abs(q.Y - initialY) > completelyArbitraryNumber && Shared.edgeLengthFudge != 1.0) { // Keep us from zooming into the abyss
                    xo -= (float)(xo / 10.0);
                    yo -= (float)(yo / 10.0);
                }
            }
            else {
                xo -= (float)(q.X / 10.0);
                yo -= (float)(q.Y / 10.0);
            }
            Shared.lastOffset = Shared.ZeroPoint;
            Shared.dragEndPoint = Shared.ZeroPoint;
            Shared.dragStartPoint = Shared.ZeroPoint;
            Shared.leftDragPrevOffset = Shared.ZeroPoint;
            Shared.rightDragPrevOffset = Shared.ZeroPoint;

            myNetworkPlot.GenerateLabels();
            foreach (Vertex tlu in Shared.network.topLevelVertices) {
                myNetworkPlot.TransformSubtree(tlu, Shared.edgeLengthFudge, (float)Shared.network.Root.coordinates.X, (float)Shared.network.Root.coordinates.Y, xo, yo, 1);
            }
            myNetworkPlot.Render_Network(Shared.network, 0, 0);
             */
        }

        public void OnSizeChanged(object sender, RoutedEventArgs e) {
            // This space is intentionally left blank
        }
    }
}
