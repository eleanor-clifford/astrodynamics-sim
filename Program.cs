using System;
using System.Collections.Generic;
using System.Threading;
using Structures;
using Mechanics;
using static Constants;

class Program {
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
				
			}
		}
	}
}