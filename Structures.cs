using System;
using System.Collections.Generic;
using static Constants;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace Structures
{
	public class Vector3 {
		public double x {get; set;}
		public double y {get; set;}
		public double z {get; set;}
		public Vector3(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public static bool operator== (Vector3 a, Vector3 b) {
			try {
				double x = a.x;
			} catch (NullReferenceException) {
				try {
					double y = b.x;
					return false;
				} catch (NullReferenceException) {return true;}
			} try {
				double x = b.x;
			} catch (NullReferenceException) {return false;}
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
		public static Vector3 cross(Vector3 a, Vector3 b) {
			return new Vector3(
				a.y*b.z - a.z*b.y,
				a.z*b.x - a.x*b.z,
				a.x*b.y - a.y*b.x
			);
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
		public static Vector3 Log(Vector3 v, double b = Math.E) {
			var polar = CartesianToPolar(v);
			var log_polar = new Vector3 (Math.Log(polar.x,b),polar.y,polar.z);
			var log = PolarToCartesian(log_polar);
			return log;
		}
		public static Vector3 LogByComponent(Vector3 v, double b = Math.E) {
			var r = new Vector3(0,0,0); // using Vector3.zero will modify it
			if (v.x < 0) r.x = -Math.Log(-v.x,b);
			else if (v.x != 0) r.x = Math.Log(v.x,b);
			if (v.y < 0) r.y = -Math.Log(-v.y,b);
			else if (v.y != 0) r.y = Math.Log(v.y,b);
			if (v.z < 0) r.z = -Math.Log(-v.z,b);
			else if (v.z != 0) r.z = Math.Log(v.z,b);
			//Console.WriteLine($"{v} -> {r}");
			return r;
		}
		public static Vector3 CartesianToPolar(Vector3 v) {
			// ISO Convention
			var r = Vector3.Magnitude(v);
			var theta = Math.Acos(Vector3.UnitDot(v,Vector3.k));
			var phi = Math.Acos(Vector3.UnitDot(new Vector3(v.x,v.y,0),Vector3.i));
			if (v.y < 0) phi = -phi;
			return new Vector3(r,theta,phi);
		}
		public static Vector3 PolarToCartesian(Vector3 v) {
			return Matrix3.ZRotation(v.z) * new Matrix3(0,v.y) * (v.x*Vector3.k);
		}
		public static Vector3 zero {get;} = new Vector3(0,0,0);
		public static Vector3 i {get;} = new Vector3(1,0,0);
		public static Vector3 j {get;} = new Vector3(0,1,0);
		public static Vector3 k {get;} = new Vector3(0,0,1);
	}
	public class Matrix3 {
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
		public static Matrix3 XRotation(double x) {
			return new Matrix3 (
				new Vector3(1,0,0),
				new Vector3(0,Math.Cos(x),Math.Sin(x)),
				new Vector3(0,-Math.Sin(x),Math.Cos(x))
			);
		}
		public static Matrix3 YRotation(double y) {
			return new Matrix3 (
				new Vector3(Math.Cos(y),0,Math.Sin(y)),
				new Vector3(0,1,0),
				new Vector3(-Math.Sin(y),0,Math.Cos(y))
			);
		}
		public static Matrix3 ZRotation(double z) {
			return new Matrix3 (
				new Vector3(Math.Cos(z),-Math.Sin(z),0),
				new Vector3(Math.Sin(z),Math.Cos(z),0),
				new Vector3(0,0,1)
			);
		}
		public static Matrix3 ExtrinsicZYXRotation(double x, double y, double z) {
			return XRotation(x)*YRotation(y)*ZRotation(z);
		}
		public static Matrix3 ExtrinsicZYXRotation(Vector3 v) {
			return XRotation(v.x)*YRotation(v.y)*ZRotation(v.z);
		}
		public static Matrix3 IntrinsicZYXRotation(double x, double y, double z) {
			return ZRotation(z)*YRotation(y)*XRotation(x);
		}
		public static Matrix3 IntrinsicZYXRotation(Vector3 v) {
			return ZRotation(v.z)*YRotation(v.y)*XRotation(v.x);
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
	public class Plane {
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
	public class Body : ICloneable {
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
		public object Clone() {
			return new Body {
				name = this.name,
				parent = this.parent,
				stdGrav = this.stdGrav,
				radius = this.radius,
				position = this.position,
				velocity = this.velocity,
				angleReference = this.angleReference,
				luminositySpectrum = this.luminositySpectrum,
				reflectivity = this.reflectivity
			};
		}
	}
	public class FundamentalVectors {
		public Vector3 angularMomentum {get; set;}
		public Vector3 eccentricity {get; set;}
		public Vector3 node {get; set;}
		public FundamentalVectors(Vector3 position, Vector3 velocity, double stdGrav) {
			this.angularMomentum = Vector3.cross(position,velocity);
			this.node = Vector3.cross(Vector3.k,this.angularMomentum);
			var mag_r = Vector3.Magnitude(position);
			var mag_v = Vector3.Magnitude(velocity);
			this.eccentricity = (1/stdGrav)*((Math.Pow(mag_v,2) - stdGrav/mag_r)*position - Vector3.dot(position,velocity)*velocity);
		}

	}
	public class OrbitalElements {
		public double semimajoraxis {get; set;}
		public double eccentricity {get; set;}
		public double inclination {get; set;}
		public double ascendingNodeLongitude {get; set;}
		public double periapsisArgument {get; set;}
		public double trueAnomaly {get; set;}
		
		public OrbitalElements(Vector3 position, Vector3 velocity, double stdGrav) {
			var fVectors = new FundamentalVectors(position,velocity,stdGrav);
			this.eccentricity = Vector3.Magnitude(fVectors.eccentricity);
			var parameter = Math.Pow(Vector3.Magnitude(fVectors.angularMomentum),2)/stdGrav;
			this.inclination = Math.Acos(fVectors.angularMomentum.z/Vector3.Magnitude(fVectors.angularMomentum)); // 0 <= i <= 180deg
			
		}
	}
}