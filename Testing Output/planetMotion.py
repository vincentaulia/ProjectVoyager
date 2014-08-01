__author__ = 'zeev'


from graphics import *
import time

def main():

    inFile = open("output-RK4.txt", "r")
    #output.txt must be created by the main.cpp program
    #If there are other planets in output.txt, this program will ignore them and only diplay the earth, moon and sun.
    win = GraphWin("Planet Motion", 500, 500)
    solX = 0
    solY = 0
    sol = Circle(Point(solX + win.getWidth()/2, solY + win.getHeight()/2), 10)
    sol.setFill("yellow")

    #The format is name, mass, radius, position, velocity, time  for (sol, earth, moon)

    #The maximum distance between the sun and the moon farthest from the sun is 384400000 + 149597870700
    #(distance between earth and moon + distance from sun and earth)
    #(this is just a rough estimate)
    maxMeters = 384400000 + 149597870700
    meterWidth = maxMeters*1.5*2 #The width of the frame represents this many meters.
    #meterWidth * conversion_Factor = win.getWidth()
    conv_Factor = win.getWidth()/meterWidth #the variable conversion_Factor will be used to convert meters into a number of pixels.

    #1. Find the initial position of the moon and the earth and use that to create the Luna and Earth objects of the circle type.
    #2. To do that, I need to read the first three lines in the file. (The first will be discarded, because that's the sun.)

    earthInfo = []
    moonInfo = []

    for line in inFile:

        info = line.split(" ") #info is a list of strings

        if (info[0] == "earth" or info[0] == "Earth" or info[0] == "Terra" or info[0] == "terra"):
            earthInfo = info
        elif (info[0] == "moon" or info[0] == "Moon" or info[0] == "luna" or info[0] == "Luna"):
            moonInfo = info
        #exit the loop after you read data for both the moon and the earth.
        if (earthInfo != [] and moonInfo != []):
            break

    earthX = float(earthInfo[3])*conv_Factor
    earthY = float(earthInfo[4])*conv_Factor
    earth = Circle(Point(earthX + win.getWidth()/2, earthY + win.getHeight()/2), 5)
    earth.setFill('green')

    moonX = float(moonInfo[3])*conv_Factor
    moonY = float(moonInfo[4])*conv_Factor


    #The distance shown between the earth and the moon is MOON_OFFSET times the actual distance.
    MOON_OFFSET = 1
    deltaX = (moonX - earthX)*MOON_OFFSET
    deltaY = (moonY - earthY)*MOON_OFFSET
    luna = Circle(Point(earthX + deltaX + win.getWidth()/2, earthY + deltaY + win.getHeight()/2), 2)
    luna.setFill('grey')

    sol.draw(win)
    earth.draw(win)
    luna.draw(win)

    win.getMouse() # Pause to view result


    for line in inFile:
        #Move the sun to its new position.
        info = line.split(" ")
        if (info[0] == "sol" or info[0] == "Sol" or info[0] == "Sun" or info[0] == "sun"):
            sol.move( float(info[3]) * conv_Factor - solX, float(info[4]) * conv_Factor - solY)
            solX = float(info[3]) * conv_Factor
            solY = float(info[4]) * conv_Factor

        elif (info[0] == "earth" or info[0] == "Earth" or info[0] == "Terra" or info[0] == "terra"):
            #Move the earth to its new position.
            newEarthX = float(info[3])*conv_Factor
            newEarthY = float(info[4])*conv_Factor
            earth.move(newEarthX - earthX, newEarthY - earthY)
            earthX = newEarthX
            earthY = newEarthY
        elif (info[0] == "moon" or info[0] == "Moon" or info[0] == "luna" or info[0] == "Luna"):
            #Move the moon to its new position.

            #newMoonX = float(moonInfo[3])*conv_Factor
            #newMoonY = float(moonInfo[4])*conv_Factor
            moonX = float(info[3])*conv_Factor
            moonY = float(info[4])*conv_Factor
            deltaX = (moonX - earthX)*MOON_OFFSET
            deltaY = (moonY - earthY)*MOON_OFFSET
            #luna.move(newMoonX - moonX, newMoonY - moonY)
            luna.undraw()
            luna = Circle(Point(earthX + deltaX + win.getWidth()/2, earthY + deltaY + win.getHeight()/2), 2)
            luna.draw(win)

        #time.sleep(0.05)


    win.getMouse() # Pause to view result
    win.close()    # Close window when done


main()