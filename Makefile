make:
	mcs -pkg:gtk-sharp-3.0 Program.cs Structures_Tests.cs Constants.cs Structures.cs Examples.cs Graphics.cs Input.cs PlanetarySystem.cs UI.cs /debug- /optimize+
	echo "cd $(realpath ./) && mono Program.exe 2>&1 | tee log.txt" > run.sh
	chmod +x run.sh
	sudo ln -s $(realpath ./)/run.sh /usr/bin/astrodynamics-sim
debug:
	mcs -pkg:gtk-sharp-3.0 Program.cs Structures_Tests.cs Constants.cs Structures.cs Examples.cs Graphics.cs Input.cs PlanetarySystem.cs UI.cs -debug
test:
	mcs Tests.cs Structures.cs Structures_Tests.cs Constants.cs Examples.cs PlanetarySystem.cs -debug
	mono Tests.exe
	rm Tests.exe*
clean:
	-sudo /usr/bin/astrodynamics-sim
	-rm Program.exe*
	-rm run.sh
	-rm log.txt
