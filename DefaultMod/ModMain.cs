using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GooseShared;
using SamEngine;
using static DefaultMod.NewColor;

namespace DefaultMod {
    public class ModEntryPoint : IMod {
        [DllImport("user32.dll")] public static extern short GetAsyncKeyState(Keys vKey);
        public bool k(Keys k) { return GetAsyncKeyState(k) != 0; }
        Font menuFont = new Font("Courier New", 12f);
        Font miscFont = new Font("Segoe UI Light", 10f);
        Random rng = new Random();

        List<Color> colors = new List<Color>();
        Color cc = Color.Black;
        int clr = 0;

        bool menu = true;
        int i = 0;

        public void keyHook() {
            bool del = false,
                 up = false,
                 dn = false,
                 rg = false;

            while (true) {
                if (k(Keys.Insert)) {
                    if (!del) menu = !menu;
                    del = true;
                } else {
                    del = false;
                }

                if (menu) {
                    if (k(Keys.Up)) {
                        if (!up) if (i > 0) i--; else i = mgr.mods.Count - 1;
                        up = true;
                    } else {
                        up = false;
                    }

                    if (k(Keys.Down)) {
                        if (!dn) if (i < mgr.mods.Count - 1) i++; else i = 0;
                        dn = true;
                    } else {
                        dn = false;
                    }

                    if (k(Keys.Right)) {
                        if (!rg) {
                            mgr.mods[i].toggle();
                        }
                        rg = true;
                    } else {
                        rg = false;
                    }
                }
            }
        }

        ModManager mgr = new ModManager();

        void IMod.Init() {
            Thread t = new Thread(new ThreadStart(keyHook));
            t.Start();

            for (int i = 0; i < 360; i++) {
                RGB rgb = HSVToRGB(new HSV(i, 1.0, 1.0));
                colors.Add(Color.FromArgb(rgb.R, rgb.G, rgb.B));
            }

            string cfg = Path.Combine(API.Helper.getModDirectory(this), "..", "..", "..", "sizzurpmods.ini");

            Action quit = () => {
                Utils.putConfig(mgr.mods, cfg);
                Process.GetCurrentProcess().Kill();
            };

            Action reset = () => {
                if (health == 0f) health = 20f;
            };

            mgr.createMod("Follow Mouse");
            mgr.createMod("Disco Goose");
            mgr.createMod("Hyperspeed");
            mgr.createMod("Long Neck");
            mgr.createMod("Slowness");
            mgr.createMod("Anti-Aim");
            mgr.createBtn("[Exit]", quit);
            mgr.createBtn("[Respawn Goose]", reset);

            if (!File.Exists(cfg)) {
                MessageBox.Show("Welcome to SizzurpMods!\r\n" +
                                "Controls:\r\n" +
                                "INSERT - Toggle Menu\r\n" +
                                "Arrow Up/Down - Navigate Menu\r\n" +
                                "Arrow Left - Toggle/Use Mod",

                                "Welcome to SizurpMods!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                Utils.putConfig(mgr.mods, cfg);
            }

            try {
                Utils.loadConfig(ref mgr.mods, cfg);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            InjectionPoints.PreTickEvent += PreTick;
            InjectionPoints.PostTickEvent += PostTick;
            InjectionPoints.PostRenderEvent += DrawGUI;
            InjectionPoints.PostRenderEvent += DrawName;
            InjectionPoints.PostRenderEvent += DrawHealth;
        }

        public string rep(string s, int i) {
            string r = "";
            for (int a = 0; a < i; a++) {
                r += s;
            }
            return r;
        }

        float health = 20f;

        public void DrawGUI(GooseEntity g, Graphics e) {
            cc = colors[clr++ % colors.Count];

            if (menu) {
                string menuName = "// SizzurpMods GUI //";
                int h = (int)(e.MeasureString(menuName, menuFont).Height + 2);
                int w = (int)(e.MeasureString(menuName, menuFont).Width + 4);

                Pen ol = new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)));
                Brush bk = new SolidBrush(Color.FromArgb(0, 0, 0));
                Brush bg = new SolidBrush(Color.FromArgb(50, 50, 50));
                Brush sl = new SolidBrush(Color.FromArgb(75, 75, 75));
                Pen so = new Pen(new SolidBrush(Color.FromArgb(25, 25, 25)));
                Brush fg = new SolidBrush(Color.FromArgb(200, 200, 200));
                e.FillRectangle(bg, new Rectangle(2, 2, w, 1 + ((mgr.mods.Count + 1) * h) + 4));
                e.DrawRectangle(ol, new Rectangle(2, 2, w, 1 + ((mgr.mods.Count + 1) * h) + 4));

                int f = 1;
                foreach (char s in menuName) {
                    string c = rep(" ", f - 1) + s + rep(" ", menuName.Length - f);
                    Brush rb = new SolidBrush(colors[((clr * 9) + (f * ((f / colors.Count) + 10))) % colors.Count]);
                    e.DrawString(c, menuFont, bk, 6, 6);
                    e.DrawString(c, menuFont, rb, 5, 5);
                    f++;
                }
                e.DrawLine(ol, new Point(2, 2 + h), new Point(w + 1, 2 + h));

                for (int i = 0; i < mgr.mods.Count; i++) {
                    Mod mod = mgr.mods[i];

                    int x = 5;
                    int y = x + (h * (i + 1));
                    int nx = x;

                    if (this.i == i) {
                        e.DrawRectangle(so, new Rectangle(x - 1, y - 1, w - 4, h + 1));
                        e.FillRectangle(sl, new Rectangle(x, y, w - 5, h));
                    }

                    if (!mod.isFunc) {
                        string status = mod.Toggled ? "[X] " : "[ ] ";
                        Color statClr = mod.Toggled ? Color.FromArgb(0, 255, 0) : Color.FromArgb(255, 0, 0);

                        e.DrawString(status, menuFont, Brushes.Black, x + 1, y + 1);
                        e.DrawString(status, menuFont, new SolidBrush(statClr), x, y);

                        nx += (int)e.MeasureString(status, menuFont).Width;
                    }

                    e.DrawString(mod.Name, menuFont, Brushes.Black, nx + 1, y);
                    e.DrawString(mod.Name, menuFont, fg, nx, y);
                }
            }
        }

