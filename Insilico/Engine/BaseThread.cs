using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Insilico {

    /// <summary>Custom Thread object</summary>
    public abstract class BaseThread {

        private Thread _thread;

        public BaseThread() { _thread = new Thread(new ThreadStart(this.RunThread)); }

        /// <summary>Starts the thread</summary>
        public void Start() {
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        /// <summary>Join</summary>
        public void Join() { _thread.Join(); }

        /// <summary>Returns whether the thread is alive</summary>
        public bool IsAlive { get { return _thread.IsAlive; } }

        protected bool isFree = true;
        public bool IsFree {
            get {
                return isFree;
            }
        }

        private bool isImmutable = false;

        public bool Immutable {
            get { return isImmutable; }
            set { isImmutable = value; }
        }

        protected bool _shouldStop = false;
        protected bool _shouldPause = false;
        protected bool _shouldSleep = false;
        protected bool _shouldRestart = false;

        protected int _sleepTime = 0;

        /// <summary>Restarts the thread</summary>
        public void Restart() {
            _shouldRestart = true;
        }

        /// <summary>Stops the thread</summary>
        public void Stop() {
            _shouldStop = true;
        }

        /// <summary>Pauses the thread</summary>
        public void Pause() {
            _shouldPause = true;
        }

        /// <summary>Resumes the thread</summary>
        public void Resume() {
            _shouldPause = false;
        }

        /// <summary>Returns whether the thread is paused</summary>
        public bool IsPaused() {
            return _shouldPause;
        }

        /// <summary>Sends the thread to sleep for a given number of milliseconds</summary>
        public void Sleep(int sleepTime) {
            _sleepTime = sleepTime;
            _shouldSleep = true;
        }

        /// <summary>Main execution loop. Override this and fill it with the code you wish to execute</summary>
        public abstract void RunThread();
    }
}
