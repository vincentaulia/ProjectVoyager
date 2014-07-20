__author__ = 'zeev'


from graphics import *
import time

def main():

    inFile = open("output.txt", "r")
    #output.txt must be created by my main.cpp program
    #This program will only work with Sol,Earth,Luna in that order.
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

    #The first line has information about the initial position of the sun, which is unnecessary, so we discard it.
    inFile.readline()

    earthInfo = (inFile.readline()).split(",") #earthInfo is a list of strings
    earthX = float(earthInfo[3])*conv_Factor
    earthY = float(earthInfo[4])*conv_Factor
    earth = Circle(Point(earthX + win.getWidth()/2, earthY + win.getHeight()/2), 5)
    earth.setFill('green')

    moonInfo = (inFile.readline()).split(",") #moonInfo is a list of strings
    moonX = float(moonInfo[3])*conv_Factor
    moonY = float(moonInfo[4])*conv_Factor

    #The distance shown between the earth and the moon is 50 times the actual distance.
    deltaX = (moonX - earthX)*50
    deltaY = (moonY - earthY)*50
    luna = Circle(Point(earthX + deltaX + win.getWidth()/2, earthY + deltaY + win.getHeight()/2), 2)
    luna.setFill('grey')

    sol.draw(win)
    earth.draw(win)
    luna.draw(win)

    win.getMouse() # Pause to view result


    for line in inFile:
        #Move the sun to its new position.
        sol.move( float((line.split(","))[3]) * conv_Factor - solX, float((line.split(","))[3]) * conv_Factor - solY)
        solX = float((line.split(","))[3]) * conv_Factor
        solY = float((line.split(","))[4]) * conv_Factor

        #Move the earth to its new position.
        earthInfo = (inFile.readline()).split(",")
        newEarthX = float(earthInfo[3])*conv_Factor
        newEarthY = float(earthInfo[4])*conv_Factor
        earth.move(newEarthX - earthX, newEarthY - earthY)
        earthX = newEarthX
        earthY = newEarthY

        #Move the moon to its new position.

        moonInfo = (inFile.readline()).split(",")
        #newMoonX = float(moonInfo[3])*conv_Factor
        #newMoonY = float(moonInfo[4])*conv_Factor
        moonX = float(moonInfo[3])*conv_Factor
        moonY = float(moonInfo[4])*conv_Factor
        deltaX = (moonX - earthX)*50
        deltaY = (moonY - earthY)*50
        #luna.move(newMoonX - moonX, newMoonY - moonY)
        luna.undraw()
        luna = Circle(Point(earthX + deltaX + win.getWidth()/2, earthY + deltaY + win.getHeight()/2), 2)
        luna.draw(win)
        time.sleep(.05)


    win.getMouse() # Pause to view result
    win.close()    # Close window when done


main()