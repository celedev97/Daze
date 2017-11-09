﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    public class Timer {
        #region Variables and properties
        private int _ID;
        public int ID { get => _ID; }

        private int _MSPerTick;
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

        internal float currentMS;

        public Action tickAction;

        public bool restartFlag;
        #endregion

        #region Constructors
        public Timer(int timerID, int msPerTick, Action tickAction, bool restartFlag = true, int currentMS = 0) {
            this.restartFlag = restartFlag;
            this.tickAction = tickAction;
            _ID = timerID;
            set(msPerTick, currentMS);
        }
        #endregion

        #region Methods
        public bool ticked() {
            bool output;
            if((output = msPerTick <= currentMS) && restartFlag) {
                restart();
            }
            return output;
        }

        public void restart() {
            currentMS = 0;
        }

        public void set(int msPerTick, int currentMS = 0) {
            this.msPerTick = msPerTick;
            this.currentMS = currentMS;
        }

        #endregion
    }
}
