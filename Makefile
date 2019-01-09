make:
	mcs -pkg:gtk-sharp-3.0 Program.cs Structures_Tests.cs Constants.cs Structures.cs Examples.cs Graphics.cs Input.cs PlanetarySystem.cs UI.cs /debug- /optimize+
debug:
	mcs -pkg:gtk-sharp-3.0 Program.cs Structures_Tests.cs Constants.cs Structures.cs Examples.cs Graphics.cs Input.cs PlanetarySystem.cs UI.cs -debug
tests:
	mcs Tests.cs Structures.cs Structures_Tests.cs Constants.cs Examples.cs PlanetarySystem.cs -debug
clean:
	rm Program.exe*
	rm Tests.exe*
