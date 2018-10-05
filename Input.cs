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
    private static bool canMove = false;
    private static Vector3 rootPos = null;
    private static Vector3 rootAngle = null;
    public static int INTERVAL = 5;
    public static double mouse_sensitivity {get; set;} = 1;
    public static double scroll_sensitivity {get; set;} = 0.01;
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
            args.RetVal = true;
        }
		if (args.Event.Key == Gdk.Key.l) {
            canMove = !canMove;
            if (!canMove) {
                rootPos = null;
            }
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
            Program.sys_view.bounds_multiplier -= scroll_sensitivity;
        } else if (args.Event.Direction == Gdk.ScrollDirection.Down) {
            Program.sys_view.bounds_multiplier += scroll_sensitivity;
        }
    }
}