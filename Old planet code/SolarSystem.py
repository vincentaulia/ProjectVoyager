"""
SolarSystem.py
    The program pulls the data of the Sun and the 8 planets in the solar system
    from NASA database... sorta

    Jihad El Sheikh, 24 July, 2014
"""

from selenium import webdriver
from selenium.webdriver.common.keys import Keys

driver = webdriver.Chrome()

#Reset all settings
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?reset=1")


#Ephemeris Type
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_type=1#top")
element = driver.find_element_by_xpath(".//input[@type='radio' and @value='VECTORS']")
element.click()

element = driver.find_element_by_name("set_table_type")
element.click()

#Target Body
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_body=1#top")
element = driver.find_element_by_name("show_mb_list")
element.click()

element = driver.find_element_by_xpath("//select[@name='body']/option[@value='MB:10']")
element.click()

element = driver.find_element_by_name("select_body")
element.click()

#Observer Location
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_loc=1#top")

element = driver.find_element_by_name("l_str")
element.send_keys("@0")
element.send_keys(Keys.ENTER)

#Time Span
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_time=1#top")
element = driver.find_element_by_name("start_time")
element.clear()
element.send_keys("2014-08-01")

element = driver.find_element_by_name("stop_time")
element.clear()
element.send_keys("2014-08-02")

element = driver.find_element_by_name("step_size")
element.clear()
element.send_keys("2")

element = driver.find_element_by_name("set_time_span")
element.click()

"""
#Table Settings
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_tset=1#top")

#Uncomment below if you want csv format
#element = driver.find_element_by_name("csv_format")
#element.click()

#Uncommnet below if you don't wan't object details
#element = driver.find_element_by_name("obj_data")
#element.click()

element = driver.find_element_by_name("set_table")
element.click()
"""

#Display/Output
driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_disp=1#top")
element = driver.find_element_by_xpath(".//input[@type='radio' and @value='TEXT']")
element.click()

element = driver.find_element_by_name("set_display")
element.click()

#Generate Data
element = driver.find_element_by_name("go")
element.click()

#Saving the data
source = driver.page_source

#Getting target name
target = source.partition("Target body name: ")[2]
target = target.split(None,1)[0]

#Getting the mass
temp = source.partition("Mass")[2]
temp = temp.splitlines()[0]
temp = temp.partition('^')[2]
exp = temp.partition('k')[0].strip()
temp = temp.replace('~', '=')
temp = temp.partition('=')[2]
mass = temp.split()[0].strip()
mass = mass + "E" + exp

#Getting the radius
temp = source.partition("Radius (photosphere)")[2].splitlines()[0]
temp = temp.partition('=')[2]
radius = temp.partition('(')[0].strip()
temp = temp.partition('^')[2]
radius = radius + "E" +temp.partition(')')[0].strip()

#Getting the core data
entries = source.partition("$$SOE\n")[2]
entries = entries.partition("$$EOE")[0]
entries = entries.splitlines()

f = open('solar_data.txt', 'w')
f.write("9\n")

f.write(target + "\n")
f.write(mass + " " + radius + "\n")

for l in entries:
    f.write(l+"\n")

f.flush()

driver.back()

#Repeat for 8 planets

bodies = ("MB:199", "MB:299", "MB:399", "MB:499", "MB:599", "MB:699", "MB:799", "MB:899")

for i in range(8):

    #Target Body
    driver.get("http://ssd.jpl.nasa.gov/horizons.cgi?s_body=1#top")
    element = driver.find_element_by_name("show_mb_list")
    element.click()
    
    element = driver.find_element_by_xpath("//select[@name='body']/option[@value='"+bodies[i]+"']")
    element.click()
    
    element = driver.find_element_by_name("select_body")
    element.click()

    #Generate Data
    element = driver.find_element_by_name("go")
    element.click()

    #Saving the data
    source = driver.page_source
    driver.back()
    
    #Getting target name
    target = source.partition("Target body name: ")[2]
    target = target.split(None,1)[0]

    #Getting the mass
    temp = source.partition("Mass")[2]
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
    temp = source.partition("radius")[2].splitlines()[0]
    temp = temp.partition('=')[2]
    temp = temp.replace('+', '(')
    radius = temp.partition('(')[0].strip()

    #Getting the core data
    entries = source.partition("$$SOE\n")[2]
    entries = entries.partition("$$EOE")[0]
    entries = entries.splitlines()
    
    f.write(target + "\n")
    f.write("   " + mass + "  " + radius + "\n")
    
    for l in entries:
        f.write(l+"\n")
    f.flush()

#Print legend at the end of file

legend = open("legend.txt", "r")

for line in legend:
    f.write(line)

f.close()
legend.close()

input("Press Enter to exit...")
driver.close()


