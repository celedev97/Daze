using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    /// <summary>
    /// A timer of a GameObject, gameObject can have several timers, you can edit them by using gameObject.createTimer and so on.
    /// </summary>
    public class Timer {
        #region Variables and properties
        private int _ID;

        /// <summary>
        /// The ID of the timer
        /// </summary>
        public int ID { get => _ID; }

        private int _MSPerTick;
        /// <summary>
        /// The number of MS of the duration of this timer
        /// </summary>
        public int msPerTick {
            get => _MSPerTick;
            set {
                if(value > 0) {
                    _MSPerTick = value;
                } else {
                    throw new Exception("Specified MS for Timer are less than 1");
                }
            }
        }

        /// <summary>
        /// The current milliseconds of this timer
        /// </summary>
        public float currentMS;

        /// <summary>
        /// The action that this timer execute every time it reach the msPerTick number of milliseconds
        /// </summary>
        public Action tickAction;

        /// <summary>
        /// If this flag is set to true the timer will automatically restart after it ticks,
        /// otherwise you will have to start it again manually
        /// </summary>
        public bool restartFlag;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerID">The ID of the timer</param>
        /// <param name="msPerTick">The number of MS of the duration of this timer</param>
        /// <param name="tickAction">The action that this timer execute every time it reach the msPerTick number of milliseconds</param>
        /// <param name="restartFlag"></param>
        /// <param name="currentMS"></param>
        public Timer(int timerID, int msPerTick, Action tickAction = null, bool restartFlag = true, int currentMS = 0) {
            this.restartFlag = restartFlag;
            this.tickAction = tickAction;
            _ID = timerID;
            Set(msPerTick, currentMS);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Return true if the time ticked
        /// NOTE: this method will also restart the timer if the resetFlag is true
        /// </summary>
        /// <returns></returns>
        public bool ticked() {
            bool output;
            if((output = msPerTick <= currentMS) && restartFlag) {
                Restart();
            }
            return output;
        }

        /// <summary>
        /// This method restart the timer.
        /// </summary>
        public void Restart() {
            currentMS = 0;
        }

        /// <summary>
        /// This method can be used to set the timer again in one line
        /// </summary>
        /// <param name="msPerTick"></param>
        /// <param name="currentMS"></param>
        public void Set(int msPerTick, int currentMS = 0) {
            this.msPerTick = msPerTick;
            this.currentMS = currentMS;
        }
        #endregion
    }
}
