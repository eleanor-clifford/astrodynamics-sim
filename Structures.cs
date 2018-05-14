using System;
using System.Collections.Generic;
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
		public static Vector3 zero = new Vector3(0,0,0);
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
		public override String ToString() {
			return $"Matrix3( {x.x} {x.y} {x.z}\n         {y.x} {y.y} {y.z}\n         {z.x} {z.y} {z.z} )";
		}
		public static bool operator== (Matrix3 a, Matrix3 b) {
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}
		public static bool operator!= (Matrix3 a, Matrix3 b) {
			return !(a == b);
		}
		public static Matrix3 Rotation(double x, double y) {
			return new Matrix3 (
				new Vector3(
					Math.Cos(y),
					0,
					Math.Sin(y)
				),
				new Vector3(
					Math.Sin(x)*Math.Sin(y),
					Math.Cos(y),
					-(Math.Sin(x)*Math.Cos(y))
				),
				new Vector3(
					-(Math.Cos(x)*Math.Sin(y)),
					Math.Sin(x),
					Math.Cos(x)*Math.Cos(y)
				)
			);
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
	class Body {
		public Body(double stdGrav, double radius, Vector3 position, Vector3 velocity, Vector3 luminositySpectrum, Vector3 reflectivity) {
			this.stdGrav = stdGrav;
			this.radius = radius;
			this.position = position;
			this.velocity = velocity;
			this.luminositySpectrum = luminositySpectrum;
			this.reflectivity = reflectivity;
		}
		public double stdGrav {get; protected set;}
		public double radius {get; protected set;}
		public Vector3 position {get; set;}
		public Vector3 velocity {get; set;}
		public Vector3 luminositySpectrum {get; protected set;}
		public Vector3 reflectivity {get; protected set;}
		//public static Body FromKepler(float ...);
	}
	class PlanetarySystem {
		public List<Body> bodies {get; protected set;}
		public Vector3 bounds {get; protected set; }
		public void Add(Body body) {
			bodies.Add(body);
		}
	}
}