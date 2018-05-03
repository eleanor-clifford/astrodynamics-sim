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
		public override String ToString() {
			return $"{x},{y},{z}";
		}
		public static double Magnitude(Vector3 v) {
			return Math.Sqrt(Math.Pow(v.x,2)+Math.Pow(v.y,2)+Math.Pow(v.z,2));
		} 
	}
	class Body {
		public Body(double stdGrav, double radius, Vector3 position, Vector3 velocity) {
			this.stdGrav = stdGrav;
			this.radius = radius;
			this.position = position;
			this.velocity = velocity;
		}
		public double stdGrav {get; protected set;}
		public double radius {get; protected set;}
		public Vector3 position {get; set;}
		public Vector3 velocity {get; set;}
	}
	class System {
		public List<Body> bodies {get; protected set;}
		public Vector3 bounds {get; protected set; }
	}
}