using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Structures;
using Mechanics;
using StartupScreen;
using static Constants;
using Gtk;
using Gdk;
using Cairo;
using Graphics;
using static Input;

static class Program {
	public static PlanetarySystem activesys;
	public static SystemView sys_view;
	public static Task mechanics;
	public static double STEP;
	public static List<Boolean> RadioOptions;
	public static List<Body> CustomBodies = new List<Body>();
	public static void Start() {
		//RadioOptions = new List<Boolean>() {true,false,false,false,false};
		PlanetarySystem solar_system;
		if (RadioOptions[4]) {
			solar_system = new Structures.PlanetarySystem(CustomBodies) {
				bounds = 50*AU*new Vector3(1,1,1),
			};
			foreach (Body b in CustomBodies) Console.Write($"{b.name} ");
			Console.WriteLine();
			//Thread.Sleep(10000);
		}
		else if (RadioOptions[2]) { // inner
			solar_system = Structures.Examples.inner_solar_system;
		} else {
			solar_system = Structures.Examples.solar_system;
		}
		solar_system.centers.Add(0);
		activesys = solar_system;
		if (RadioOptions[1]) { // blackhole
				Body blackhole = (Body)Structures.Examples.sun.Clone();
				blackhole.stdGrav *= 10;
				blackhole.name = "Black Hole 1";
				blackhole.reflectivity = new Vector3(1,0,0);
				blackhole.radius = solar_system.bodies[7].radius;
				blackhole.position = AU*new Vector3(-3,1,0);
				blackhole.velocity = new Vector3(60e3,0,0);
				solar_system.Add(blackhole);
				solar_system.centers.Add(solar_system.bodies.Count-1);
		} else if (RadioOptions[3]) { // Binary Star System
				double radius = 0.01*AU;
				Body sun = solar_system.bodies[0];
				sun.radius /= 500;
				Body sun2 = (Body)sun.Clone();
				sun.stdGrav /= 2;
				sun2.stdGrav = sun.stdGrav;
				sun.position = new Vector3(0,radius,0);
				sun2.position = -sun.position;
				sun.velocity = Math.Sqrt(sun.stdGrav/(2*radius))/2 * Vector3.i;
				sun2.velocity = -sun.velocity;

				Body earth = solar_system.bodies[3];
				//Body mercury = solar_system.bodies[1];
				earth.position = Vector3.zero;
				earth.velocity = Vector3.zero;
				earth.radius /= 1000;
				solar_system.bodies.Clear();
				solar_system.Add(sun);
				solar_system.Add(sun2);
				solar_system.Add(earth);
				//solar_system.Add(mercury);
				solar_system.bounds = 0.2*AU*new Vector3(1,1,1);
		}
		Gtk.Window mainWindow = new Gtk.Window("Astrodynamics Simulation");
		mainWindow.SetDefaultSize(1280,720);
		mainWindow.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask;
		mainWindow.DeleteEvent += delegate { Application.Quit (); };
		mainWindow.KeyPressEvent += Input.KeyPress;
		mainWindow.MotionNotifyEvent += Input.MouseMovement;
		mainWindow.ScrollEvent += Input.Scroll;
		sys_view = new SystemView(solar_system);
		if (RadioOptions[2] || RadioOptions[1] || RadioOptions[4]) { // inner or custom
			sys_view.camera = new Camera(50*AU,new Vector3(0,0,0));
			sys_view.radius_multiplier = 4;
			sys_view.line_multiplier = 0.8;
			sys_view.bounds_multiplier = 1;
			sys_view.perspective_scale = 0.5;
		}
		mainWindow.Add(sys_view);
		solar_system.StartNoReturn(step: STEP, verbose: false); // Start Mechanics
		Task.Run(() => sys_view.Play(0)); // Start Display
		Program.activesys.ReCenterLocked(INTERVAL,null); // Lock camera
		mainWindow.ShowAll();
	}
	
	static void Main(string[] args) {
		Application.Init();
		var menu = new StartupScreen.Menu();
		Application.Run();
	}
}
