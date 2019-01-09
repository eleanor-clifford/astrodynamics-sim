using System;
using Structures;
using static Structures.Tests;

class Tests {
    static void Main() {
        if (VectorTest()) Console.WriteLine("Vector test complete");
        else Console.WriteLine("Vector test failed");
        if (MatrixTest()) Console.WriteLine("Matrix test complete");
        else Console.WriteLine("Matrix test failed");
        if (BodyTest()) Console.WriteLine("Body test complete");
        else Console.WriteLine("Body test failed");
    }
}