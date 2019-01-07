using System;
using System.Collections.Generic;
using static Program.Constants;
namespace Structures {
    public static class Tests {
        public static bool MatrixTest() {
            // Scalar Arithmetic
            var i = new Matrix3(new Vector3(1,0,0),new Vector3(0,1,0),new Vector3(0,0,1));
            var a = new Matrix3(new Vector3(1,3,1),new Vector3(0,4,1),new Vector3(2,-1,0));
            if (i*i != i) {
                Console.WriteLine("i*i != i");
                return false;
            }
            if (i*a != a || a*i != a) {                
                Console.WriteLine("a*i != a");
                return false;
            }
            if ((double)5 * a / (double)5 != a) {
                Console.WriteLine("5*a/5 != i");
                return false;
            }
            if (a + a != 2 * a) {
                Console.WriteLine("a + a != 2 * a");
                return false;
            }
            
            // Inverse (Also tests Determinant, Minor, Transpose_Cofactor)
            var a_inv = new Matrix3(new Vector3(-1,1,1),new Vector3(-2,2,1),new Vector3(8,-7,-4));
            if (Matrix3.Inverse(a) != a_inv) {
                Console.WriteLine($"Matrix3.Inverse(i) == \n{Matrix3.Inverse(a)} != \n{a_inv}");
                return false;
            }
            try {
                Matrix3.Inverse(new Matrix3(Vector3.zero,Vector3.zero,Vector3.zero));
                Console.WriteLine("No Exception on Inverse of Singular Matrix");
            } catch (DivideByZeroException) {}

            // Matrix-Matrix Multiplication (Also tests Transpose)
            var a_sq = new Matrix3(new Vector3(3,14,4), new Vector3(2,15,4), new Vector3(2,2,1));
            if (a * a != a_sq) {
                Console.WriteLine($"a * a != a_sq");
                return false;
            }
            return true;
        }
        public static bool VectorTest() {
            var a = new Vector3(2,3,6);
            var z = Vector3.zero;
            // Scalar Arithmetic
            if (a + z != a || z + a != a) {
                Console.WriteLine("a + z != a");
                return false;
            }
            if ((double)5 * a / (double)5 != a) {
                Console.WriteLine("5*a/5 != i");
                return false;
            }
            if (a + a != 2 * a) {
                Console.WriteLine("a + a != 2 * a");
                return false;
            }
            if (-a != z - a) {
                Console.WriteLine("-a != z - a");
                return false;
            }
            if (Vector3.Magnitude(a) != 7) {
                Console.WriteLine("Incorrect Magnitude");
                return false;
            }
            if (Vector3.dot(a,z) != 0 || Vector3.dot(a,a) != 49) {
                Console.WriteLine("incorrect dot");
                return false;
            }
            var a_u = new Vector3((double)2/7,(double)3/7,(double)6/7);
            if (Vector3.Unit(a) != a_u) {
                Console.WriteLine("incorrect unit");
                return false;
            }
            var exp = new Vector3(1000,0,-100);
            return true;
        }
        public static bool OrbitalElementsTest() {
            var elem = new OrbitalElements() {
                semimajoraxis = 1*AU,
                eccentricity = 0.027,
                inclination = 30*deg,
                ascendingNodeLongitude = 10*deg,
                periapsisArgument = 15*deg,
                trueAnomaly = 5*deg,
            };
            Console.WriteLine(elem.semimajoraxis/AU);
            var sun = Structures.Examples.sun;
            var b = new Body(sun,elem);
            var fVectors = new FundamentalVectors(b.position,b.velocity,sun.stdGrav);
            Console.WriteLine(fVectors);
            var elem2 = new OrbitalElements(b.position,b.velocity,sun.stdGrav);
            Console.WriteLine(elem2.semimajoraxis/AU);
            return true;
        }

        public static bool BodyTest() {
            var sun = new Body {
    			stdGrav = 4.47e20,
    			radius = 6.95e10, // 100x
    			position = Vector3.zero,
    			velocity = Vector3.zero,
    			color = Vector3.zero
    		};
            var earthElements = new OrbitalElements {
                semimajoraxis = 3.5*AU,
                eccentricity = 0.7,
                inclination = 37*deg,
                ascendingNodeLongitude = 128*deg,
                periapsisArgument = 250*deg,
                trueAnomaly = 7*deg
            };
            var earth1 = new Body(sun,earthElements);
            var earthElements2 = new OrbitalElements(earth1.position,earth1.velocity,sun.stdGrav);
            foreach (Tuple<string,double,double> t in new List<Tuple<string,double,double>>() {
                new Tuple<string,double,double>("l",earthElements.ascendingNodeLongitude, earthElements2.ascendingNodeLongitude),
                new Tuple<string,double,double>("e",earthElements.eccentricity,           earthElements2.eccentricity),
                new Tuple<string,double,double>("i",earthElements.inclination,            earthElements2.inclination),
                new Tuple<string,double,double>("w",earthElements.periapsisArgument,      earthElements2.periapsisArgument),
                new Tuple<string,double,double>("a",earthElements.semimajoraxis,          earthElements2.semimajoraxis),
                new Tuple<string,double,double>("v",earthElements.trueAnomaly,            earthElements2.trueAnomaly),
            }) {
                if ((t.Item2 - t.Item3)/t.Item2 > 1e-6) {
                    Console.WriteLine($"Orbital element test failed: {t.Item1}, {t.Item2}, {t.Item3}, {((t.Item2 - t.Item3)/t.Item2)*100}%");
                    return false;
                }
            }
            var expected_position = AU * new Vector3(0.7104623753,0.4739122976,-0.6417428577);
            var expected_velocity = new Vector3(-31555.21806,60479.93979,-9320.949522);
            if (Math.Abs(Vector3.Magnitude(earth1.position - expected_position))/Vector3.Magnitude(expected_position) > Math.Pow(10,-6)) {
                return false;
            }
            if (Math.Abs(Vector3.Magnitude(earth1.velocity - expected_velocity))/Vector3.Magnitude(expected_velocity) > Math.Pow(10,-6)) {
                return false;
            }
            sun.stdGrav = 1.3271440019e20;
            for (double i = 0; i < 2*Math.PI; i += 0.1) {
                for (double j = 0; j < 2*Math.PI; j += 0.1) {
                    for (double k = 0; k < 2*Math.PI; k += 0.1) {
                        for (double l = 0; l < 2*Math.PI; l += 0.1) {
                            var elements = new OrbitalElements() {
                                semimajoraxis = 1*AU,
                                inclination = Math.PI,
                                ascendingNodeLongitude = j,
                                periapsisArgument = k,
                                trueAnomaly = l
                            };
                            var earth = new Body(sun,elements){
            	        		stdGrav = 3.986004419e14,
            			        radius = 6.371e8, // 100x
    		        	        color = new Vector3(0,0.2,0.8),
    		                };
                            if (!(Math.Abs(Vector3.Magnitude(earth.velocity) - 3e4) < 1e3 )) {
                                Console.WriteLine($"{i},{j},{k},{l},{earth.velocity}");
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}