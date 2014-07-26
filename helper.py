"""
helper.py
    It contains functions to extract the mass and radius of the solar bodies.

Jihad El Sheikh, July 26, 2014
"""

def get_mass(source):
    temp = source.partition("Mass")[2]
    temp = temp.splitlines()[0]
    temp = temp.replace('(', ' ')
    temp = temp.replace(')', ' ')
    temp = temp.replace('^', ' ')
    temp = temp.replace('+', ' ')
    temp = temp.replace('~', '=')
    temp = temp.split()
    
    p = temp.index('10')
    exp = int(temp[p+1])
    if temp[p+2] == 'g':
        exp -=3

    p = temp.index('=')
    mass = temp[p+1]

    if len(temp) > p+2 and temp[p+2] == '10':
        exp += int(temp[p+3])

    return mass + 'E' + str(exp)

def get_radius(source):
    if 'Pluto' in source:
        temp = source.partition("Radius")[2]
    else:
        temp = source.partition("adius")[2]
        
    temp = temp.splitlines()[0]
    temp = temp.replace('(', ' ')
    temp = temp.replace(')', ' ')
    temp = temp.replace('+', ' ')
    temp = temp.replace('x', ' , ')
    temp = temp.split()

    p = temp.index('=')
    radius = temp[p+1]
    
    if len(temp) > p+5 and temp[p+2] == ',':
        radius += temp[p+2] + temp[p+3] + temp[p+4] + temp[p+5]

    return radius

"""
import telnetlib
WAIT = 5
tn = telnetlib.Telnet("horizons.jpl.nasa.gov", 6775)

f = open("test.txt", 'w')

#Grab the list of major bodies
d = open("MB_list.txt",'r')
bodies = []
for l in d:
    bodies.append(l.strip())
d.close()

for i in bodies:
    #Target Body
    tn.read_until(b"Horizons>", WAIT)
    tn.write(i.encode("utf-8")+b"\n")

    #Gotta read info info
    info = tn.read_until(b"<cr>: ", WAIT)
    info = info.decode("utf-8")

    mass = get_mass(info)
        
    f.write(i + ": ")
    f.write(mass + ' ')

    radius = get_radius(info)

    f.write(radius + "\n")
    f.flush()
    tn.write(b"\n")


f.close()
tn.close()
"""
