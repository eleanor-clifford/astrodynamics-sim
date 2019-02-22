using System;
using System.Collections;
using System.Collections.Generic;
using static Program.Constants;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace Structures
{
    public class PlanetarySystem : IEnumerable<Body> {
		protected bool running = false;
		protected List<Body> bodies;
		public List<int> centers {get; set;} = new List<int>();
		// -1 indicates space is not locked
		public int center_index = -1;
		public PlanetarySystem(List<Body> bodies = null) {
			if (bodies == null) this.bodies = new List<Body>();
			else this.bodies = bodies;
		}


		public Body this[int key] {
    		get {
        		return this.bodies[key];
    		}
		}
		public IEnumerator<Body> GetEnumerator() { return this.bodies.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return this.bodies.GetEnumerator(); }
		public int Count {
			get {
				return this.bodies.Count;
			}
		}
		public void Add(Body body) {
			bodies.Add(body);
		}
		public Vector3 Barycenter() {
			Vector3 weighted_center = Vector3.zero;
			double mu_total = 0;
			foreach (Body b in this) {
				mu_total += b.stdGrav;
				weighted_center += b.stdGrav*b.position;
			}
			return weighted_center/mu_total;
		}
		public void IterateCenter() {
			this.center_index += 1;
            if (this.center_index >= this.centers.Count) {
                this.center_index = -1;
            }
		}
		public Vector3 origin {
			get {
				if (this.center_index == -1) return this.Barycenter();
				else return this[this.centers[this.center_index]].position;
			}
		}
		protected Vector3[] GetAcceleration() {
			Vector3[] acceleration = new Vector3[this.Count];
			// Initialise our array to Vector3.zero, since the default is a null pointer.
			Parallel.For (0, this.Count, i => {
				acceleration[i] = Vector3.zero;
			});
			for (int i = 0; i < this.Count; i++) {
				// We will need the index later so foreach is not possible
				Body body1 = this[i];
				for (int j = i + 1; j < this.Count; j++) {
					Body body2 = this[j]; // Again here
					// The magnitude of the force, multiplied by G, = %mu_1 * %mu_2 / r^2
					double mag_force_g = body1.stdGrav * body2.stdGrav / Math.Pow(Vector3.Magnitude(body1.position - body2.position),2);
					// We lost direction in the previous calculation (since we had to square the vector), but we need it.
					Vector3 direction = Vector3.Unit(body1.position - body2.position);
					// since acceleration is F/m, and we have G*F and G*m, we can find an acceleration vector easily
					Vector3 acceleration1 =  mag_force_g * -direction / body1.stdGrav;
					Vector3 acceleration2 = mag_force_g * direction / body2.stdGrav;
					acceleration[i] += acceleration1;
					acceleration[j] += acceleration2;
				}
			}
			return acceleration;
		}
		protected void TimeStep(double step) {
			var acceleration = this.GetAcceleration();
			for (int i = 0; i < acceleration.Length; i++) {
				Body body = this[i];
				Vector3 a = acceleration[i];
				body.position += step*body.velocity + Math.Pow(step,2)*a/2;
				body.velocity += step*a;
			}
		}
		public void StartAsync(double step = 1) {
			Task.Run(() => Start(step));
		}
		public void Start(double step = 1) {
			this.running = true;
			while (running) this.TimeStep(step);
		}
		public void Stop() {
			this.running = false;
		}
	}
}