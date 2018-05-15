using System;
using System.Collections.Generic;
namespace Structures {
    public class Tests {
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
            return true;
        }
    }
}