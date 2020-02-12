using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultMod {
    class Utils {
		public static void loadConfig(ref List<Mod> mods, string path) {
			if (!File.Exists(path)) return;
			Dictionary<string, bool> cfg = new Dictionary<string, bool>();
			using (StreamReader streamReader = new StreamReader(path)) {
				string text;
				while ((text = streamReader.ReadLine()) != null) {
					string[] array = text.Split('=');
					if (array.Length == 2) {
						cfg.Add(array[0], bool.Parse(array[1]));
					}
				}
			}
			foreach(Mod mod in mods) {
				if (mod.isFunc || !cfg.Keys.Contains(mod.Name)) continue;
				mod.Toggled = cfg[mod.Name];
			}
		}
		public static void putConfig(List<Mod> mods, string path) {
			using (StreamWriter streamWriter = new StreamWriter(path)) {
				streamWriter.WriteLine("[SizzurpMods Config File]");
				foreach (Mod mod in mods) {
					if (mod.isFunc) continue;
					streamWriter.WriteLine(mod.Name + "=" + mod.Toggled);
				}
			}
		}
	}
}
