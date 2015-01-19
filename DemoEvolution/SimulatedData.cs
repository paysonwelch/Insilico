using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Insilico;
using Nucleo;

namespace InsilicoDemo {
    public class DataSource : BaseThread {

        public Nucleo.EvolutionarySimulation simulation;
        public Insilico.EEG eeg;

        public float canvasWidth;
        public float canvasHeight;
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
                    // TimeSpan ts = DateTime.Now - Cached.startTime;
                    // double ticker = ts.TotalMilliseconds;

                    // Data generation (one step)
                    #region Code to Execute
                    if(simulation.currentState != SimState.Complete) {
                        //Thread.Sleep(10);
                        try {
                            eeg.PushDatum((float)simulation.MeanPopFit());
                        }
                        catch (Exception e) {
                        }
                    }
                    else {
                        simulation.Stop();
                        this.Stop();

                    }
                    #endregion
                }
                else { Thread.Sleep(400); } // Extra long slumber while we're paused.
                Thread.Sleep(10);
            }
        }
    }
}
