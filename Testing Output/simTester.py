__author__ = 'zeev'


simName = "output-1yrEuler15min.txt"
simulated = open(simName, "r")
realData = open("realData-1yr.txt")

numPlanets = 39


sumDiffPos = 0
sumDiffVel = 0
counter = 0
trackDiffPos = 0
trackDiffVel = 0
objectToTrack = "Neptune"

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
trackDiffPos = trackDiffPos / (counter / numPlanets)
trackDiffVel = trackDiffVel / (counter / numPlanets)

print("Simulation input file: {}".format(simName))
print("Sum difference in position (in AU): {}".format(sumDiffPos/149597870700))
print("Sum difference in velocity (in AU/s): {}".format(sumDiffVel/149597870700))
print("Average difference in position (in AU): {}".format(avgDiffPos/149597870700))
print("Average difference in velocity (in AU/s): {}".format(avgDiffVel/149597870700))
print("Average difference in {} position (in AU): {}".format(objectToTrack, trackDiffPos/149597870700))
print("Average difference in {} velocity (in AU/s): {}".format(objectToTrack, trackDiffVel/149597870700))