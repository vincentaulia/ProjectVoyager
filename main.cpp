//
//  main.cpp
//  Project Voyager
//  Created by Zeev Suprun on 2014-06-21.
//

#include <iostream>
#include <fstream>
#include <armadillo>
using namespace std;
using namespace arma;
#define numSpaceObjs 3

class planet {
    
public:
    //Planet's mass in kg.
    double mass;
    std::string name;
    //The radius of the planet in meters.
    double radius;
    //The radius of the planet's sphere of influence. (this variable is dummied out for now)
    //double rad_sphere_of_influence;
    
    //A matrix for the planet's position and velocity in cartesian coordinates. (in meters and m/s for now)
    //(at a later time, more appropriate units should be selected.)
    mat position;
    mat velocity;
    
};

//This function was taken from Zach's example code.
mat grav_accel(mat Position, mat Mass) {
	/*This function will calculate the gravitational attraction on each mass by every other mass. This should work for
     n masses, with thier locations given by Location (n x 3) with each row giving the x,y,z coordinates of each mass in
     Mass, such that the first row of Location corresponds to the first row of Mass.*/
    
	double G = datum::G;
	mat Acceleration(Mass.n_rows,3);
	Acceleration.zeros();
    
	for (int i = 0; i < Mass.n_rows; i++){
		for (int j = 0; j < Mass.n_rows; j++){
			if (i != j) {
				Acceleration.row(i) = Acceleration.row(i) + G*Mass(j,0)*((Position.row(j) - Position.row(i))/pow(norm(Position.row(j) - Position.row(i),2),3));
			}
		}
	}
	return Acceleration;
}

void writeData(planet* planetObjs, mat& vel, mat& pos, int time, ofstream& outFile) {
	/*Given an array of planet objects, the current time, and an ofstream object
      this function writes the relevant data to a csv file in the format:
      name, mass, radius, position, velocity, time */
	for (int i = 0; i < numSpaceObjs; i++) {
    
        outFile  << planetObjs[i].name << ", " << planetObjs[i].mass << ", " << planetObjs[i].radius << ", ";
        
        //Printing out the position in the x, y and z dimensions.
        for (int j = 0; j < pos.n_cols; j++) {
            outFile << pos(i,j) << ", ";
            
        }
        for (int j = 0; j < vel.n_cols; j++) {
            outFile << vel(i,j) << ", ";
            
        }
        outFile << time << "\n";
        
        
    }
}

int main(int argc, char** argv)
{
    ofstream outputFile;
    outputFile.open("output.txt");
    
    //Creating and populate the array of planets.
    /*************************************************************
     *************************************************************
                    Start of placeholder code
     *************************************************************
     ************************************************************/

    //Initializing an array of planet objects.
    //In the final program all this data will be read from a text file or polled from the database
    planet spaceObjects[numSpaceObjs];
    
    //Initializing the first planet object: Sol, with appropriate values. Assume center of the system.
    spaceObjects[0].name = "Sol";
	spaceObjects[0].mass = 1.989e30;
    spaceObjects[0].radius = 695500000;
	spaceObjects[0].position << 0 << 0 << 0 << endr;
	spaceObjects[0].velocity << 0 << 0 << 0 << endr;
    
    
    //Initializing the second planet object: Earth, at it's average distance from the sun.
	spaceObjects[1].name = "Earth";
	spaceObjects[1].mass = 5.97219e24;
    spaceObjects[1].radius = 6371000;
	spaceObjects[1].position << 149597870700 << 0 << 0 << endr;
	spaceObjects[1].velocity << 0 << 30000 << 0 << endr;
    
    //Initializing the second planet object, the Moon, at its average distance from earth
    //Note that in this placeholder code
	spaceObjects[2].name = "Luna";
	spaceObjects[2].mass = 6.3477e22;
    spaceObjects[2].radius = 1737100;
	spaceObjects[2].position << 384400000 + 149597870700 << 0 << 0 << endr;
	spaceObjects[2].velocity << 0 << 1023 + 30000 << 0 << endr;
    
    //Note that for the initial conditions of the placeholder code,
    //the earth moon and sun are all moving in the same plane, and it is a lunar eclipse.
    //this is to simplify things.
    
    /*************************************************************
     *************************************************************
                        End of placeholder code
     *************************************************************
     ************************************************************/
    
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
                    Start of more placeholder code
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
        writeData(spaceObjects, Velocity, Position, t, outputFile);
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
    
    outputFile.close();
    return 0;
}

