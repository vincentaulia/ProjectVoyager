//
//  main.cpp
//  Project Voyager
//  Created by Zeev Suprun on 2014-06-21.
//

#include <armadillo>
using namespace std;
using namespace arma;
#include "main.h"


int main(int argc, char** argv)
{
    ofstream outputFile;
    outputFile.open("output.txt");
    ifstream inputFile;
    inputFile.open("major_bodies.txt");
    
    int numSpaceObjs;
    inputFile >> numSpaceObjs;
    
    //Allocating memory for an array of planet objects.
    planet *spaceObjects;
    spaceObjects = new planet[numSpaceObjs];
    
    //Temporary variables used to read values into the position and velocity matrices
    double xPos, yPos, zPos, xVel, yVel, zVel;

    //The following loop initializes every element in the array of planet objects
    for (int i = 0; i < numSpaceObjs; i++) {
    
        inputFile >> spaceObjects[i].name;
        inputFile >> spaceObjects[i].mass;
        inputFile >> spaceObjects[i].radius;
        inputFile.ignore(10000,'\n');        //Ignore the rest of the line
        //(this means it will only read the first radius for planets with 3 radii)
        inputFile.ignore(10000,'\n');        //Ignore a line.
        inputFile >> xPos >> yPos >> zPos;
        inputFile >> xVel >> yVel >> zVel;
        
        //Converting position from km to meters.
        xPos = xPos * 1000;
        yPos = yPos * 1000;
        zPos = zPos * 1000;
        //Converting velocity from km/s to meters/second.
        xVel = xVel * 1000;
        yVel = yVel * 1000;
        zVel = zVel * 1000;
        
        spaceObjects[i].position << xPos << yPos << zPos << endr;
        spaceObjects[i].velocity << xVel << yVel << zVel << endr;
        inputFile.get();
        inputFile.ignore(10000,'\n');        //Ignore a line.

    }
    inputFile.close();
    
    //Now we are initializing the Mass, Radius, Position and Velocity matrices
    //with the values from our planet objects (can be N large)
    mat Mass(numSpaceObjs, 1);
    mat Radius(numSpaceObjs, 1);
    mat Position(numSpaceObjs, 3);
    mat Velocity(numSpaceObjs, 3);
    for (int i = 0; i < numSpaceObjs; i++) {
        Mass.row(i) = spaceObjects[i].mass;
        Radius.row(i) = spaceObjects[i].radius;
        Position.row(i) = spaceObjects[i].position;
        Velocity.row(i) = spaceObjects[i].velocity;
    }
    
    /*************************************************************
     *************************************************************
                        Start of placeholder code
     *************************************************************
     *************************************************************/
    /*The following is a first order Euler approximation of the motion of the bodies
     This was taken from Zach's example code. This should be eventually replaced with
     a Runge-Kutta 5th order approximation.*/
    
	unsigned int h = 60*60*0.25;		//h = step size, in seconds. Right now it's about 15 minutes.
	unsigned int t = 0;		//t is the running time track. Starts at t = 0
	unsigned int t_final = 24*60*60*365*1;	//Must be an integer!
	mat Acceleration(Velocity.n_rows,Velocity.n_cols);
	Acceleration.zeros();
    
	while (t <= t_final) {
        
        //This writes the planet data to the output file, but only every 24 hours, to avoid huge text files.
        if (t % (60*60*24) == 0) {
            writeData(spaceObjects, numSpaceObjs, Velocity, Position, t, outputFile);
        }
        //start of first order Euler approximation.
		Acceleration = grav_accel(Position,Mass);
        //calculate the acceleration due to solar radiation pressure
        Acceleration += rad_pressure_accel(Position, Mass, Radius);

		Velocity = Velocity + h*Acceleration;
		Position = Position + h*Velocity;
		t = t + h;
	}
    
    /*Above is a commented out Euler approximation. The following is an RK4 approximation.
     *So far, this approximation doesn't seem to be accurate.
    
    unsigned int h = 60*60*0.25;	 //h = step size, in seconds
    unsigned int t = 0;	 //t is the running time track. Starts at t = 0
    unsigned int t_final = 24*60*60*365*1;	//Must be an integer!
    
    mat k1(Velocity.n_rows, Velocity.n_cols);
    k1.zeros();
    mat l1(Velocity.n_rows, Velocity.n_cols);
    l1.zeros();
    mat k2(Velocity.n_rows, Velocity.n_cols);
    k2.zeros();
    mat l2(Velocity.n_rows, Velocity.n_cols);
    l2.zeros();
    mat k3(Velocity.n_rows, Velocity.n_cols);
    k3.zeros();
    mat l3(Velocity.n_rows, Velocity.n_cols);
    l3.zeros();
    mat k4(Velocity.n_rows, Velocity.n_cols);
    k4.zeros();
    mat l4(Velocity.n_rows, Velocity.n_cols);
    l4.zeros();
    
    while (t <= t_final) {
        //This writes the planet data to the output file, but only every 24 hours.
        if (t % (60*60*24) == 0) {
            writeData(spaceObjects, numSpaceObjs, Velocity, Position, t, outputFile);
        }
        //start of RK4 approximation.
        k1 = grav_accel(Position,Mass);
        l1 = Velocity;
        k2 = grav_accel(Position + l1/2, Mass);
        l2 = Velocity + k1/2;
        k3 = grav_accel(Position + l2/2, Mass);
        l3 = Velocity + k2/2;
        k4 = grav_accel(Position + l3, Mass);
        l4 = Velocity + k3;
        
        Velocity = Velocity + h/6*(k1 + 2*k2 + 2*k3 + k4);
        Position = Position + h/6*(l1 + 2*l2 + 2*l3 + l4);
        t = t + h;
    }*/
    /*************************************************************
     *************************************************************
                        End of placeholder code
     *************************************************************
     *************************************************************/
    
    delete [] spaceObjects;
    outputFile.close();
    return 0;
}

