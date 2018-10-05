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
using static Input;

class Program {
	public static PlanetarySystem activesys;
	public static SystemView sys_view;
	public static Task mechanics;
	public static double STEP = 100;
	
	static void Main(string[] args) {
		foreach (string s in args) {
			if (s == "--test") {
				if (Structures.Tests.MatrixTest()) Console.WriteLine("Matrix Tests Passed");
				else Console.WriteLine("Matrix Tests Failed");
				if (Structures.Tests.VectorTest()) Console.WriteLine("Vector Tests Passed");
				else Console.WriteLine("Vector Tests Failed");		
				if (Structures.Tests.BodyTest()) Console.WriteLine("Body Tests Passed");
				else Console.WriteLine("Body Tests Failed");
				//if (Mechanics.Tests.EarthSun()) Console.WriteLine("Earth-Sun Tests Passed");
				//else Console.WriteLine("Earth-Sun Tests Failed");
				PlanetarySystem solar_system = Structures.Examples.solar_system;
				foreach (List<Body> step in solar_system.Start(step: 1, verbose: true)) {
					//solar_system.Stop();
					continue;
				}
				
			} else if (s == "--gtk") {
				PlanetarySystem solar_system = Structures.Examples.solar_system;
				foreach (String s2 in args) {
					if (s2 == "--inner") {
						solar_system = Structures.Examples.inner_solar_system;
					}
				}
				solar_system.centers.Add(0);
				//solar_system.centers.Add(3);
				activesys = solar_system;
				foreach (String s2 in args) {
					if (s2 == "--blackhole") {
						Body blackhole = (Body)Structures.Examples.sun.Clone();
						blackhole.stdGrav *= 10;
						blackhole.name = "Black Hole 1";
						blackhole.reflectivity = new Vector3(1,0,0);
						blackhole.radius = solar_system.bodies[7].radius;
						blackhole.position = AU*new Vector3(-3,1,0);
						blackhole.velocity = new Vector3(60e3,0,0);
						solar_system.Add(blackhole);
						solar_system.centers.Add(solar_system.bodies.Count-1);
					} else if (s2 == "--binary") {
						double radius = 0.01*AU;
						Body sun = solar_system.bodies[0];
						sun.radius /= 100;
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
						earth.radius /= 100;
						solar_system.bodies.Clear();
						solar_system.Add(sun);
						solar_system.Add(sun2);
						solar_system.Add(earth);
						//solar_system.Add(mercury);
						solar_system.bounds = 0.2*AU*new Vector3(1,1,1);
						STEP = 1;
					}
				}
				Application.Init();
				Gtk.Window inputWindow = new Gtk.Window("Input");
				mechanics = Task.Run(() => solar_system.StartNoReturn(step: STEP, verbose: false));

				Gtk.Window mainWindow = new Gtk.Window("Astrodynamics Simulation");
				mainWindow.SetDefaultSize(1280,720);
				mainWindow.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask;
				mainWindow.DeleteEvent += delegate { Application.Quit (); };
				mainWindow.KeyPressEvent += Input.KeyPress;
				mainWindow.MotionNotifyEvent += Input.MouseMovement;
				mainWindow.ScrollEvent += Input.Scroll;
				var lg = false;
				foreach (String s2 in args) {
					if (s2 == "--logarithmic-position") lg = true;
				}
				if (lg) {
					sys_view = new SystemView(solar_system) {
						logarithmic = true,
						log_base = 1.000000000000001
					};
				} else {
					sys_view = new SystemView(solar_system);
					foreach (String s2 in args) {
						if (s2 == "--inner") {
							sys_view.camera = new Camera(50*AU,new Vector3(80*deg,20*deg,0));
							sys_view.radius_multiplier = 4;
							sys_view.line_multiplier = 0.8;
							sys_view.bounds_multiplier = 1;
							sys_view.perspective_scale = 0.5;
						}
					}
				}
				mainWindow.Add(sys_view);
				Task.Run(() => sys_view.Play(0));
				Program.activesys.ReCenterLocked(INTERVAL,null);
				mainWindow.ShowAll();
				Application.Run();
			}
		}
	}
}