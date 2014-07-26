"""
Telnet_MB.py
    The program gets the information of 39 bodies in the solar system
    and the sun. It connects to NASA's database vis telnet and extracts
    the required info.
    Outputs the result to major_bodies.txt
    
Helper files:
    helper.py
        contains functions that aid this program
    MB_list.txt
        contains a list of id's of the bodies in the solar system

Jihad El Sheikh, 26 July, 2014
"""

import helper
import telnetlib
WAIT = 5

tn = telnetlib.Telnet("horizons.jpl.nasa.gov", 6775)

#Target Body
tn.read_until(b"Horizons>", WAIT)
tn.write(b"10\n")

#Gotta read info info
info = tn.read_until(b"<cr>: ", WAIT)
info = info.decode("utf-8")

#Stores mass and radius
mass = helper.get_mass(info)

temp = info.partition("Radius (photosphere)")[2].splitlines()[0]
temp = temp.partition('=')[2]
radius = temp.partition('(')[0].strip()
temp = temp.partition('^')[2]
radius = radius + "E" +temp.partition(')')[0].strip()

#Require Ephemeris
tn.write(b"E\n")

#Vectors
tn.read_until(b" : ", WAIT)
tn.write(b"v\n")

#Coordinate Origin
tn.read_until(b" : ", WAIT)
tn.write(b"@0\n")

#Reference plane
tn.read_until(b" : ", WAIT)
tn.write(b"\n")

#Start time
tn.read_until(b" : ", WAIT)
tn.write(b"2014-aug-1\n")

#End time
tn.read_until(b" : ", WAIT)
tn.write(b"2014-aug-2\n")

#Step size
tn.read_until(b" : ", WAIT)
tn.write(b"2d\n")

#Use default output
tn.read_until(b" : ", WAIT)
tn.write(b"n\n")

#Output reference frame
tn.read_until(b" : ", WAIT)
tn.write(b"\n")

#Corrections
tn.read_until(b" : ", WAIT)
tn.write(b"\n")

#Output units [1=KM-S, 2=AU-D, 3=KM-D]
tn.read_until(b" : ", WAIT)
tn.write(b"1\n")

#CSV format [YES,NO]
tn.read_until(b" : ", WAIT)
tn.write(b"\n")

#Label cartesian output [YES, NO]
tn.read_until(b" : ", WAIT)
tn.write(b"\n")

#Output table type [1-6]
tn.read_until(b" : ", WAIT)
tn.write(b"\n")

info = tn.read_until(b"? : ", WAIT)

f = open("major_bodies.txt", 'w')
f.write("40\n")

#convert from bytes to string
info = info.decode("utf-8")

#Getting target name
target = info.partition("Target body name: ")[2]
target = target.split(None,1)[0]

#Getting the core data
entries = info.partition("$$SOE")[2].strip()
entries = entries.partition("$$EOE")[0]
entries = entries.splitlines()

f.write(target + "\n")
f.write(mass + " " + radius + "\n")

for l in entries:
    f.write(l+"\n")

f.flush()

#Grab the list of major bodies
d = open("MB_list.txt",'r')
bodies = []
for l in d:
    bodies.append(l.strip())

for i in range(len(bodies)):
    #Select new case
    tn.write(b"N\n")

    #Target Body
    tn.read_until(b"Horizons>", WAIT)
    tn.write(bodies[i].encode("utf-8")+b"\n")
    #print(bodies[i])

    #Gotta read info info
    info = tn.read_until(b"<cr>: ", WAIT)
    info = info.decode("utf-8")

    #Stores mass and radius
    mass = helper.get_mass(info)
    radius = helper.get_radius(info)

    #Require Ephemeris
    tn.read_until(b"<cr>: ", WAIT)
    tn.write(b"E\n")

    #Vectors
    tn.read_until(b" : ", WAIT)
    tn.write(b"v\n")

    #Coordinate Origin (use previous center)
    tn.read_until(b" : ", WAIT)
    tn.write(b"\n")

    #Reference plane
    tn.read_until(b" : ", WAIT)
    tn.write(b"\n")

    #Start time
    tn.read_until(b" : ", WAIT)
    tn.write(b"2014-aug-1\n")

    #End time
    tn.read_until(b" : ", WAIT)
    tn.write(b"2014-aug-2\n")

    #Step size
    tn.read_until(b" : ", WAIT)
    tn.write(b"2d\n")

    #Use default output
    tn.read_until(b" : ", WAIT)
    tn.write(b"\n")

    info = tn.read_until(b"? : ", WAIT)

    #convert from bytes to string
    info = info.decode("utf-8")

    #Getting target name
    target = info.partition("Target body name: ")[2]
    target = target.split(None,1)[0]

    #Getting the core data
    entries = info.partition("$$SOE")[2].strip()
    entries = entries.partition("$$EOE")[0]
    entries = entries.splitlines()

    f.write(target + "\n")
    f.write(mass + " " + radius + "\n")

    for l in entries:
        f.write(l+"\n")
    f.flush()

#Print legend at the end of file

legend = open("legend.txt", "r")

for line in legend:
    f.write(line)

f.close()
legend.close()
tn.close()
