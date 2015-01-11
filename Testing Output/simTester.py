__author__ = 'zeev'

'''
Input:
An output file that main.cpp generates when it takes major_bodies.txt as an input.
A nasa data text file (either realData-1yr.txt or realData-10yr.txt)

Note: both these files track only the 40 major bodies in the solar system.
So the sun, the planets, and the bigger moons. If you want to see more celestial bodies
you'd need to use a different input file for main.cpp and a different nasa data text file.

Note: It is important that both of these new files start on the same day.

Output:
the sum difference in position/velocity and the average difference in position/velocity.
(these are average over all of the celestial bodies so I don't think they have a useful mathematical meaning
but they're useful for us because if these numbers are lower for one simulation than another then that simulation
is better)

The average difference in position/velocity for one celestial body in particular.
'''
simName = "output-1yrRK10.txt"
simulated = open(simName, "r")
realData = open("realData-1yr.txt")

numPlanets = 40

sumDiffPos = 0
sumDiffVel = 0
counter = 0
trackDiffPos = 0
trackDiffVel = 0
#Change this line to track a different object. Make sure the name is right
objectToTrack = "Earth"

for line in realData:
    #Find the real postion and velocity vectors of the planet object.
    realInfo = line.split(" ")
    realPos = [float(realInfo[0])*1000, float(realInfo[1])*1000, float(realInfo[2])*1000]
    realVel = [float(realInfo[3])*1000, float(realInfo[4])*1000, float(realInfo[5])*1000]

    #Find the simulated position and velocity vectors
    simInfo = (simulated.readline()).split(" ")
    simPos = [float(simInfo[3]), float(simInfo[4]), float(simInfo[5])]
    simVel = [float(simInfo[6]), float(simInfo[7]), float(simInfo[8])]

    #Take the difference between the real and simulated position.
    diffPos = [a - b for a,b in zip(realPos, simPos)]
    diffVel = [a - b for a,b in zip(realVel, simVel)]

    #Add the magnitude of the difference vector
    sumDiffPos += (diffPos[0]**2 + diffPos[1]**2 + diffPos[2]**2)**0.5
    sumDiffVel += (diffVel[0]**2 + diffVel[1]**2 + diffVel[2]**2)**0.5

    if (simInfo[0] == objectToTrack):
        trackDiffPos += (diffPos[0]**2 + diffPos[1]**2 + diffPos[2]**2)**0.5
        trackDiffVel += (diffVel[0]**2 + diffVel[1]**2 + diffVel[2]**2)**0.5

    counter += 1

#Take the average difference in position and velocity. Since this is the average difference for all planet objects
#it's mathematically meaningless, but it gives a good idea of how accurate the simulation is.
avgDiffPos = sumDiffPos/counter
avgDiffVel = sumDiffVel/counter
#The average difference in position/velocity for the planet you're interested in.
trackDiffPos = trackDiffPos / (counter / numPlanets)
trackDiffVel = trackDiffVel / (counter / numPlanets)

print("Simulation input file: {}".format(simName))
print("Sum difference in position (in AU): {}".format(sumDiffPos/149597870700))
print("Sum difference in velocity (in AU/s): {}".format(sumDiffVel/149597870700))
print("Average difference in position (in AU): {}".format(avgDiffPos/149597870700))
print("Average difference in velocity (in AU/s): {}\n\n".format(avgDiffVel/149597870700))

print("Average difference in {} position (in AU):{}".format(objectToTrack, trackDiffPos/149597870700))
print("Average difference in {} velocity (in AU/s): {}".format(objectToTrack, trackDiffVel/149597870700))