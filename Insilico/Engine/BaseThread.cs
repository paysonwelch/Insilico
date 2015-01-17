using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Insilico {

    /// <summary>
    /// Custom Thread object.
    /// </summary>
    public abstract class BaseThread {

        private Thread _thread;

        public BaseThread() { _thread = new Thread(new ThreadStart(this.RunThread)); }

        // Thread methods / properties
        public void Start() {
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start(); }
        public void Join() { _thread.Join(); }
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

        public bool _shouldStop = false;
        public bool _shouldPause = false;
        public bool _shouldSleep = false;
        public bool _shouldRestart = false;

        public int _sleepTime = 0;

        public void Restart() {
            _shouldRestart = true;
        }

        public void Stop() {
            _shouldStop = true;
        }

        public void Pause() {
            _shouldPause = true;
        }

        public void Resume() {
            _shouldPause = false;
        }

        public bool IsPaused() {
            return _shouldPause;
        }

        public void Sleep(int sleepTime) {
            _sleepTime = sleepTime;
            _shouldSleep = true;
        }

        // Override in base class
        public abstract void RunThread();
    }
}
