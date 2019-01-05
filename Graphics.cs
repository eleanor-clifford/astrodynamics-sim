using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using Cairo;
using Structures;
using System.Threading;
using System.Threading.Tasks;
using static Constants;
namespace Graphics {
	class Camera {
		public Vector3 position {get; protected set;}
		public Vector3 angle {get; protected set;}
		public Camera(double distance, Vector3 angle) {
			// the camera always "points" to the origin
			this.angle = angle;
			position = Matrix3.IntrinsicZYXRotation(angle)*new Vector3(0,0,distance);

		}
		public Vector3 Transform(Vector3 position) {
			return Matrix3.ExtrinsicZYXRotation(this.angle)*(position - this.position);
		}
	}
	class SystemView : DrawingArea {
		public PlanetarySystem sys {get; private set;}
		bool playing = false;
		public Camera camera {get; set;} = new Camera(1*AU,Vector3.zero);
		public double bounds_multiplier {get; set;} = 0.5;
		public double radius_multiplier {get; set;} = 1;
		public double line_multiplier {get; set;} = 0.8;
		public double perspective_scale = 0;
		public int line_max {get; set;} = 100;
		public List<Vector3>[] paths;
		private int[] order;
		protected double max = 0;
		public SystemView(PlanetarySystem sys) {
			this.sys = sys;
			Redraw();
		}
		public void Redraw() {
			order = new int[sys.bodies.Count];
			for (int i = 0; i < sys.bodies.Count; i++) order[i] = i;
			max = 0;
			foreach (Body b in sys.bodies) {
				var p = Vector3.Magnitude(camera.Transform(b.position) - camera.Transform(sys.origin));
				if (p > max) {
					max = p;
				}
			} 
		}
		public void Play(int interval) {
			playing = true;
			while (playing) {
				this.QueueDraw();
				Thread.Sleep(interval);
			}
		}
		public void PlayAsync(int interval) {
			Task.Run(() => Play(interval));
		}
		public void Stop() {
			playing = false;
		}
		protected override bool OnDrawn (Cairo.Context ctx) {
			ctx.SetSourceRGB(0,0,0);
			ctx.Paint();
			ctx.Translate(AllocatedWidth/2,AllocatedHeight/2);
			ctx.Scale(0.5,0.5);
			var bounds = bounds_multiplier * max * new Vector3(1,1,1);
			var scale = Math.Min(AllocatedWidth/bounds.x,AllocatedHeight/bounds.y);
			ctx.Scale(scale,scale);
			if (paths == null) {
				paths = new List<Vector3>[sys.bodies.Count];
				for (int i = 0; i < sys.bodies.Count; i++) {
					paths[order[i]] = new List<Vector3>();
				}
			}
			Vector3 origin;
			if (Program.activesys.center_index == -1) origin = Program.activesys.Barycenter();
			else origin = Program.activesys.bodies[Program.activesys.centers[Program.activesys.center_index]].position;

			order = order.OrderByDescending(x => Vector3.Magnitude(sys.bodies[x].position - camera.position)).ToArray();
			for (int i = 0; i < sys.bodies.Count; i++) {
				Body body = sys.bodies[order[i]];
				var r = radius_multiplier * body.radius * Math.Pow((Vector3.Magnitude(camera.position) / Vector3.Magnitude(body.position - camera.position)),perspective_scale);
				ctx.LineWidth = line_multiplier * radius_multiplier * body.radius;
				Vector3 lastPath = Vector3.zero;
				try {
					lastPath = paths[order[i]][0];
				} catch (ArgumentOutOfRangeException) {};
				for (int j = -1; j < paths[order[i]].Count; j++) {
					
					Vector3 true_position;
					if (j == -1) true_position = body.position;//origin;
					else true_position = paths[order[i]][j] + origin;
					Vector3 pos;
					pos = camera.Transform(true_position) - camera.Transform(origin);
					var cl = body.reflectivity;
					ctx.SetSourceRGB (cl.x,cl.y,cl.z);
					//Console.WriteLine(Vector3.LogByComponent(lastPath));
					if (j == -1) {
						ctx.Arc(pos.x,pos.y,r,0,2*Math.PI);
						ctx.Fill();
					}
					else if (j > 0) {
						//lastPath -= camera.Transform(origin);
						ctx.MoveTo(lastPath.x,lastPath.y);
						ctx.LineTo(pos.x,pos.y);
						ctx.Stroke();
					} lastPath = pos;// + camera.Transform(origin);

				}
				paths[order[i]].Add(body.position - origin);
				if (paths[order[i]].Count > line_max + 1) {
					paths[order[i]].RemoveAt(0);
					paths[order[i]].RemoveAt(0);
					// remove faster than they can be created if line_max has been reduced
				}
				else if (paths[order[i]].Count > line_max) {
					paths[order[i]].RemoveAt(0);
				}
			}
			return true;
		}
	}
}