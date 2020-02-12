using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultMod {
    class ModManager {
        public List<Mod> mods = new List<Mod>();
        public void createMod(string Name) {
            mods.Add(new Mod(Name));
        }

        public void createBtn(string Name, Action Func) {
            mods.Add(new Mod(Name, Func));
        }

        public Mod getMod(string Name) {
            return mods.Find(m => m.Name == Name);
        }
    }
}
