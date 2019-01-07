# Analysis

## Target audience

- Ordinary people who want to understand astrodynamics visually
- Astrophysicists who want to model events such as the effects of rogue planets on orbits

## Interface

- The system can be initially set up by adding bodies to a list (GUI) and using sliders to
set position, mass, radius, luminosity vector, color vector, and either a Keplerian 
orbit description around another body, or an initial velocity vector. All these variables 
can be modified at runtime, including adding another body spontaneously.

- The system will be rendered into 2D realistically - but since the radius of the body does 
not affect the orbits, they can be set to be arbitrarily large for ease of viewing. If the 
barycenter of the system is defined, orbital tracks will be drawn according to an approximation 
of the Keplerian orbit. Internally, however, Newton's laws are used directly.

- The player can place several cameras and define a reference frame for each before the simulation 
is started, and direction each camera is pointing can be changed while the program is running.

- An example system (our Solar System) is provided, and the state of any system can be 
serialized and written to disk.




