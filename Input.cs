using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Structures;
using Mechanics;
using static Constants;
using Gtk;
using Gdk;
using Cairo;
using Graphics;
using static Program;

static class Input {
    public static int INTERVAL = 5;
    [GLib.ConnectBefore]
	public static void KeyPress(object sender, KeyPressEventArgs args) {
		if (args.Event.Key == Gdk.Key.f) {
			if (Program.activesys == null) return;
			else {
                Program.activesys.center_index += 1;
                if (Program.activesys.center_index >= Program.activesys.centers.Count) {
                    Program.activesys.center_index = -1;
                }
                //Thread.Sleep(1000000);
                Program.activesys.UnlockCenter();
                if (Program.activesys.center_task != null) {
                    Program.activesys.center_task.Wait();
                }
                if (Program.activesys.center_index != -1) Program.activesys.ReCenterLocked(INTERVAL,Program.activesys.bodies[Program.activesys.centers[Program.activesys.center_index]]);
                else Program.activesys.ReCenterLocked(INTERVAL,null);
            }
        }
		if (args.Event.Key == Gdk.Key.l) {

        }
	}
}