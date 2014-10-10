//
//  main.h
//  Project Voyager
//
//  Created by Zeev Suprun on 2014-07-19.
//  Copyright (c) 2014 Zeev Suprun. All rights reserved.
//

#ifndef __Project_Voyager__main__
#define __Project_Voyager__main__

#include <iostream>
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

// This function calculated the force applied by solar radiation pressure on objects in the solar system
mat rad_pressure_accel(mat Position, mat Mass, mat Radius) {

    // Solar Lumonisity and Light Speed constants
    double Solar_Lumonisity = 3.846 * pow(10,26);
    double Light_Speed = 299792458;

    // Position of the sun
    mat Sol_Position = Position.row(0);

    // Number of objects in the array
    int Number_Objects = Position.n_rows;

    mat Acceleration(Number_Objects, 3);
    Acceleration.zeros();

    // Calculate the acceleration due to solar radiation pressure for every object in the list
    // Assumes the space objects are perfectly sperical and have a perfectly absorbing surface
    for (int i = 1; i < Number_Objects; i++) {
        double Distance_Mag = sqrt(pow(Sol_Position(0) - Position(i, 0), 2) + pow(Sol_Position(1) - Position(i, 1), 2) + pow(Sol_Position(2) - Position(i, 2), 2));
        Acceleration.row(i) = (((Solar_Lumonisity / Light_Speed)*(pow(Radius(i, 0), 2) / 4 / pow(Distance_Mag, 2)) / Distance_Mag) * (Position.row(i) - Sol_Position)) / Mass(i,0);
    }

    return Acceleration;
}

void writeData(planet* planetObjs, int numSpaceObjs, mat& vel, mat& pos, unsigned int time, ofstream& outFile) {
	/*Given an array of planet objects, the current time, and an ofstream object
     this function writes the relevant data to a csv file in the format:
     name, mass, radius, position, velocity, time */
	for (int i = 0; i < numSpaceObjs; i++) {
        
        outFile  << planetObjs[i].name << " " << planetObjs[i].mass << " " << planetObjs[i].radius << " ";
        
        //Printing out the position in the x, y and z dimensions.
        for (int j = 0; j < pos.n_cols; j++) {
            outFile << pos(i,j) << " ";
            
        }
        for (int j = 0; j < vel.n_cols; j++) {
            outFile << vel(i,j) << " ";
            
        }
        outFile << time << "\n";
        
    }
}

#endif /* defined(__Project_Voyager__main__) */
