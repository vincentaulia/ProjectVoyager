__author__ = 'zeev'


simulated = open("output-1yrRK4.txt", "r")
realData = open("realData-1yr.txt")

numPlanets = 40

#for every timestep:
#For every planet, find the magnitude of the vector difference.

sumDiffPos = 0
sumDiffVel = 0
counter = 0

for line in realData:
    realInfo = line.split(" ")
    realPos = [float(realInfo[0])*1000, float(realInfo[1])*1000, float(realInfo[2])*1000]
    realVel = [float(realInfo[3])*1000, float(realInfo[4])*1000, float(realInfo[5])*1000]

    simInfo = (simulated.readline()).split(" ")
    simPos = [float(simInfo[3]), float(simInfo[4]), float(simInfo[5])]
    simVel = [float(simInfo[6]), float(simInfo[7]), float(simInfo[8])]

    diffPos = [a - b for a,b in zip(realPos, simPos)]
    diffVel = [a - b for a,b in zip(realVel, simVel)]

    sumDiffPos += (diffPos[0]**2 + diffPos[1]**2 + diffPos[2]**2)**0.5
    sumDiffVel += (diffVel[0]**2 + diffVel[1]**2 + diffVel[2]**2)**0.5

    counter += 1

avgDiffPos = sumDiffPos/counter
avgDiffVel = sumDiffVel/counter

print("Sum difference in position (in AU): {}".format(sumDiffPos/149597870700))
print("Sum difference in velocity (in AU/s): {}".format(sumDiffVel/149597870700))
print("Average difference in position (in AU): {}".format(avgDiffPos/149597870700))
print("Average difference in velocity (in AU/s): {}".format(avgDiffVel/149597870700))