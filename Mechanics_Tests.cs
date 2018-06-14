using System;
using Structures;
//using static Mechanics.Mechanics;
using static Constants;

namespace Mechanics {
    class Tests {
        public static bool EarthSun() {
            var sys = new PlanetarySystem();
    		var sun = new Body {
    			stdGrav = 1.3271440019e20,
    			radius = 6.95e10, // 100x
    			position = Vector3.zero,
    			velocity = Vector3.zero,
     			luminositySpectrum = new Vector3(1,1,1),
    			reflectivity = Vector3.zero
    		};
    		var earth = new Body {
    			stdGrav = 3.986004419e14,
    			radius = 6.371e8, // 100x
    			position = new Vector3(0,1*AU,0),
    			velocity = new Vector3(3e4,0,0),
    			luminositySpectrum = Vector3.zero,
    			reflectivity = new Vector3(0,0.2,0.8),
    		};
    		sys.Add(sun);
    		sys.Add(earth);
    		//Console.WriteLine(sys.bodies);
    		var initialPosition = sys.bodies[1].position;
    		Vector3 lastPosition = sys.bodies[1].position;
    		double t = 1;
    		bool half = false;
    		for (int i = 0; true; i++) {
				/*
                if (i%100000 == 0) {
    				Console.WriteLine($"Sun Position: {sys.bodies[0].position}\nEarth Position: {sys.bodies[1].position}\n");
    				Console.WriteLine($"Sun Velocity: {sys.bodies[0].velocity}\nEarth Velocity: {sys.bodies[1].velocity}\n");
    				Console.WriteLine($"Position Difference: {initialPosition - sys.bodies[1].position}\n");
    				var acuteAngle = Math.Acos(Vector3.UnitDot(initialPosition,sys.bodies[1].position));
    				Console.WriteLine($"Angle: {(half ? 2*Math.PI - acuteAngle : acuteAngle)}\n");
    				Console.WriteLine($"Orbital Radius {Vector3.Magnitude(sys.bodies[1].position)}\n");
    				//Thread.Sleep(1);
    			}
                */
    			if (!half && Vector3.Magnitude(lastPosition - initialPosition) > Vector3.Magnitude(sys.bodies[1].position - initialPosition))
    			{
    				half = true;
    				//Console.WriteLine("halfway");
    			}
    			if (half && Vector3.Magnitude(lastPosition - initialPosition) < Vector3.Magnitude(sys.bodies[1].position - initialPosition))
    			{
    				half = false;
                    /*
    				Console.WriteLine("Full Turn");
    				Console.WriteLine($"From {lastPosition} to {sys.bodies[1].position}");
    				Console.WriteLine($"Initial Position: {initialPosition}");
    				Console.WriteLine($"Relative radius difference: {(Vector3.Magnitude(sys.bodies[1].position) - 1*AU)/(1*AU)}");
    				*/
                    double radiusDifference = (Vector3.Magnitude(sys.bodies[1].position) - 1*AU)/(1*AU);
                    if (radiusDifference < 1e-5) return true;
                    else return false;
    			}
    			lastPosition = sys.bodies[1].position;
	    		sys.TimeStep(t);
            }
	    }
    }
}