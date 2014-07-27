__author__ = 'zeev'


inFile = open("output.txt", "r")

counter = 0
#Average distance between the solar barycenter and the sun, mercury, venus, earth, mars, jupiter, saturn, uranus, neptune.
averageDistances = [0, 0, 0, 0, 0, 0, 0, 0, 0]

#Read through every line in the file to calculate the distance between the solar barycenter and the given planetary body.
for line in inFile:
    info = line.split(",")
    averageDistances[0] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5

    for i in range(1,9):
        info = (inFile.readline()).split(",")
        averageDistances[i] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5

    counter += 1


actualAverageDistances = [0, 0.38, 0.72, 1, 1.52, 5.2, 9, 19.2, 30.1] #unit is AU

for i in range(9):
    averageDistances[i] /= counter
    averageDistances[i] /= 149597870700 #Convert to AU
    averageDistances[i] = averageDistances[i] - actualAverageDistances[i]




print(averageDistances)