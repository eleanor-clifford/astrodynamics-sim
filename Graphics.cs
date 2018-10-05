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
	static class Transforms {
		public static Vector3 Perspective(Vector3 true_position, Camera c) {
			return Matrix3.ExtrinsicZYXRotation(c.angle)*(true_position - c.position);
		}
	}
	class Camera {
		public Vector3 position {get; protected set;}
		public Vector3 angle {get; protected set;}
		public Camera(double distance, Vector3 angle) {
			this.angle = angle;
			position = Matrix3.IntrinsicZYXRotation(angle)*new Vector3(0,0,distance);
		}
	}
	class SystemView : DrawingArea {
		public PlanetarySystem sys {get; private set;}
		bool playing = false;
		private Vector3 bounds;
		public bool logarithmic {get; set;} = false;
		public double log_base {get; set;} = 1.5;
		private bool started {get; set;} = false;
		private double min_log;
		public Camera camera {get; set;} = new Camera(1*AU,new Vector3(80*deg,20*deg,0));
		public double bounds_multiplier {get; set;} = 0.5;
		public double radius_multiplier {get; set;} = 10;
		public double line_multiplier {get; set;} = 0.8;
		public double perspective_scale = 1;
		public int line_max {get; set;} = 100;
		private List<Vector3>[] paths;
		private int[] order;
		public void Redraw() {
			started = false;
		}
		protected override bool OnDrawn (Cairo.Context ctx) {
			ctx.SetSourceRGB(0,0,0);
			ctx.Paint();
			ctx.Translate(AllocatedWidth/2,AllocatedHeight/2);
			ctx.Scale(0.5,0.5);
			if (!started) {
				order = new int[sys.bodies.Count];
				for (int i = 0; i < sys.bodies.Count; i++) order[i] = i;
			}
			if (logarithmic) {
				if (!started) {
					min_log = 9e99;
					foreach (Body b in sys.bodies) {
						var p = b.position - sys.origin;
						if (b.name == "Sol") continue;
						if (Math.Log(Vector3.Magnitude(p),log_base) < min_log) {
							min_log = Math.Log(Vector3.Magnitude(p),log_base);
						}
					}
					started = true; 
				} 
				bounds = 0.5*Vector3.Log(sys.bounds, log_base);
			} else {
				if (!started) {
					max = 0;
					foreach (Body b in sys.bodies) {
						var p = Vector3.Magnitude(Transforms.Perspective(b.position,camera) - Transforms.Perspective(sys.origin,camera));
						var v = Transforms.Perspective(b.position,camera) - Transforms.Perspective(sys.origin,camera);
						if (p > max) {
							max = p;
						}
					}
					started = true;
				} bounds = bounds_multiplier * max * new Vector3(1,1,1);
			}
			var scale = Math.Min(AllocatedWidth/bounds.x,AllocatedHeight/bounds.y);
			ctx.Scale(scale,scale);
			if (paths == null) {
				paths = new List<Vector3>[sys.bodies.Count];
				for (int i = 0; i < sys.bodies.Count; i++) {
					paths[order[i]] = new List<Vector3>();
				}
			}
			order = order.OrderByDescending(x => Vector3.Magnitude(sys.bodies[x].position - camera.position)).ToArray();
			for (int i = 0; i < sys.bodies.Count; i++) {
				Body body = sys.bodies[order[i]];
				var r = radius_multiplier * body.radius * Math.Pow((Vector3.Magnitude(camera.position) / Vector3.Magnitude(body.position - camera.position)),perspective_scale);
				ctx.LineWidth = line_multiplier * radius_multiplier * body.radius;
				if (logarithmic) {
				//	r = Math.Log(body.radius/100,log_base)/100;
					r = body.radius*1e5;
				}
				Vector3 lastPath = Vector3.zero;
				try {
					lastPath = paths[order[i]][0];
				} catch (ArgumentOutOfRangeException) {};
				for (int j = -1; j < paths[order[i]].Count; j++) {
					
					Vector3 true_position;
					if (j == -1) true_position = body.position;
					else true_position = paths[order[i]][j];
					Vector3 pos;
					if (logarithmic) {
						var log_pos = Vector3.Log(true_position, log_base);
						pos = log_pos - min_log*Vector3.Unit(log_pos);
					} else {
						pos = Transforms.Perspective(true_position,camera) - Transforms.Perspective(sys.origin,camera);
					}
					var cl = body.reflectivity;//Vector3.zero;
					//if (body.name != "Sol") {
						//cl = Contrast(body.reflectivity);
					//} else {
					//	cl = Vector3.zero;
					//}
				
					ctx.SetSourceRGB (255*cl.x,255*cl.y,255*cl.z);
					if (j == -1) {
						ctx.Arc(pos.x,pos.y,r,0,2*Math.PI);
						ctx.Fill();
					}
					else if (j > 0) {
						ctx.MoveTo(lastPath.x,lastPath.y);
						ctx.LineTo(pos.x,pos.y);
						ctx.Stroke();
					} lastPath = pos;

				}
				paths[order[i]].Add(body.position);
				if (paths[order[i]].Count > line_max) {
					paths[order[i]].RemoveAt(0);
				}
			}
			return true;
		}
		protected double min = 1;
		protected double max = 0;
		protected Vector3 Contrast(Vector3 rgb) {
			if (rgb.x > max) max = rgb.x;
			if (rgb.x < min) min = rgb.x;
			if (rgb.y > max) max = rgb.y;
			if (rgb.y < min) min = rgb.y;
			if (rgb.z > max) max = rgb.z;
			if (rgb.z < min) min = rgb.z;
			var r = (rgb - min*new Vector3(1,1,1));
			r /= (max - min);
			return r;
		}
		
		public void Play(int interval) {
			playing = true;
			while (playing) {
				this.QueueDraw();
				Thread.Sleep(interval);
			}
		}
		public void Stop() {
			playing = false;
		}
		public SystemView(PlanetarySystem sys) {
			this.sys = sys;
		}
	}
}