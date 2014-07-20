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
    inputFile.open("planet_data.txt");
    
    int numSpaceObjs;
    inputFile >> numSpaceObjs;
    
    //Allocating memory for an array of planet objects.
    planet *spaceObjects;
    spaceObjects = new planet[numSpaceObjs];
    
    //Temporary variables used to read values into the position and velocity matrices
    double xPos, yPos, zPos, xVel, yVel, zVel;

    //The following loop initializes every element in the array of planet objects
    //The data is written in the format:
    //name mass radius x-Position yPosition zPosition xVelocity yVelocity zVelocity
    for (int i = 0; i < numSpaceObjs; i++) {
    
        inputFile >> spaceObjects[i].name;
        inputFile >> spaceObjects[i].mass;
        inputFile >> spaceObjects[i].radius;
        
        inputFile >> xPos >> yPos >> zPos;
        inputFile >> xVel >> yVel >> zVel;
    
        spaceObjects[i].position << xPos << yPos << zPos << endr;
        spaceObjects[i].velocity << xVel << yVel << zVel << endr;
    }
    inputFile.close();
    //Note that for the placeholder inital conditions
    //the earth, moon and sun are all moving in the same plane, and it is a lunar eclipse.
    //this is to simplify things.
    
    
    //Now we are initializing the Mass, Position and Velocity matrices
    //with the values from our planet objects (can be N large)
    mat Mass(numSpaceObjs,1);
    mat Position(numSpaceObjs,3);
    mat Velocity(numSpaceObjs,3);
    for (int i = 0; i < numSpaceObjs; i++) {
        Mass.row(i) = spaceObjects[i].mass;
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
     a Runge-Kutta 5th order approximation.   */
    
	int h = 60*60*24;		//h = step size. For now, 1 day.
	int t = 0;		//t is the running time track. Starts at t = 0
	int t_final = 24*60*60*365;	//Must be an integer! Currently set to ~1 year.
	mat Acceleration(Velocity.n_rows,Velocity.n_cols);
	Acceleration.zeros();
    
	while (t <= t_final) {
        
        //This writes the planet data to the output file.
        writeData(spaceObjects, numSpaceObjs, Velocity, Position, t, outputFile);
        //start of first order Euler approximation.
		Acceleration = grav_accel(Position,Mass);
		Velocity = Velocity + h*Acceleration;
		Position = Position + h*Velocity;
		t = t + h;
	}
    /*************************************************************
     *************************************************************
                        End of placeholder code
     *************************************************************
     *************************************************************/
    
    delete [] spaceObjects;
    outputFile.close();
    return 0;
}

