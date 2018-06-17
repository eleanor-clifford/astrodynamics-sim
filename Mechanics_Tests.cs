using System;
using Structures;
using System.Collections.Generic;
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
    		bool half = false;
			int i = 0;
    		foreach (List<Body> bodies in sys.Start()) {
				i++;
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
                    if (radiusDifference < 1e-5) {
						Console.WriteLine(i);
						return true;
					}
                    else return false;
    			}
    			lastPosition = sys.bodies[1].position;
				if (i >= 1e8) {
					// We shouldn't ever get here, but if everything goes wrong it will eventually fall here and return
					return false;
				}
            } return false;
	    }
    }
}