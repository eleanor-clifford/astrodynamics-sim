using System;
using System.Linq;
using System.Collections.Generic;
using Structures;
using static Program.Constants;
namespace Structures {
	static class Examples {
		// Example bodies, elements, and systems
		public static Body sun {get; private set;}
		public static PlanetarySystem solar_system {get; private set;}
		public static List<OrbitalElements> solar_system_elements {get; private set;}
		public static List<Body> solar_system_bodies {get; private set;}
		static Examples() {
			sun = new Body() {
				name = "Sol",
				stdGrav = 1.32712440018e20,
				radius = 696342e3/5, // 5x smaller than normal for viewabilty
				position = Vector3.zero,
				velocity = Vector3.zero,
				color = new Vector3(1,1,0)
			};
			solar_system_elements = new List<OrbitalElements>() {
				// Taken from data at the J2000 Epoch
				// http://www.met.rdg.ac.uk/~ross/Astronomy/Planets.html
				new OrbitalElements() {
					// Mercury
					semilatusrectum = 0.37073084632655*AU,
					eccentricity = 0.20563069,
					inclination = 7.00487*deg,
					ascendingNodeLongitude = 48.33167*deg,
					periapsisArgument = 77.45645*deg,
					trueAnomaly = 252.25084*deg
				},
				new OrbitalElements() {
					// Venus 
					semilatusrectum = 0.723298805955343*AU,
					eccentricity = 0.00677323,
					inclination = 3.39471*deg,
					ascendingNodeLongitude = 76.68069*deg,
					periapsisArgument = 131.53298*deg,
					trueAnomaly = 181.97973*deg
				},
				new OrbitalElements() {
					// Earth 
					semilatusrectum = 0.999720878516836*AU,
					eccentricity = 0.01671022,
					inclination = 0.00005*deg,
					ascendingNodeLongitude = 348.73936*deg,
					periapsisArgument = 102.94719*deg,
					trueAnomaly = 100.46435*deg
				},
				new OrbitalElements() {
					// Mars 
					semilatusrectum = 1.51036704082126*AU,
					eccentricity = 0.09341233,
					inclination = 1.85061*deg,
					ascendingNodeLongitude = 49.57854*deg,
					periapsisArgument = 336.04084*deg,
					trueAnomaly = 355.45332*deg
				},
				new OrbitalElements() {
					// Jupiter 
					semilatusrectum = 5.191177516718821*AU,
					eccentricity = 0.04839266,
					inclination = 1.30530*deg,
					ascendingNodeLongitude = 100.55615*deg,
					periapsisArgument = 14.75385*deg,
					trueAnomaly = 34.40438*deg
				},
				new OrbitalElements() {
					// Saturn 
					semilatusrectum = 9.50910488810135*AU,
					eccentricity = 0.05415060,
					inclination = 2.48446*deg,
					ascendingNodeLongitude = 113.71504*deg,
					periapsisArgument = 92.43194*deg,
					trueAnomaly = 49.94432*deg
				},
				new OrbitalElements() {
					// Uranus 
					semilatusrectum = 19.1485673429066*AU,
					eccentricity = 0.04716771,
					inclination = 0.76986*deg,
					ascendingNodeLongitude = 74.22988*deg,
					periapsisArgument = 170.96424*deg,
					trueAnomaly = 313.23218*deg
				},
				new OrbitalElements() {
					// Neptune 
					semilatusrectum = 30.0667468812982*AU,
					eccentricity = 0.00858587,
					inclination = 1.76917*deg,
					ascendingNodeLongitude = 131.72169*deg,
					periapsisArgument = 44.97135*deg,
					trueAnomaly = 304.88003*deg
				},
				new OrbitalElements() {
					// Pluto 
					semilatusrectum = 39.48168677*AU,
					eccentricity = 0.24880766,
					inclination = 17.14175*deg,
					ascendingNodeLongitude = 110.30347*deg,
					periapsisArgument = 224.06676*deg,
					trueAnomaly = 238.92881*deg
				}
			};
			solar_system_bodies = new List<Body>() {
				(Body)sun.Clone(),
				new Body() {
					name = "Mercury",
					stdGrav = 2.2033e13,
					radius = 2439.7e3,
					color = new Vector3(0.5604629613577541,0.5506810776290613,0.5615709550944886)
				},
				new Body() {
					name = "Venus",
					stdGrav = 3.24860e14,
					radius = 6051.8e3,
					color = new Vector3(0.7290057613658241,0.7163768245238121,0.6791579213171579)
				},
				new Body() {
					name = "Earth",
					stdGrav = 3.986004419e14,
					radius = 6371.0e3,
					color = new Vector3(0.36141510867913057,0.3805593555251558,0.4684865790976585)
				},
				new Body() {
					name = "Mars",
					stdGrav = 4.282837e13,
					radius = 3389.5e3,
					color = new Vector3(0.5128845217545257,0.3367414685964679,0.2022838932412694)
				},
				new Body() {
					name = "Jupiter",
					stdGrav = 1.26686535e17,
					radius = 69911e3,
					color = new Vector3(0.7189596667682617,0.6638891549711422,0.6361916372766723)
				},
				new Body() {
					name = "Saturn",
					stdGrav = 3.7931188e16,
					radius = 58232e3,
					color = new Vector3(0.8246372253577235,0.7470193676770795,0.59518943574319)
				},
				new Body() {
					name = "Uranus",
					stdGrav = 5.793940e15,
					radius = 25362e3,
					color = new Vector3(0.565224110171928,0.7359458915531022,0.8092590995342418)
				},
				new Body() {
					name = "Neptune",
					stdGrav = 6.836530e15,
					radius = 24622e3,
					color = new Vector3(0.5525244704623422,0.7383866805149026,0.868736820570925)
				},
				new Body() {
					name = "Pluto",
					stdGrav = 8.72e11,
					radius = 1186e3,
					color = new Vector3(0.732870760490961,0.6071190239708979,0.4988704626052213)
				}
			};
			solar_system = new PlanetarySystem(new List<Body>() {
				// All radii are multiplied by 100
				// Colors from https://planetarium.madison.k12.wi.us/planets-true.htm
				// Radii from https://en.wikipedia.org/wiki/List_of_Solar_System_objects_by_size
				(Body)sun.Clone(),
				new Body(sun, solar_system_elements[0]) {
					name = "Mercury",
					stdGrav = 2.2033e13,
					radius = 2439.7e3,
					color = new Vector3(0.5604629613577541,0.5506810776290613,0.5615709550944886)
				},
				new Body(sun, solar_system_elements[1]) {
					name = "Venus",
					stdGrav = 3.24860e14,
					radius = 6051.8e3,
					color = new Vector3(0.7290057613658241,0.7163768245238121,0.6791579213171579)
				},
				new Body(sun, solar_system_elements[2]) {
					name = "Earth",
					stdGrav = 3.986004419e14,
					radius = 6371.0e3,
					color = new Vector3(0.36141510867913057,0.3805593555251558,0.4684865790976585)
				},
				new Body(sun, solar_system_elements[3]) {
					name = "Mars",
					stdGrav = 4.282837e13,
					radius = 3389.5e3,
					color = new Vector3(0.5128845217545257,0.3367414685964679,0.2022838932412694)
				},
				new Body(sun, solar_system_elements[4]) {
					name = "Jupiter",
					stdGrav = 1.26686535e17,
					radius = 69911e3,
					color = new Vector3(0.7189596667682617,0.6638891549711422,0.6361916372766723)
				},
				new Body(sun, solar_system_elements[5]) {
					name = "Saturn",
					stdGrav = 3.7931188e16,
					radius = 58232e3,
					color = new Vector3(0.8246372253577235,0.7470193676770795,0.59518943574319)
				},
				new Body(sun, solar_system_elements[6]) {
					name = "Uranus",
					stdGrav = 5.793940e15,
					radius = 25362e3,
					color = new Vector3(0.565224110171928,0.7359458915531022,0.8092590995342418)
				},
				new Body(sun, solar_system_elements[7]) {
					name = "Neptune",
					stdGrav = 6.836530e15,
					radius = 24622e3,
					color = new Vector3(0.5525244704623422,0.7383866805149026,0.868736820570925)
				},
				new Body(sun, solar_system_elements[8]) {
					name = "Pluto",
					stdGrav = 8.72e11,
					radius = 1186e3,
					color = new Vector3(0.732870760490961,0.6071190239708979,0.4988704626052213)
				}
			});
		}
	}
}