        bool first = true;
        public void DrawHealth(GooseEntity g, Graphics e) {
            if (health <= 0f) return;
            int h = 4;
            int w = 100;
            int x = (((int)g.position.x) - (w / 2));
            int y = (int)g.position.y + 20;

            Pen ol = new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)));
            Brush b1 = new SolidBrush(Color.FromArgb(0, 255, 0));
            Brush b2 = new SolidBrush(Color.FromArgb(255, 0, 0));
            try {
                e.FillRectangle(b2, new Rectangle(x, y, w, h));
                e.FillRectangle(b1, new Rectangle(x, y, (int)(w * (health / 20f)), h));
                e.DrawRectangle(ol, new Rectangle(x, y, w, h));
            } catch (Exception bb) {
                MessageBox.Show(bb.Message);
            }
        }
        public void DrawName(GooseEntity g, Graphics e) {
            if (health <= 0f) return;
            string name = "Mr. Goose";
            int w = (int)Math.Floor(e.MeasureString(name, miscFont).Width);
            int x = (((int)g.position.x) - ((int) (w / 1.3)));
            int y = (int)g.position.y - 80;

            try {
                e.DrawString(name, menuFont, Brushes.Black, new Point(x + 1, y + 1));
                e.DrawString(name, menuFont, Brushes.White, new Point(x, y));
            } catch (Exception bb) {
                MessageBox.Show(bb.Message);
            }
        }

        public void nulltask(GooseEntity g) { }

        public void PostTick(GooseEntity g) {
            if (g.currentTask == 4) {
                g.targetPos = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                health -= 0.5f;
                API.Goose.setTaskRoaming(g);
            }
        }

        public void PreTick(GooseEntity g) {
            if (first) {
                g.position = new Vector2(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            }
            first = false;

            if (mgr.getMod("Follow Mouse").Toggled) {
                if(g.currentTask == 0)
                    g.targetPos = new Vector2(Cursor.Position.X, Cursor.Position.Y);
            }

            if (mgr.getMod("Long Neck").Toggled) {
                g.rig.neckLerpPercent = 25;
            } else {
                g.rig.neckLerpPercent = 1;
            }
    
            if (mgr.getMod("Disco Goose").Toggled) {
                g.renderData.brushGooseWhite = new SolidBrush(cc);
                g.renderData.brushGooseOutline = new SolidBrush(Color.Black);
            } else {
                g.renderData.brushGooseWhite = new SolidBrush(Color.White);
                g.renderData.brushGooseOrange = new SolidBrush(Color.Orange);
                g.renderData.brushGooseOutline = new SolidBrush(Color.White);
            }

            if (mgr.getMod("Hyperspeed").Toggled) {
                g.currentSpeed = 10000f;
                g.currentAcceleration = 10000f;
                mgr.getMod("Slowness").Toggled = false;
            } else if (mgr.getMod("Slowness").Toggled) {
                g.currentSpeed = 10f;
                g.currentAcceleration = 10f;
            } else {
                g.currentSpeed = 80f;
                g.currentAcceleration = 1300f;
            }

            if (mgr.getMod("Anti-Aim").Toggled) {
                if (g.currentTask == 0)
                    g.direction = rng.Next(0, 360);
            }

            if (health <= 0f) {
                health = 0f;
                g.currentSpeed = 0f;
                g.currentAcceleration = 0f;
                g.targetPos = g.position;
                g.renderData.brushGooseWhite = new SolidBrush(Color.Transparent);
                g.renderData.brushGooseOrange = new SolidBrush(Color.Transparent);
                g.renderData.brushGooseOutline = new SolidBrush(Color.Transparent);
            } else if(!mgr.getMod("Disco Goose").Toggled) {
                g.renderData.brushGooseWhite = new SolidBrush(Color.White);
                g.renderData.brushGooseOrange = new SolidBrush(Color.Orange);
                g.renderData.brushGooseOutline = new SolidBrush(Color.White);
            }
        }
    }
}