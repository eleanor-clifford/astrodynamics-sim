using System;
using System.Collections.Generic;
using static Constants;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace Structures
{
    public class PlanetarySystem {
		protected bool running = false;
		public List<Body> bodies {get; protected set;}
		public List<int> centers {get; set;} = new List<int>();
		// -1 indicates space is not locked
		public int center_index = -1;
		public Task center_task {get; private set;}
		CancellationTokenSource center_task_source;
		CancellationToken center_task_token;
		public Vector3 origin {get; private set;} = Vector3.zero;
		public Vector3 bounds {get; set;}
		public PlanetarySystem(List<Body> bodies = null) {
			if (bodies == null) this.bodies = new List<Body>();
			else this.bodies = bodies;
		}
		public void Add(Body body) {
			bodies.Add(body);
		}
		public void ReCenter(Vector3 position) {
			origin = position;
		}
		public void ReCenterLocked(int interval, Body center) {
			center_task_source = new CancellationTokenSource();
			center_task_token = center_task_source.Token;
			center_task = Task.Run(() => {
				while (true) {
					if (center == null) {
						//his.Stop();
						ReCenter(this.Barycenter());
						//Program.mechanics = this.StartNoReturn(Program.STEP,false)
					} else {
						//Console.WriteLine(center.name);
						ReCenter(center.position);
					}
					if (interval != 0) Thread.Sleep(interval);
					if (center_task_token.IsCancellationRequested) {
						break;
					}
				}
			},center_task_token);
		}
		public void UnlockCenter() {
			if (center_task_source != null) {
				center_task_source.Cancel();
			}
		}
		public Vector3 Barycenter() {
			Vector3 weighted_center = Vector3.zero;
			double mu_total = 0;
			foreach (Body b in this.bodies) {
				mu_total += b.stdGrav;
				weighted_center += b.stdGrav*b.position;
			}
			return weighted_center/mu_total;
		}
		protected Vector3[] GetAcceleration() {
			Vector3[] acceleration = new Vector3[this.bodies.Count];
			// Initialise our array to Vector3.zero, since the default is a null pointer.
			Parallel.For (0, this.bodies.Count, i => {
				acceleration[i] = Vector3.zero;
			});
			//Parallel.For (0, this.bodies.Count, i => {
			for (int i = 0; i < this.bodies.Count; i++) {
				Body body1 = this.bodies[i]; // We will need the index later so foreach is not possible
				//Parallel.For ( i+1, this.bodies.Count, j=> {
				for (int j = i + 1; j < this.bodies.Count; j++) {
					Body body2 = this.bodies[j]; // Again here
					// The magnitude of the force, multiplied by G, = %mu_1 * %mu_2 / r^2
					double mag_force_g = body1.stdGrav * body2.stdGrav / Math.Pow(Vector3.Magnitude(body1.position - body2.position),2);
					// We lost direction in the previous calculation (since we had to square the vector), but we need it.
					Vector3 direction = (body1.position - body2.position);
					direction /= Vector3.Magnitude(direction);
					// since acceleration is F/m, and we have G*F and G*m, we can find an acceleration vector easily
					Vector3 acceleration1 =  mag_force_g * -direction / body1.stdGrav;
					Vector3 acceleration2 = mag_force_g * direction / body2.stdGrav;
					acceleration[i] += acceleration1;
					acceleration[j] += acceleration2;
				}//);
			}//);
			return acceleration;
		}
		protected void TimeStep(double step) {
			var acceleration = this.GetAcceleration();
			//Parallel.For (0, acceleration.Length, i=> {
			for (int i = 0; i < acceleration.Length; i++) {
				Body body = this.bodies[i];
				Vector3 a = acceleration[i];
				body.position += step*body.velocity + Math.Pow(step,2)*a/2;
				body.velocity += step*a;
			}//);
		}
		public void StartNoReturn(double step = 1, bool verbose = false) {
			Task.Run(() => {foreach (var b in Start(step,verbose)) continue;});
		}
		public IEnumerable<List<Body>> Start(double step = 1, bool verbose = false) {
			this.running = true;
			int i = -1;
			bool[] half = new bool[this.bodies.Count];
			Vector3[] initialPosition = new Vector3[this.bodies.Count];
    		Vector3[] lastPosition = new Vector3[this.bodies.Count];
			if (verbose) {
				for (int j = 0; j < this.bodies.Count; j++) {
					half[j] = false;
					initialPosition[j] = this.bodies[j].position;
					lastPosition[j] = this.bodies[j].position;
				}
				Console.WriteLine("Starting System");
			}
			while (running) {
				i++;
                if (verbose && i%1000 == 0) {
					//for (int j = 0; j < this.bodies.Count; j++) {
					for (int j = this.bodies.Count - 1; j >= 0; j--) {
						var b = this.bodies[j];			
	    				Console.WriteLine($"{b.name}:\n\tPosition: {b.position}\n\tVelocity: {b.velocity}");
    					if (b.parent != null) {
							var acuteAngle = Math.Acos(Vector3.UnitDot(b.angleReference,b.position));
							var acuteAngleVelocity = Math.Acos(Vector3.UnitDot(b.angleReference,b.velocity));
    						Console.WriteLine($"\tPosition Angle: {(half[j] ? 2*Math.PI - acuteAngle : acuteAngle)/deg} deg");
    						Console.WriteLine($"\tVelocity Angle: {acuteAngleVelocity/deg} deg");
							Console.WriteLine($"\tOrbital Radius {Vector3.Magnitude(b.position)/AU} AU\n");
						} else {
							Console.WriteLine($"\tDistance from origin: {Vector3.Magnitude(b.position)/AU} AU");
						}
						if (!half[j] && Vector3.Magnitude(lastPosition[j] - initialPosition[j]) > Vector3.Magnitude(b.position - initialPosition[j]))
		   	 			{
   			 				half[j] = true;
		    			}
   		 				if (half[j] && Vector3.Magnitude(lastPosition[j] - initialPosition[j]) < Vector3.Magnitude(b.position - initialPosition[j]))
    					{
    						half[j] = false;
						}
						lastPosition[j] = b.position;
					}	
				}
				this.TimeStep(step);
				yield return this.bodies;
			}
		}
		public void Stop() {
			this.running = false;
		}
	}
}