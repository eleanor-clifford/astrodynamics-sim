using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Structures;
using UI;
using static Program.Constants;
using Gtk;
using Gdk;
using Cairo;
using Graphics;
namespace Program {
    static class Program {
        public static PlanetarySystem activesys {get; set;}
        public static SystemView sys_view {get; set;} = null;
        public static double timestep {get; set;}
        //public static List<Boolean> RadioOptions {get; set;}
        public static List<Body> CustomBodies {get; set;} = new List<Body>();
        public static List<bool> CustomCenters {get; set;} = new List<bool>();
        public static Gtk.Window mainWindow {get; set;}
        public static double radius_multiplier {get; set;}
        public static int line_max {get; set;}
        public static void Start() {
            activesys = new Structures.PlanetarySystem(CustomBodies);
            if (activesys.centers == null) activesys.centers = new List<int>();
            activesys.centers.Clear();
            for (int i = 0; i < CustomCenters.Count; i++) {
                if (CustomCenters[i]) activesys.centers.Add(i);
            }
            mainWindow = new Gtk.Window("Astrodynamics Simulation");
            mainWindow.SetDefaultSize(1280,720);
            mainWindow.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask;
            mainWindow.DeleteEvent += delegate { Application.Quit (); };
            mainWindow.KeyPressEvent += Input.KeyPress;
            mainWindow.MotionNotifyEvent += Input.MouseMovement;
            mainWindow.ScrollEvent += Input.Scroll;
            sys_view = new SystemView(activesys);
            sys_view.radius_multiplier = radius_multiplier;
            sys_view.line_max = line_max;
            mainWindow.Add(sys_view);
            activesys.StartAsync(step: timestep); // Start Mechanics
            sys_view.PlayAsync(interval: 0); // Start Display
            mainWindow.ShowAll();
    }
    
        static void Main(string[] args) {
            try {
                Application.Init();
                var menu = new UI.Menu();
                Application.Run();
            } catch (Exception e) {
                Console.WriteLine("An unexpected error occured");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
