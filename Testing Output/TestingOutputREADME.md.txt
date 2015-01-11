ProjectVoyager
==============

The testing output folder. 

output-***####.txt files are output files from main.cpp, using major_bodies.txt as an input
The *** is the length of time they cover (either 1 or 10 years)
The #### is the type of numerical integration method used to make the output. (either euler, RK4 or RK10)

The realData-*yr.txt files contain actual data on the position of the 40 major bodies over * years, taken from the NASA database.

simTester.py compares an output file with the real data. 

planetTester.py is checks the output files against the actual average position of the planets. (It’s a quick and dirty check and it only works for the planets, not any planetary body)

planetMotion.py shows a 2D thing of the earth, moon and sun moving around, based on one of the output files. 
graphics.py is required for planetMotion.py to work. 