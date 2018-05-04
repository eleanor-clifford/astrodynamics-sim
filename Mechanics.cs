using System;
using System.Collections.Generic;
using System.Linq;
using System.Math;
using Structures;

class Mechanics {
	public Vector3[] GetAcceleration(System s) {
		Body body1, body2;
		Vector3[] acceleration;
		for (int i = 0; i < s.bodies.Length; i++) {
			acceleration[i] = Vector3.zero;
		}
		for (int i = 0; i < s.bodies.Length; i++) {
			body1 = s.bodies[i]; // We will need the index later so foreach is not possible
			for (int j = i+1; j < s.bodies.Length; j++) {
				body2 = s.bodies[j];
				float mag_force_g = body1.stdGrav * body2.stdGrav / Math.Pow(Vector3.Magnitude(body1.position - body2.position),2);
				float direction = (body1.position - body2.position);
				direction /= Vector3.Magnitude(direction);
				// TODO: Check gravity is attractive on the next two lines
				float acceleration1 = -direction * force_g / body1.stdGrav;
				float acceleration2 = direction * force_g / body2.stdGrav;
				acceleration[i] += acceleration1;
				acceleration[j] += acceleration2;
			}
		} return acceleration;
	}
	public void TimeStep(System s, Vector3[] acceleration, float step) {
		foreach ((Body,Vector3) (body,a) in (s.bodies,acceleration)) {
			body.velocity += a*step;
			body.position += a*Math.Pow(step,2)/2;
		}
	}
}