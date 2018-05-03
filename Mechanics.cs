using System;
using System.Collections.Generic;
using Structures;

class Mechanics {
	static public (Body, Body) TwoBodyStep(Body body1, Body body2, float time) {
		double r2 = Math.Pow(Vector3.Magnitude(body1.position-body2.position),2);
		//Console.WriteLine($"Distance: {r2}");
		double dV1 = time * body2.stdGrav / r2;
		double dV2 = time * body1.stdGrav / r2;
		Vector3 direction1 = body2.position - body1.position;
		direction1 /= Vector3.Magnitude(direction1);
		Vector3 direction2 = -direction1;
		body1.velocity += dV1 * direction1;
		body2.velocity += dV2 * direction2;
		Console.WriteLine($"deltaV: {Vector3.Magnitude(dV2 * direction2)}");
		body1.position += time * body1.velocity;
		body2.position += time * body2.velocity;
		return (body1,body2);
	}
}