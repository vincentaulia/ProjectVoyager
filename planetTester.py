__author__ = 'zeev'

#This program calculates the average distance between the solar barycenter and the 8 planets,
#and compares that value to the actual average distance.
inFile = open("output.txt", "r")

counter = 0
#Average distance between the solar barycenter and the sun, mercury, venus, earth, mars, jupiter, saturn, uranus, neptune.
averageDistances = [0, 0, 0, 0, 0, 0, 0, 0, 0]

#Read through every line in the file to calculate the distance between the solar barycenter and the given planetary body.
for line in inFile:
    info = line.split(",")
    if(info[0] == "sol" or info[0] == "Sol" or info[0] == "Sun" or info[0] == "sun"):
        averageDistances[0] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "mercury" or info[0] == "Mercury"):
        averageDistances[1] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "venus" or info[0] == "Venus"):
        averageDistances[2] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "earth" or info[0] == "Earth" or info[0] == "Terra" or info[0] == "terra"):
        averageDistances[3] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "mars" or info[0] == "Mars"):
        averageDistances[4] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "jupiter" or info[0] == "Jupiter"):
        averageDistances[5] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "saturn" or info[0] == "Saturn"):
        averageDistances[6] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "uranus" or info[0] == "Uranus"):
        averageDistances[7] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9
    elif (info[0] == "neptune" or info[0] == "Neptune"):
        averageDistances[8] += (float(info[3])**2 + float(info[4])**2 + float(info[5])**2)**0.5
        counter += 1/9

counter = round(counter)
print(counter)

names = ["Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune"]
actualAverageDistances = [0, 0.38, 0.72, 1, 1.52, 5.2, 9, 19.2, 30.1] #unit is AU
differences = [0 for i in range(9)]


for i in range(9):
    averageDistances[i] /= counter
    averageDistances[i] /= 149597870700 #Convert to AU
    differences[i] = averageDistances[i] - actualAverageDistances[i]
    print("{}: \nCalculated average distance from solar barycenter:  {}".format(names[i], averageDistances[i]))
    print("Actual average distance from sun:\t\t\t\t\t{}".format(actualAverageDistances[i]))
    print("Difference:\t\t\t\t\t\t\t\t\t\t\t{}".format(differences[i]))

print("\nList of differences:")
print(differences)