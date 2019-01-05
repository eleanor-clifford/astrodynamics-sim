using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Structures;
using static Constants;
using Gtk;
using Gdk;
using Cairo;
using Graphics;
using static Program;

static class Input {
    private static bool canMove = false;
    private static Vector3 rootPos = null;
    private static Vector3 rootAngle = null;
    public static int INTERVAL = 0;
    public static double mouse_sensitivity {get; set;} = 1;
    public static double scroll_sensitivity {get; set;} = 1.1;
    public static double time_sensitivity = 1.2;
    public static double radius_sensitivity = 1.1;
    public static int line_sensitivity = 5;
    [GLib.ConnectBefore]
	public static void KeyPress(object sender, KeyPressEventArgs args) {
		if (args.Event.Key == Gdk.Key.f) {
			if (Program.activesys == null) return;
			else {
                Program.activesys.center_index += 1;
                if (Program.activesys.center_index >= Program.activesys.centers.Count) {
                    Program.activesys.center_index = -1;
                }
                Program.sys_view.paths = new List<Vector3>[Program.activesys.bodies.Count];
				for (int i = 0; i < Program.activesys.bodies.Count; i++) {
					Program.sys_view.paths[i] = new List<Vector3>();
				}
            }
            args.RetVal = true;
        } else if (args.Event.Key == Gdk.Key.r) {
            double d = Vector3.Magnitude(Program.sys_view.camera.position);
            Program.sys_view.camera = new Camera(d,Vector3.zero);
        } else if (args.Event.Key == Gdk.Key.l) {
            canMove = !canMove;
            if (!canMove) {
                rootPos = null;
            }
        } else if (args.Event.Key == Gdk.Key.Up) {
            Program.sys_view.radius_multiplier *= radius_sensitivity;
        } else if (args.Event.Key == Gdk.Key.Down) {
            Program.sys_view.radius_multiplier /= radius_sensitivity;
        } else if (args.Event.Key == Gdk.Key.Right) {
            Program.activesys.Stop();
            Program.STEP *= time_sensitivity;
            Program.activesys.StartAsync(step: Program.STEP);
        } else if (args.Event.Key == Gdk.Key.Left) {
            Program.activesys.Stop();
            Program.STEP /= time_sensitivity;
            Program.activesys.StartAsync(step: Program.STEP);
        } else if (args.Event.Key == Gdk.Key.Page_Down) {
            // don't make it smaller than 0
            if (Program.sys_view.line_max >= line_sensitivity) {
                Program.sys_view.line_max -= line_sensitivity;
            }
        } else if (args.Event.Key == Gdk.Key.Page_Up) {
            Program.sys_view.line_max += line_sensitivity;
        } else if (args.Event.Key == Gdk.Key.Escape) {
            Console.WriteLine(Program.activesys.bodies.Count);
            Program.sys_view.Stop();
            Program.activesys.Stop();
            Program.mainWindow.Destroy();
            
            var menu = new StartupScreen.Menu();
            var data = new StartupScreen.SaveData() {
                bodies = Program.activesys.bodies,
                centers = Program.CustomCenters,
                STEP = Program.STEP,
                radius_multiplier = Program.sys_view.radius_multiplier,
                line_max = Program.sys_view.line_max
            };
            menu.temp_savedata = data;
            menu.loadButton.Click();
        }

	}
    [GLib.ConnectBefore]
    public static void MouseMovement(Object sender, MotionNotifyEventArgs args) {
        if (canMove) {
            if (rootPos == null || rootAngle == null ) {
                rootPos = new Vector3(args.Event.X,args.Event.Y,0);
                rootAngle = Program.sys_view.camera.angle;
            } else {
                double d = Vector3.Magnitude(Program.sys_view.camera.position);
                Program.sys_view.camera = new Camera(d,rootAngle + deg*mouse_sensitivity* new Vector3(rootPos.y - args.Event.Y,args.Event.X - rootPos.x, 0));
            } args.RetVal = true;
        }
    }
    [GLib.ConnectBefore]
    public static void Scroll(Object sender, ScrollEventArgs args) {
        if (args.Event.Direction == Gdk.ScrollDirection.Up) {
            Program.sys_view.bounds_multiplier /= scroll_sensitivity;
        } else if (args.Event.Direction == Gdk.ScrollDirection.Down) {
            Program.sys_view.bounds_multiplier *= scroll_sensitivity;
        }
    }
}