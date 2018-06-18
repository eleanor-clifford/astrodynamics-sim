using System;
using System.Collections.Generic;
using static Constants;
using System.Threading;
using System.Threading.Tasks;
namespace Structures
{
	class Vector3 {
		public double x {get; set;}
		public double y {get; set;}
		public double z {get; set;}
		public Vector3(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public static bool operator== (Vector3 a, Vector3 b) {
			return a.x == b.x && b.y == b.y && a.z == b.z;
		}
		public static bool operator!= (Vector3 a, Vector3 b) {
			return !(a == b);
		}
		public static Vector3 operator- (Vector3 a, Vector3 b) {
			return new Vector3 (a.x-b.x,a.y-b.y,a.z-b.z);
		}
		public static Vector3 operator- (Vector3 a) {
			return new Vector3(-a.x,-a.y,-a.z);
		}
		public static Vector3 operator+ (Vector3 a, Vector3 b) {
			return new Vector3 (a.x+b.x,a.y+b.y,a.z+b.z);
		}
		public static Vector3 operator* (double a, Vector3 b) {
			return new Vector3 (a*b.x,a*b.y,a*b.z);
		}
		public static Vector3 operator/ (Vector3 a, double b) {
			return new Vector3 (a.x/b,a.y/b,a.z/b);
		}
		public static double dot(Vector3 a, Vector3 b) {
			// This could be overloaded to operator*, but an explicit function increases readibility.
			return a.x*b.x + a.y*b.y + a.z*b.z; 
		}
		public override String ToString() {
			return $"Vector3({x},{y},{z})";
		}
		public static double Magnitude(Vector3 v) {
			return Math.Sqrt(Math.Pow(v.x,2)+Math.Pow(v.y,2)+Math.Pow(v.z,2));
		}
		public static Vector3 Unit(Vector3 v) {
			return v / Vector3.Magnitude(v);
		}
		public static double UnitDot(Vector3 a, Vector3 b) {
			return Vector3.dot(Vector3.Unit(a),Vector3.Unit(b));
		}
		public static Vector3 zero = new Vector3(0,0,0);
		public static Vector3 i = new Vector3(1,0,0);
		public static Vector3 j = new Vector3(0,1,0);
		public static Vector3 k = new Vector3(0,0,1);
	}
	class Matrix3 {
		// the fields describe the rows
		public Vector3 x {get;}
		public Vector3 y {get;}
		public Vector3 z {get;}
		public Matrix3(Vector3 x, Vector3 y, Vector3 z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Matrix3(double x, double y) {
			this.x = new Vector3(
				Math.Cos(y),
				0,
				Math.Sin(y)
			);
			this.y = new Vector3(
				Math.Sin(x)*Math.Sin(y),
				Math.Cos(x),
				-(Math.Sin(x)*Math.Cos(y))
			);
			this.z = new Vector3(
				-(Math.Cos(x)*Math.Sin(y)),
				Math.Sin(x),
				Math.Cos(x)*Math.Cos(y)
			);
		}
		public static Matrix3 ZRotation(double z) {
			return new Matrix3 (
				new Vector3(Math.Cos(z),-Math.Sin(z),0),
				new Vector3(Math.Sin(z),Math.Cos(z),0),
				new Vector3(0,0,1)
			);
		}
		public override String ToString() {
			return $"Matrix3( {x.x} {x.y} {x.z}\n         {y.x} {y.y} {y.z}\n         {z.x} {z.y} {z.z} )";
		}
		public static bool operator== (Matrix3 a, Matrix3 b) {
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}
		public static bool operator!= (Matrix3 a, Matrix3 b) {
			return !(a == b);
		}

		public static Matrix3 operator+ (Matrix3 a, Matrix3 b) {
			return new Matrix3(
				a.x + b.x,
				a.y + b.y,
				a.z + b.z
			);
		}
		public static Vector3 operator* (Matrix3 m, Vector3 v) {
			return new Vector3(
				Vector3.dot(m.x,v),
				Vector3.dot(m.y,v),
				Vector3.dot(m.z,v)
			);
		}
		public static Matrix3 operator* (double d, Matrix3 m) {
			return new Matrix3(
				d * m.x,
				d * m.y,
				d * m.z
			);
		}
		public static Matrix3 operator/ (Matrix3 m, double d) {
			if (d == 0) throw new DivideByZeroException("Matrix Division By Zero");
			else return (1/d) * m;
		}
		public static Matrix3 operator* (Matrix3 l, Matrix3 r) {
			var r_t = Matrix3.Transpose(r);
			return new Matrix3 (
				new Vector3(
					Vector3.dot(l.x,r_t.x),
					Vector3.dot(l.x,r_t.y),
					Vector3.dot(l.x,r_t.z)
				),
				new Vector3(
					Vector3.dot(l.y,r_t.x),
					Vector3.dot(l.y,r_t.y),
					Vector3.dot(l.y,r_t.z)
				),
				new Vector3(
					Vector3.dot(l.z,r_t.x),
					Vector3.dot(l.z,r_t.y),
					Vector3.dot(l.z,r_t.z)
				)
			);
		}
		public static double Determinant(Matrix3 m) {
			return m.x.x * (m.y.y*m.z.z - m.y.z*m.z.y)
			      -m.x.y * (m.y.x*m.z.z - m.y.z*m.z.x)
				  +m.x.z * (m.y.x*m.z.y - m.y.y*m.z.x);
		}
		public static Matrix3 Transpose(Matrix3 m) {
			return new Matrix3(
				new Vector3(m.x.x,m.y.x,m.z.x),
				new Vector3(m.x.y,m.y.y,m.z.y),
				new Vector3(m.x.z,m.y.z,m.z.z)
			);
		}
		public static Matrix3 Transpose_Cofactor(Matrix3 m) {
			// We never need to do the cofactor without the transpose, so this is an optimisation
			return new Matrix3(
				new Vector3(m.x.x,-m.y.x,m.z.x),
				new Vector3(-m.x.y,m.y.y,-m.z.y),
				new Vector3(m.x.z,-m.y.z,m.z.z)
			);
		}
		public static Matrix3 Minor(Matrix3 m) {
			return new Matrix3(
				new Vector3(
					(m.y.y*m.z.z - m.y.z*m.z.y),
					(m.y.x*m.z.z - m.y.z*m.z.x),
					(m.y.x*m.z.y - m.y.y*m.z.x)
				),
				new Vector3(
					(m.x.y*m.z.z - m.x.z*m.z.y),
					(m.x.x*m.z.z - m.x.z*m.z.x),
					(m.x.x*m.z.y - m.x.y*m.z.x)
				),
				new Vector3(
					(m.x.y*m.y.z - m.x.z*m.y.y),
					(m.x.x*m.y.z - m.x.z*m.y.x),
					(m.x.x*m.y.y - m.x.y*m.y.x)
				)
			);
		}
		public static Matrix3 Inverse(Matrix3 m) {
			if (Matrix3.Determinant(m) == 0) throw new DivideByZeroException("Singular Matrix");
			Matrix3 C_T = Matrix3.Transpose_Cofactor(Matrix3.Minor(m));
			return (1/Matrix3.Determinant(m)) * C_T;
		}
	}
	class Plane {
		// P = M(mu*i+lambda*j) + ck
		public Matrix3 M {get; set;}
		public double c {get; set;}
		public Vector3 Normal() {
			return M*Vector3.k;
		}
		public bool OnPlane(Vector3 p) {
			Vector3 norm = p - M*Vector3.k;
			Vector3 rot = Matrix3.Inverse(M) * norm;
			if (rot.z == 0) return true;
			else return false;
		}
	}
	class Body {
		public string name {get; set;}
		public Body parent {get; set;}
		public double stdGrav {get; set;}
		public double radius {get; set;}
		public Vector3 position {get; set;} = Vector3.zero;
		public Vector3 velocity {get; set;} = Vector3.zero;
		public Vector3 angleReference { get; protected set;} = Vector3.i;
		public Vector3 luminositySpectrum {get; set;} = Vector3.zero;
		public Vector3 reflectivity {get; set;} = Vector3.zero;
		public Body (
			Body parent = null, 
			double semimajoraxis = 0, 
			double eccentricity = 0, 
			double inclination = 0, 
			double ascendingNodeLongitude = 0,
			double periapsisArgument = 0,
			double trueAnomaly = 0
		) {
			// First check the values are reasonable. If parent == null it is assumed that
			// position and velocity are set explicitly
			if (parent == null) return;
			this.parent = parent;
			if (inclination < 0 
			 || eccentricity < 0 
			 || semimajoraxis < 0 
			 || ascendingNodeLongitude < 0
			 || ascendingNodeLongitude >= 2*Math.PI
			 || periapsisArgument < 0
			 || periapsisArgument >= 2*Math.PI
			 || trueAnomaly < 0
			 || trueAnomaly >= 2*Math.PI
			){
				throw new ArgumentException();
			}
			// derive some useful values
			double periapsis = semimajoraxis*(1-eccentricity);
			double apoapsis = semimajoraxis*(1+eccentricity);
			// a^2 = b^2 + c^2 , a,b,c >= 0
			// a is the semimajor axis, b the semiminor axis, and c the distance from the center to the foci
			double semiminoraxis = Math.Sqrt(Math.Pow(semimajoraxis,2) - Math.Pow(semimajoraxis-periapsis,2));
			
			// Reference direction is the x axis and reference plane is xy
			Vector3 reference = new Vector3(1,0,0);

			// This is the transformation from the reference plane to the orbital plane.
			// First incline the orbit, then move the ascending node.
			Matrix3 transformation = Matrix3.ZRotation(ascendingNodeLongitude) 
			                       * new Matrix3(inclination,0);
			// The equation of the ellipse in the reference plane is
			// r = (a*cos(anomaly) + r_p - a, b*sin(anomaly),0)
			// with the parent body at the origin
			Vector3 referencePosition = new Vector3(semimajoraxis*Math.Cos(trueAnomaly) + periapsis - semimajoraxis,
			                                        semiminoraxis*Math.Sin(trueAnomaly),0);
			// To get the true position we rotate the ellipse in the reference plane according to
			// The argument of periapsis, then apply the transformation to the orbital plane
			Vector3 truePosition = parent.position + transformation * Matrix3.ZRotation(periapsisArgument) * referencePosition;
			
			// |v| = sqrt(mu(2/r - 1/a))
			double orbitalSpeed = Math.Sqrt(parent.stdGrav*(2/Vector3.Magnitude(referencePosition) - 1/semimajoraxis));
			// dy/dx = -b/a cot v
			Vector3 referenceVelocity;
			if (trueAnomaly == 0) {
				referenceVelocity = new Vector3(0, orbitalSpeed, 0);
			} else if (trueAnomaly == Math.PI) {
				referenceVelocity = new Vector3(0, -orbitalSpeed, 0);
			} else if (trueAnomaly > 0 && trueAnomaly < Math.PI) {
				referenceVelocity = orbitalSpeed * Vector3.Unit(new Vector3(-1, (semiminoraxis/semimajoraxis) * (1/Math.Tan(trueAnomaly)),0));
			} else {
				referenceVelocity = orbitalSpeed * Vector3.Unit(new Vector3(1, -(semiminoraxis/semimajoraxis) * (1/Math.Tan(trueAnomaly)),0));
			}
			// Rotate by the argument of periapsis then apply the transformation to the orbital plane
			Vector3 trueVelocity = transformation * Matrix3.ZRotation(periapsisArgument) * referenceVelocity;
			this.position = truePosition;
			this.velocity = trueVelocity;
			this.angleReference = transformation * Matrix3.ZRotation(periapsisArgument) * Vector3.i;
		}
	}
	class PlanetarySystem {
		protected bool running = false;
		public List<Body> bodies {get; protected set;}
		public Vector3 bounds {get; protected set; }
		public PlanetarySystem(List<Body> bodies = null) {
			if (bodies == null) this.bodies = new List<Body>();
			else this.bodies = bodies;
		}
		public void Add(Body body) {
			bodies.Add(body);
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