using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Insilico;
using InsilicoDemo;
using Nucleo;

/*
 * 
 * This demo shows how to set up a DataSource object and supply it with content (in this
 * case from a Nucleo evolutionary optimizer instance). The DataSource object is a separate 
 * thread which monitors the state of the optimizer and pushes that data to the EEG display
 * object in the Insilico engine.
 * 
 * */

namespace EvolutionDemo {

    public partial class MainWindow: Window {

        Engine insilico = new Insilico.Engine();
        Random rand = new Random();

        public MainWindow() {
            InitializeComponent();
            this.Title = "EvolutionDemo";

            #region Engine setup
            insilico.canvas = MyCanvas;             // Tell the library where to draw
            insilico.bShowAnimations = true;        // Enable node-physics and smooth display transitions
            insilico.Start();                       // Start the background thread (for animations and physics)

            insilico.bEnableSimulatedData = false;  // <------------------ Enable/Disable simulated data here
            #endregion

            #region Display Objects (LinePlots, Histograms, VitalIndicators, EEGs, Networks, etc)
            EEG eeg = new EEG(30);
            insilico.displays.Add(eeg);
            eeg.Height = 450;
            eeg.Width = 1033;
            eeg.yo = 0;
            eeg.xo = 0;
            eeg.max = 40;
            eeg.min = 0;
            eeg.Activate();
            #endregion

            #region Set up evolutionary optimizer
            EvolutionarySimulation evo = new EvolutionarySimulation() {
                reqGenerations = 10000,
                initialPopCount = 100,
                indFitCutoff = 0.5,
                popFitCutoff = 0.5,
                geneCount = 3,
                FitnessTest = SolveSystemOf_3Equations,
                acceptedDistance = 0.01,
                bJournalingEnabled = true,
                endCondition = SimEndCondition.StopOnFirstSolution,
                mutationRegime = MutationRegime.LinearDecrease,
                bCheckMeanPopDiv = false
            };
            #endregion

            #region Set up data source (feed from the evolutionary optimizer)
            DataSource myDataSource = new DataSource();
            myDataSource.simulation = evo;
            myDataSource.eeg = eeg;
            myDataSource.canvasWidth = 450.0f;
            myDataSource.canvasHeight = 1033.0f;
            myDataSource.Start();
            evo.StartSim();
            #endregion
        }


        // 3-Equation fitness test
        static double SolveSystemOf_3Equations(List<double> genome) {
            if (genome.Count() < 3) return 0;

            // The variables x, y, and z for our system of equations
            double x = genome[0];
            double y = genome[1];
            double z = genome[2];

            // The values our system of equations is set to
            //  x - 3y + 3z = -4
            // 2x + 3y -  z = 15
            // 4x - 3y -  z = 19
            double a = -4;
            double b = 15;
            double c = 19;

            // The distances we want to minimize
            double s0 = Math.Abs((x - (3 * y) + (3 * z)) - a);
            double s1 = Math.Abs(((2 * x) + (3 * y) - z) - b);
            double s2 = Math.Abs(((4 * x) - (3 * y) - z) - c);

            // Total distance from acceptable solution
            // Solution: x = 5, y = 1, z = -2
            return (s0 + s1 + s2);
        }


        #region Mouse Events (if you need them)
        private void LeftMouseDown(object sender, MouseButtonEventArgs e) { insilico.OnLeftMouseDown(sender, e); }
        private void RightMouseDown(object sender, MouseButtonEventArgs e) { insilico.OnRightMouseDown(sender, e); }
        private void MoveMouse(object sender, MouseEventArgs e) { insilico.OnMouseMove(sender, e); }
        private void ClickUp(object sender, RoutedEventArgs e) { insilico.OnClickUp(sender, e); }
        private void OnSizeChanged(object sender, RoutedEventArgs e) { insilico.OnSizeChanged(sender, e); }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e) { insilico.OnMouseWheel(sender, e); }
        #endregion
    }
}
