using System;
using System.Collections.Generic;
using Structures;
using static Mechanics;
class Program {
	static void Main(string[] args) {
		Body earth = new Body(3.986004418e14,6371e3,new Vector3(0,0,0),new Vector3(0,0,0));
		Body ISS = new Body(0,8000,new Vector3(0,6779e3,0),new Vector3(8,0,0));
		for (int i = 0; i < 1000000; i++) {
			(earth,ISS) = Mechanics.TwoBodyStep(earth,ISS,0.01f);
			//if (i%100000 == 0) {
				System.Console.WriteLine($"ISS Position: {ISS.position.ToString()}, radius {Vector3.Magnitude(earth.position-ISS.position)}");
				System.Console.WriteLine($"ISS Velocity: {ISS.velocity.ToString()}");					
			//}	
		}
	}
}