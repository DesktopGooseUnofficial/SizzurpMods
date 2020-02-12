using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultMod {
    class Mod {
        public string Name;
        public bool Toggled = false;
        public bool Enabled = true;
        public bool isFunc;
        private Action Func;

        public void toggle() {
            if (!Enabled) return;
            if (isFunc)
                Func();
            else
                Toggled = !Toggled;
        }

        public Mod(string Name, bool isFunc = false) {
            this.Name = Name;
            this.isFunc = isFunc;
        }

        public Mod(string Name, Action Func) {
            this.Name = Name;
            this.Func = Func;
            isFunc = true;
        }
    }
}
