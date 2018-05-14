using System;
using System.Collections.Generic;
using System.Linq;
using Structures;

class Mechanics {
	public Vector3[] GetAcceleration(PlanetarySystem s) {
		Body body1, body2;
		Vector3[] acceleration = new Vector3[s.bodies.Count];
		// Initialise our array to zero, since the default is a null pointer.
		for (int i = 0; i < s.bodies.Count; i++) {
			acceleration[i] = Vector3.zero;
		}
		for (int i = 0; i < s.bodies.Count; i++) {
			body1 = s.bodies[i]; // We will need the index later so foreach is not possible
			for (int j = i+1; j < s.bodies.Count; j++) {
				body2 = s.bodies[j]; // Again here
				// The magnitude of the force, multiplied by G, = %mu_1 * %mu_2 / r^2
				double mag_force_g = body1.stdGrav * body2.stdGrav / Math.Pow(Vector3.Magnitude(body1.position - body2.position),2);
				// We lost direction in the previous calculation (since we had to square the vector), but we need it.
				Vector3 direction = (body1.position - body2.position);
				direction /= Vector3.Magnitude(direction);
				// TODO: Check gravity is attractive on the next two lines
				// since acceleration is F/m, and we have G*F and G*m, we can find an acceleration vector easily
				Vector3 acceleration1 =  mag_force_g * -direction / body1.stdGrav;
				Vector3 acceleration2 = mag_force_g * direction / body2.stdGrav;
				acceleration[i] += acceleration1;
				acceleration[j] += acceleration2;
			}
		} return acceleration;
	}
	public void TimeStep(PlanetarySystem s, Vector3[] acceleration, float step) {
		for (int i = 0; i < acceleration.Length; i++) {
			Body body = s.bodies[i];
			Vector3 a = acceleration[i];
			body.velocity += step*a;
			body.position += body.velocity + Math.Pow(step,2)*a/2;
		}
	}
}