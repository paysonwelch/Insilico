using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Insilico;

namespace InsilicoDemo {
    public class SimulatedData : BaseThread {

        public float canvasWidth;
        public float canvasHeight;

        public Insilico.Graph graph;
        public Insilico.Histogram histogram;
        public Insilico.LinePlot lineplot;

        Random rand = new Random(); 

        public override void RunThread() {
            while (!_shouldStop) {
                if (!_shouldPause) {
                    if (_shouldSleep) {
                        Thread.Sleep(_sleepTime);
                        _sleepTime = 0;
                        _shouldSleep = false;
                    }

                    // Time ticker 
                    TimeSpan ts = DateTime.Now - Cached.startTime;
                    double ticker = ts.TotalMilliseconds;

                    // Data generation (one step)
                    #region Code to Execute
                    //GenerateGraphData(ticker);
                    #endregion
                }
                else { Thread.Sleep(400); } // Extra long slumber while we're paused.
                Thread.Sleep(50);
            }
        }



        public void GenerateGraphData(double ticker) {
            if (Math.Round(ticker, 0) % 9 == 0) {
                //int toDelete = rand.Next(0, gr.vertices.Count - 1);
                //gr.Remove(gr.vertices.Values.ToList()[toDelete]); 
                Vertex newVertex = new Vertex();
                newVertex.style = Styles.Green_VertexStyle;
                newVertex.type = 5;
                newVertex.label = "" + Math.Round(ticker, 0);
                newVertex.coordinates = Insilico.Engine.ToWPFCoords(new Point(rand.Next(-100, 100), rand.Next(-100, 100)), canvasWidth, canvasHeight);
                //graph.forceConstant += rand.Next(-3, 3);
                graph.Add(newVertex);
                //graph.CreateUnidirectionalEdge(newVertex, graph.GetRandom());
                //graph.CreateUnidirectionalEdge(graph.GetRandom(), newVertex);
            }

        }
    }
}
