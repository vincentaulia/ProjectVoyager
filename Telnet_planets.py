"""
Telnet_planets.py
    The program accesses NASA's database using telnet and extracts the
    required information about the sun and the eight planets

    Jihad El Sheikh, 25 July, 2014
"""
import telnetlib
WAIT = 5

tn = telnetlib.Telnet("horizons.jpl.nasa.gov", 6775)

#Target Body
tn.read_until(b"Horizons>", WAIT)
tn.write(b"10\n")

#Gotta read info info
info = tn.read_until(b"<cr>: ", WAIT)
info = info.decode("utf-8")

#Getting the mass
temp = info.partition("Mass")[2]
temp = temp.splitlines()[0]
temp = temp.partition('^')[2]
exp = temp.partition('k')[0].strip()
temp = temp.replace('~', '=')
temp = temp.partition('=')[2]
mass = temp.split()[0].strip()
mass = mass + "E" + exp

#Getting the radius
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
tn.write(b"2\n")

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

f = open("solar_data.txt", 'w')

f.write("9\n")
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

bodies = ['199','299','399','499','599','699','799','899']

for i in range(8):
    #Select new case
    tn.write(b"N\n")

    #Target Body
    tn.read_until(b"Horizons>", WAIT)
    tn.write(bodies[i].encode("utf-8")+b"\n")

    #Gotta read info info
    info = tn.read_until(b"<cr>: ", WAIT)
    info = info.decode("utf-8")

    #Getting the mass
    temp = info.partition("Mass")[2]
    temp = temp.splitlines()[0]
    temp = temp.partition('^')[2]
    exp = temp.partition('k')[0].strip()
    temp = temp.replace('~', '=')
    temp = temp.partition('=')[2]
    mass = temp.split()[0].strip()
    if '+' in mass:
        mass = mass.partition('+')[0]
    mass = mass + "E" + exp
    
    #Getting the radius
    temp = info.partition("radius")[2].splitlines()[0]
    temp = temp.partition('=')[2]
    temp = temp.replace('+', '(')
    radius = temp.partition('(')[0].strip()

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
