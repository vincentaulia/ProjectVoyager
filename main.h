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


void populate_RK10_coeffs (double a_RK10[17][17], double b_RK10[17]) {
    //This function populates the a_RK10 and b_RK10 arrays
    //I'm putting all this crap in a function in the header file
    //because otherwise it would break up the program flow.
    
    //All these values are taken from a pdf on the Project Voyager drive.
    //They had to be copied out manually,
    //so there's a slight chance something was misstyped but I'm pretty sure nothing was.
    
    
    //b_RK10 array.
    //Note, indexing here is done in the form 1-1, 2-1 instead of 0, 1
    //to make it easier to compare against the table of coefficients
    //on the drive, to make sure none of the coefficients were miss-typed.

    b_RK10[1-1] = (3 + 1/3)*pow(10,-2);
    b_RK10[2-1] = -(3 + 1/3)*pow(10,-2);
    b_RK10[3-1] = -1.2 / 10;
    b_RK10[4-1] = 0;
    b_RK10[5-1] = 0;
    b_RK10[6-1] = -1.3 / 10;
    b_RK10[7-1] = -1.8 / 10;
    b_RK10[8-1] = 0;
    b_RK10[9-1] = 2.77429188517743176508 / 10;
    b_RK10[10-1] = 1.89237478148923490158 / 10;
    b_RK10[11-1] = 2.77429188517743176508 / 10;
    b_RK10[12-1] = 1.89237478148923490158 / 10;
    b_RK10[13-1] = 1.3/10;
    b_RK10[14-1] = 1.8/10;
    b_RK10[15-1] = 1.2/10;
    b_RK10[16-1] = (3 + 1/3)*pow(10,-2);
    b_RK10[17-1] = (3 + 1/3)*pow(10,-2);

    //Matrix of A coefficients, taken from a pdf on the drive, for RK10.
    //Note: There are a lot of blank spaces in this matrix but they are supposed to be that way.
    //Other Note: This array is treated as though it's indexed from one 
    
    a_RK10[2][1] = 5/10;
    a_RK10[3][1] = 2.49297267609681978013 / 10;
    a_RK10[3][2] = 2.77211832531930184738 / 10;
    a_RK10[4][1] = 1.97440912553104561032 / 10;
    a_RK10[4][2] = 0;
    a_RK10[4][3] = 5.92322737659313683095 / 10;
    a_RK10[5][1] = 1.97320548628702140900 / 10;
    a_RK10[5][2] = 0;
    a_RK10[5][3] = 2.95083334092671853711 / 10;
    a_RK10[5][4] = -9.84803125957023833277 / 100;
    a_RK10[6][1] = 1.31313417344461520076 / 10;
    a_RK10[6][2] = 0;
    a_RK10[6][3] = 0;
    a_RK10[6][4] = 1.10154439538638507040 / 10;
    a_RK10[6][5] = 5.25186129370448772884 / 10;
    a_RK10[7][1] = 1.34200341846322406193 / 10;
    a_RK10[7][2] = 0;
    a_RK10[7][3] = 0;
    a_RK10[7][4] = 6.96088703288076908079 / 10;
    a_RK10[7][5] = 2.50497721570339375352 / 10;
    a_RK10[7][6] = -7.91023116492320445498 / 10;
    a_RK10[8][1] = 7.22182741896621454448 / 100;
    a_RK10[8][2] = 0;
    a_RK10[8][3] = 0;
    a_RK10[8][4] = 0;
    a_RK10[8][5] = -5.83363229364550369126 / 100;
    a_RK10[8][6] = 3.04755766857449437925 / 1000;
    a_RK10[8][7] = 9.15481802977846100286 / 100;
    a_RK10[9][1] = 3.12550081351656170620 / 100;
    a_RK10[9][2] = 0;
    a_RK10[9][3] = 0;
    a_RK10[9][4] = 0;
    a_RK10[9][5] = 0;
    a_RK10[9][6] = 1.09123821542419946873*pow(10,-4);
    a_RK10[9][7] = 1.56725758630995015164 / 10;
    a_RK10[9][8] = 1.69294351171974399670 / 10;
    a_RK10[10][1] = 1.19066044146750321445 / 100;
    a_RK10[10][2] = 0;
    a_RK10[10][3] = 0;
    a_RK10[10][4] = 0;
    a_RK10[10][5] = 0;
    a_RK10[10][6] = 2.83437082024606548112 / 10;
    a_RK10[10][7] = -4.16312167570561315056 / 10;
    a_RK10[10][8] = 2.64646333949743004837 / 10;
    a_RK10[10][9] = 7.38849809146269076388 / 10;
    a_RK10[11][1] = 2.34065736913354493717 / 100;
    a_RK10[11][2] = 0;
    a_RK10[11][3] = 0;
    a_RK10[11][4] = 0;
    a_RK10[11][5] = 0;
    a_RK10[11][6] = 9.44931301894961802240 / 100;
    a_RK10[11][7] = -2.72872055901956419006 / 10;
    a_RK10[11][8] = 2.24022046115592207410 / 10;
    a_RK10[11][9] = 6.04381441075135095416 / 10;
    a_RK10[11][10] = -3.08153769292799652586 / 100;
    a_RK10[12][1] = 4.54437753101763699408 / 100;
    a_RK10[12][2] = 0;
    a_RK10[12][3] = 0;
    a_RK10[12][4] = 0;
    a_RK10[12][5] = 0;
    a_RK10[12][6] = -1.18799667186441567723 / 1000;
    a_RK10[12][7] = 1.20356549909281134802 / 100;
    a_RK10[12][8] = 7.51269029876479240591 / 100;
    a_RK10[12][9] = -1.82209240988845690412 / 100;
    a_RK10[12][10] = -2.57152854084065042855*pow(10,-4);
    a_RK10[12][11] = 4.53207837134829585506 / 1000;
    a_RK10[13][1] = 1.78401086400436429292 / 10;
    a_RK10[13][2] = 0;
    a_RK10[13][3] = 0;
    a_RK10[13][4] = 1.10154439538638507040 / 10;
    a_RK10[13][5] = 5.25186129370448772884 / 10;
    a_RK10[13][6] = -4.89148591820436212803 / 10;
    a_RK10[13][7] = 9.32443612635135733038 / 10;
    a_RK10[13][8] = -7.74475053439839525409 / 10;
    a_RK10[13][9] = -1.05490217813935824270;
    a_RK10[13][10] = 1.31046712034157154509 /10;
    a_RK10[13][11] = 5.87049777599487392267 / 10;
    a_RK10[13][12] = 6.20898052074878791881 / 10;
    a_RK10[14][1] = 1.30220806600497793496 / 10;
    a_RK10[14][2] = 0;
    a_RK10[14][3] = 0;
    a_RK10[14][4] = 6.96088703288076908079 / 10;
    a_RK10[14][5] = 2.50497721570339375352 / 10;
    a_RK10[14][6] = -7.58948987129607342662 / 10;
    a_RK10[14][7] = -1.71517208463488383577 / 10;
    a_RK10[14][8] = -3.70217673678906704688 / 10;
    a_RK10[14][9] = 1.24981008574747347802 / 10;
    a_RK10[14][10] = 3.35310924837267073965 / 1000;
    a_RK10[14][11] = -6.63254613676153581907 /1000;
    a_RK10[14][12] = 4.29116573121617904714 / 10;
    a_RK10[14][13] = -3.71778567824697893108 / 100;
    a_RK10[15][1] = 2.49297267609681978013 / 10;
    a_RK10[15][2] = 2.77211832531930184738 / 10;
    a_RK10[15][3] = 0;
    a_RK10[15][4] = 0;
    a_RK10[15][5] = 0;
    a_RK10[15][6] = -1.45940595936085218185 / 10;
    a_RK10[15][7] = -7.99015893511029475358 / 10;
    a_RK10[15][8] = 0;
    a_RK10[15][9] = 0;
    a_RK10[15][10] = 0;
    a_RK10[15][11] = 0;
    a_RK10[15][12] = 0;
    a_RK10[15][13] = 1.45940595936085218185 / 10;
    a_RK10[15][14] = 7.99015893511029475358 / 10;
    a_RK10[16][1] = 5 / 10;
    a_RK10[16][2] = 0;
    a_RK10[16][3] = -8.07097076095341093251 / 10;
    //put a 0 in every spot between a[16][4] and a[16][14], inclusive.
    for (int zero_Counter = 4; zero_Counter < 15; zero_Counter++) {
        a_RK10[16][zero_Counter] = 0;
    }
    a_RK10[16][15] = 8.07097076095341093251 / 10;
    a_RK10[17][1] = 5.73207954320575412321 / 100;
    a_RK10[17][2] = -5 / 10;
    a_RK10[17][3] = -8.97470163394855120846 / 10;
    a_RK10[17][4] = 0;
    a_RK10[17][5] = 0;
    a_RK10[17][6] = -1.03991004922695343354;
    a_RK10[17][7] = -4.07357014288385809022 / 10;
    a_RK10[17][8] = -1.82830236640741849663 / 10;
    a_RK10[17][9] = -3.33659270649225021137 / 10;
    a_RK10[17][10] = 3.95638542376057924001 / 10;
    a_RK10[17][11] = 6.95057049459982281780 / 10;
    a_RK10[17][12] = 2.71487376457383239111 / 10;
    a_RK10[17][13] = 5.85423734866589756811 / 10;
    a_RK10[17][14] = 9.58819072213235370429 / 10;
    a_RK10[17][15] = 8.97470163394855120846 / 10;
    a_RK10[17][16] = 5 / 10;

}
#endif /* defined(__Project_Voyager__main__) */
