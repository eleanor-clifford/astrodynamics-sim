using System;
using Gtk;
using Cairo;
using Structures;
using System.Threading;
using System.Threading.Tasks;
namespace Graphics {
	class SystemView : DrawingArea {
		public PlanetarySystem sys {get; private set;}
		bool playing = false;
		public bool logarithmic {get; set;} = false;
		public double log_base {get; set;} = 1.5;
		private bool started {get; set;} = false;
		private double min_log;
		public void Redraw() {
			started = false;
		}
		protected override bool OnDrawn (Cairo.Context ctx) {
			ctx.SetSourceRGB(0,0,0);
			ctx.Paint();
			ctx.Translate(AllocatedWidth/2,AllocatedHeight/2);
			Vector3 bounds;

			if (logarithmic) {
				if (!started) {
					min_log = 9e99;
					foreach (Body b in sys.bodies) {
						if (b.name == "Sol") continue;
						if (Math.Log(Vector3.Magnitude(b.position),log_base) < min_log) {
							min_log = Math.Log(Vector3.Magnitude(b.position),log_base);
						}
					}
					started = true; 
				} 
				bounds = 0.5*Vector3.Log(sys.bounds, log_base);
			} else {
				bounds = sys.bounds; 
			}
			var scale = Math.Min(AllocatedWidth/bounds.x,AllocatedHeight/bounds.y);
			ctx.Scale(scale,scale);
			
			foreach (Body body in sys.bodies) {
				Vector3 pos;
				if (logarithmic) {
					var log_pos = Vector3.Log(body.position, log_base);
					pos = log_pos - min_log*Vector3.Unit(log_pos);
				} else {
					pos = body.position;
				}
				var cl = Vector3.zero;
				//if (body.name != "Sol") {
					cl = Contrast(body.reflectivity);
				//} else {
				//	cl = Vector3.zero;
				//}
				var r = body.radius*10;
				if (logarithmic) {
				//	r = Math.Log(body.radius/100,log_base)/100;
					r = body.radius*1e5;
				}
				ctx.SetSourceRGB (255*cl.x,255*cl.y,255*cl.z);
				ctx.Arc(pos.x,pos.y,r,0,2*Math.PI);
				var s = pos/(Math.Max(AllocatedWidth,AllocatedHeight));
				//Console.WriteLine($"{body.name}:\n\t{Vector3.CartesianToPolar(pos)}, {Vector3.Magnitude(bounds)}\n\t{Vector3.CartesianToPolar(body.position)},{Vector3.Magnitude(sys.bounds)}");
				//Console.WriteLine(min_log);
				ctx.Fill();
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