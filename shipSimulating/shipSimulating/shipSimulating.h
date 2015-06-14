//  main.h
//  Project Voyager
//
//  Created by Zeev Suprun on 2014-07-19.
//  Copyright (c) 2014 Zeev Suprun. All rights reserved.

#ifndef __Project_Voyager__main__
#define __Project_Voyager__main__

#include <iostream>
#include <iomanip>      // std::setprecision
#include <math.h>
#include <armadillo>

using namespace std;
using namespace arma;


class planet {
    
public:
    //Planet's mass in kg.
    double mass;
    string name;
    //The radius of the planet in meters.
    double radius;
    //The radius of the planet's sphere of influence. (this variable is dummied out for now)
    //double rad_sphere_of_influence;
    
    //A matrix for the planet's position and velocity in cartesian coordinates. (in meters and m/s for now)
    //(at a later time, more appropriate units should be selected.)
    mat position;
    mat velocity;
    
};

struct Elements
{
    //This class holds all the orbital elements of a planet.
public:
    //The following variables must all be read in from somewhere.
    //Mass of the object
     double mass;
    //Mass of the object it's orbiting
    double massFocus;
    //The semi-major axis (in meters)
    double axis;
    //eccentricity
    double ecc;
    //inclination (in radians)
    double incl;
    //longitude of ascending node (in radians)
    double asc;
    //mean anomaly (in radians)
    double anom;
    //argument of periapsis (in radians)
    double arg;
    //1 for Prograde and -1 for Retrograde
    int dir;
    //ID of the body it is orbiting around
    string IDFocus;
    //name of the body
    string name;
    //sphere of influence
    double soi;
    //radius of the body (in meters)
    double radiusx;
    double radiusy;
    double radiusz;
    
    //needed for the ships
    double dryMass;
    double fuelMass;
    double Isp;
    double deltaVbudget;
	
    //The variables P, Q, and n must be calculated using the other orbital elements.
    //If any of the other orbital elements change, P, Q, and n must be recalculated.
    //(the other orbital elements won't be changing for planets, but they will be for ships).
    
    mat P;
    mat Q;
    mat W;
    
    double n;
	
    void calcData ()
    {
        //This function uses the other orbital elements to calculate P, Q, and n, which are al based on other orbital elements.
        //It MUST be called every time any of the orbital elements change.
		
        //Calculating P
        double Px =  cos(arg) *  cos (asc) -  sin (arg) *  cos (incl) *  sin (asc);
        double Py =  cos(arg) *  sin (asc) +  sin (arg) *  cos (incl) *  cos (asc);
        double Pz =  sin(arg) *  sin (incl);
		
        P.set_size(3,0);
        P(0,0) = Px;
        P(1,0) = Py;
        P(2,0) = Pz;
        
        //Calculating Q
        double Qx = - sin (arg) *  cos (asc) -  cos (arg) *  cos (incl) *  sin (asc);
        double Qy = - sin (arg) *  sin (asc) +  cos (arg) *  cos (incl) *  cos (asc);
        double Qz =  sin (incl) *  cos (arg);

        Q.set_size(3,0);
        Q(0,0) = Qx;
        Q(1,0) = Qy;
        Q(2,0) = Qz;
        
        //Calculating W
        double Wx = sin(incl) *  sin(asc);
        double Wy = -1 *  sin(incl) *  cos(asc);
        double Wz =  cos(incl);

        W.set_size(3,0);
        W(0,0) = Wx;
        W(1,0) = Wy;
        W(2,0) = Wz;
		
        // Calculating n
        n = sqrt ((6.67384e-11) * (mass + massFocus) / (axis * axis * axis)) * dir;
        //Debug.Log (dir);
    }
    
    bool operator == (const Elements& y)
    {
        if (axis != y.axis) {
            if (ecc == y.ecc and incl == y.incl and asc == y.asc and \
                anom == y.anom and arg == y.arg and dir == y.dir and IDFocus == y.IDFocus) {
                return true;
            }
        }
        return false;
    }
    
    bool operator != (const Elements& y)
    {
        return !(*this == y);
    }
};

mat findPos (Elements el, long time)
{
    //This function finds the position of a planet given a bunch of orbital parameters and some other stuff.
    
    double anom = el.anom + el.n * time;
    double E = anom;
    double Enext = E;
    //Normally epsilon should be much smaller than this, but for now the program takes too long with small epsilons.
    double epsilon = pow (10, -10);
    int count = 0;
    
    do {
        count ++;
        E = Enext;
        Enext = E - ((E - el.ecc * sin (E) - anom) / (1 - el.ecc * cos (E)));
        
    } while (abs(Enext - E) > epsilon && count < 50);
    
    if (count == 50) {
        cout << ("Epsilon Crash: " + el.name);
    }
    
    mat R(3, 1);
    //*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
    R = (el.axis * (cos(E) - el.ecc)) * el.P + (el.axis * sqrt (1 - el.ecc * el.ecc) * sin(E)) * el.Q;
    
    //interchange the y and z components to move the planets in the plane of the game
    double y = R(1,0);
    R(1,0) = R(2,0);
    R(2,0) = y;
    
    //if it's a ship, get the id of it's orbit focus
    //if (body.name.Contains ("Ship")) {
    
    //orbiting = GameObject.Find (el.IDFocus);
    //R += orbiting.transform.position;
    
    //}
    //Don't need this anymore. Will make moons children of planets
    /*else {
     
     
     
     //this is to add the vector to the object it is orbiting
     int objectID = int.Parse (body.name);
     
     //if the id ends with 99, then it orbits the sun (10)
     //if the id is something else, then it orbits a planet
     if (objectID == 10)
     ;
     else if (objectID % 100 == 99) {
     
     orbiting = GameObject.Find ("10");
     //Debug.Log (body.name + ": " + orbiting.name);
     R += orbiting.transform.position;
     } else {
     int orbiting_id;
     
     orbiting_id = (objectID / 100) * 100 + 99;
     orbiting = GameObject.Find (orbiting_id.ToString ());
     //Debug.Log (body.name + ": " + orbiting.name);
     R += orbiting.transform.position;
     
     }
     
     
     }*/
    
    return R;
}

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
    outFile << setprecision(20);
    
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

/*
 mat c_RKN10(17, 1);
 mat A_RKN10(17, 17);
 mat Aprime_RKN10(17, 17);
 mat Bhat_RKN10(17,1);
 mat Bphat_RKN10(17, 1);
 mat B_RKN10(17, 1);
 
 //Populate the matrices with a function call. (all but g_RKN10 because that's different for each one)
 populate_RKN10_coeffs (c_RKN10, A_RKN10, Bhat_RKN10, Bphat_RKN10, B_RKN10);
 
 */

void populate_RKN10_coeffs (mat &c_RKN10, mat &A_RKN10, mat &Bhat_RKN10, mat &Bphat_RKN10, mat &B_RKN10, mat &Bprime_RKN10) {
    
    c_RKN10(0, 0) = 0.0e0;
    c_RKN10(1, 0) = 2.0e-2;
    c_RKN10(2, 0) = 4.0e-2;
    c_RKN10(3, 0) = 1.0e-1;
    c_RKN10(4, 0) = 1.33333333333333333333333333333e-1;
    c_RKN10(5, 0) = 1.6e-1;
    c_RKN10(6, 0) = 5.0e-2;
    c_RKN10(7, 0) = 2.0e-1;
    c_RKN10(8, 0) = 2.5e-1;
    c_RKN10(9, 0) = 3.33333333333333333333333333333e-1;
    c_RKN10(10, 0) = 5.0e-1;
    c_RKN10(11, 0) = 5.55555555555555555555555555556e-1;
    c_RKN10(12, 0) = 7.5e-1;
    c_RKN10(13, 0) = 8.57142857142857142857142857143e-1;
    c_RKN10(14, 0) = 9.45216222272014340129957427739e-1;
    c_RKN10(15, 0) = 1.0e0;
    c_RKN10(16, 0) = 1.0e0;
    
    Bhat_RKN10(0,0) = 1.21278685171854149768890395495e-2;
    Bhat_RKN10(1,0) = 0.0e0;
    Bhat_RKN10(2,0) = 0.0e0;
    Bhat_RKN10(3,0) = 0.0e0;
    Bhat_RKN10(4,0) = 0.0e0;
    Bhat_RKN10(5,0) = 0.0e0;
    Bhat_RKN10(6,0) = 8.62974625156887444363792274411e-2;
    Bhat_RKN10(7,0) = 2.52546958118714719432343449316e-1;
    Bhat_RKN10(8,0) = -1.97418679932682303358307954886e-1;
    Bhat_RKN10(9,0) = 2.03186919078972590809261561009e-1;
    Bhat_RKN10(10,0) = -2.07758080777149166121933554691e-2;
    Bhat_RKN10(11,0) = 1.09678048745020136250111237823e-1;
    Bhat_RKN10(12,0) = 3.80651325264665057344878719105e-2;
    Bhat_RKN10(13,0) =  1.16340688043242296440927709215e-2;
    Bhat_RKN10(14,0) =  4.65802970402487868693615238455e-3;
    Bhat_RKN10(15,0) =  0.0e0;
    Bhat_RKN10(16,0) =  0.0e0;
    
    // BprimeHat (high-order b-prime)
    Bphat_RKN10(0,0) = 1.21278685171854149768890395495e-2;
    Bphat_RKN10(1,0) = 0.0e0;
    Bphat_RKN10(2,0) = 0.0e0;
    Bphat_RKN10(3,0) = 0.0e0;
    Bphat_RKN10(4,0) = 0.0e0;
    Bphat_RKN10(5,0) = 0.0e0;
    Bphat_RKN10(6,0) = 9.08394342270407836172412920433e-2;
    Bphat_RKN10(7,0) = 3.15683697648393399290429311645e-1;
    Bphat_RKN10(8,0) = -2.63224906576909737811077273181e-1;
    Bphat_RKN10(9,0) = 3.04780378618458886213892341513e-1;
    Bphat_RKN10(10,0) = -4.15516161554298332243867109382e-2;
    Bphat_RKN10(11,0) = 2.46775609676295306562750285101e-1;
    Bphat_RKN10(12,0) = 1.52260530105866022937951487642e-1;
    Bphat_RKN10(13,0) = 8.14384816302696075086493964505e-2;
    Bphat_RKN10(14,0) = 8.50257119389081128008018326881e-2;
    Bphat_RKN10(15,0) = -9.15518963007796287314100251351e-3;
    Bphat_RKN10(16,0) = 2.5e-2;
    
    // B (low-order b)
    B_RKN10(0,0) = 1.70087019070069917527544646189e-2;
    B_RKN10(1,0) =     0.0e0;
    B_RKN10(2,0) =     0.0e0;
    B_RKN10(3,0) =     0.0e0;
    B_RKN10(4,0) =     0.0e0;
    B_RKN10(5,0) =     0.0e0;
    B_RKN10(6,0) =     7.22593359308314069488600038463e-2;
    B_RKN10(7,0) =     3.72026177326753045388210502067e-1;
    B_RKN10(8,0) =     -4.01821145009303521439340233863e-1;
    B_RKN10(9,0) =     3.35455068301351666696584034896e-1;
    B_RKN10(10,0) =     -1.31306501075331808430281840783e-1;
    B_RKN10(11,0) =     1.89431906616048652722659836455e-1;
    B_RKN10(12,0) =     2.68408020400290479053691655806e-2;
    B_RKN10(13,0) =     1.63056656059179238935180933102e-2;
    B_RKN10(14,0) =     3.79998835669659456166597387323e-3;
    B_RKN10(15,0) =     0.0e0;
    B_RKN10(16,0) =     0.0e0;
    
    // Bprime (low-order bprime)
    Bprime_RKN10(0,0) = 1.70087019070069917527544646189e-2;
    Bprime_RKN10(1,0) =       0.0e0;
    Bprime_RKN10(2,0) =           0.0e0;
    Bprime_RKN10(3,0) =           0.0e0;
    Bprime_RKN10(4,0) =           0.0e0;
    Bprime_RKN10(5,0) =           0.0e0;
    Bprime_RKN10(6,0) =           7.60624588745593757356421093119e-2;
    Bprime_RKN10(7,0) =           4.65032721658441306735263127583e-1;
    Bprime_RKN10(8,0) =           -5.35761526679071361919120311817e-1;
    Bprime_RKN10(9,0) =           5.03182602452027500044876052344e-1;
    Bprime_RKN10(10,0) =           -2.62613002150663616860563681567e-1;
    Bprime_RKN10(11,0) =           4.26221789886109468625984632024e-1;
    Bprime_RKN10(12,0) =           1.07363208160116191621476662322e-1;
    Bprime_RKN10(13,0) =           1.14139659241425467254626653171e-1;
    Bprime_RKN10(14,0) =           6.93633866500486770090602920091e-2;
    Bprime_RKN10(15,0) =           2.0e-2;
    Bprime_RKN10(16,0) =           0.0e0;
    
    
    
    A_RKN10.zeros();
    //All the -1's are because this was copy and pasted from matlab code and if I did the -1's this way then
    //I could do it using find and replace, but actually fixing the values normally would take more time.
    //(1, 0) ; (2, 0); (2, 1); (3, 0), (3, 1), (3, 2).
    //
    A_RKN10(-1+2,1-1 ) = 2.0e-4;
    
    A_RKN10(-1+3,1-1 ) = 2.66666666666666666666666666667e-4;
    A_RKN10(-1+3,2-1 ) = 5.33333333333333333333333333333e-4;
    
    A_RKN10(-1+4,1-1 ) = 2.91666666666666666666666666667e-3;
    A_RKN10(-1+4,2-1 ) =   -4.16666666666666666666666666667e-3;
    A_RKN10(-1+4,3-1 ) =   6.25e-3;
    
    A_RKN10(-1+5,1-1 ) = 1.64609053497942386831275720165e-3;
    A_RKN10(-1+5,2-1 ) = 0.0e0;
    A_RKN10(-1+5,3-1 ) = 5.48696844993141289437585733882e-3;
    A_RKN10(-1+5,4-1 ) = 1.75582990397805212620027434842e-3;
    
    A_RKN10(-1+6,1-1 ) = 1.9456e-3;
    A_RKN10(-1+6,2-1 ) = 0.0e0;
    A_RKN10(-1+6,3-1 ) = 7.15174603174603174603174603175e-3;
    A_RKN10(-1+6,4-1 ) = 2.91271111111111111111111111111e-3;
    A_RKN10(-1+6,5-1 ) = 7.89942857142857142857142857143e-4;
    
    A_RKN10(-1+7,1-1 ) = 5.6640625e-4;
    A_RKN10(-1+7,2-1 ) = 0.0e0;
    A_RKN10(-1+7,3-1 ) = 8.80973048941798941798941798942e-4;
    A_RKN10(-1+7,4-1 ) = -4.36921296296296296296296296296e-4;
    A_RKN10(-1+7,5-1 ) = 3.39006696428571428571428571429e-4;
    A_RKN10(-1+7,6-1 ) = -9.94646990740740740740740740741e-5;
    
    A_RKN10(-1+8,1-1 ) = 3.08333333333333333333333333333e-3;
    A_RKN10(-1+8,2-1 ) = 0.0e0;
    A_RKN10(-1+8,3-1 ) = 0.0e0;
    A_RKN10(-1+8,4-1 ) = 1.77777777777777777777777777778e-3;
    A_RKN10(-1+8,5-1 ) = 2.7e-3;
    A_RKN10(-1+8,6-1 ) = 1.57828282828282828282828282828e-3;
    A_RKN10(-1+8,7-1 ) = 1.08606060606060606060606060606e-2;
    
    A_RKN10(-1+9,1-1 ) = 3.65183937480112971375119150338e-3;
    A_RKN10(-1+9,2-1 ) = 0.0e0;
    A_RKN10(-1+9,3-1 ) = 3.96517171407234306617557289807e-3;
    A_RKN10(-1+9,4-1 ) = 3.19725826293062822350093426091e-3;
    A_RKN10(-1+9,5-1 ) = 8.22146730685543536968701883401e-3;
    A_RKN10(-1+9,6-1 ) = -1.31309269595723798362013884863e-3;
    A_RKN10(-1+9,7-1 ) = 9.77158696806486781562609494147e-3;
    A_RKN10(-1+9,8-1 ) = 3.75576906923283379487932641079e-3;
    
    A_RKN10(-1+10,1-1 ) = 3.70724106871850081019565530521e-3;
    A_RKN10(-1+10,2-1 ) = 0.0e0;
    A_RKN10(-1+10,3-1 ) = 5.08204585455528598076108163479e-3;
    A_RKN10(-1+10,4-1 ) =1.17470800217541204473569104943e-3;
    A_RKN10(-1+10,5-1 ) =-2.11476299151269914996229766362e-2;
    A_RKN10(-1+10,6-1 ) =6.01046369810788081222573525136e-2;
    A_RKN10(-1+10,7-1 ) =2.01057347685061881846748708777e-2;
    A_RKN10(-1+10,8-1 ) =-2.83507501229335808430366774368e-2;
    A_RKN10(-1+10,9-1 ) =1.48795689185819327555905582479e-2;
    
    A_RKN10(-1+11,1-1 ) = 3.51253765607334415311308293052e-2;
    A_RKN10(-1+11,2-1 ) = 0.0e0;
    A_RKN10(-1+11,3-1 ) =  -8.61574919513847910340576078545e-3;
    A_RKN10(-1+11,4-1 ) = -5.79144805100791652167632252471e-3;
    A_RKN10(-1+11,5-1 ) =  1.94555482378261584239438810411e0;
    A_RKN10(-1+11,6-1 ) =  -3.43512386745651359636787167574e0;
    A_RKN10(-1+11,7-1 ) =   -1.09307011074752217583892572001e-1;
    A_RKN10(-1+11,8-1 ) =   2.3496383118995166394320161088e0;
    A_RKN10(-1+11,9-1 ) =    -7.56009408687022978027190729778e-1;
    A_RKN10(-1+11,10-1 ) =    1.09528972221569264246502018618e-1;
    
    A_RKN10(-1+12,1-1 ) = 2.05277925374824966509720571672e-2;
    A_RKN10(-1+12,2-1 ) =               0.0e0;
    A_RKN10(-1+12,3-1 ) = -7.28644676448017991778247943149e-3;
    A_RKN10(-1+12,4-1 ) =    -2.11535560796184024069259562549e-3;
    A_RKN10(-1+12,5-1 ) =                  9.27580796872352224256768033235e-1;
    A_RKN10(-1+12,6-1 ) =  -1.65228248442573667907302673325e0;
    A_RKN10(-1+12,7-1 ) =                   -2.10795630056865698191914366913e-2;
    A_RKN10(-1+12,8-1 ) =                   1.20653643262078715447708832536e0;
    A_RKN10(-1+12,9-1 ) =                   -4.13714477001066141324662463645e-1;
    A_RKN10(-1+12,10-1 ) =                   9.07987398280965375956795739516e-2;
    A_RKN10(-1+12,11-1 ) =                   5.35555260053398504916870658215e-3;
    
    A_RKN10(-1+13,1-1 ) = -1.43240788755455150458921091632e-1;
    A_RKN10(-1+13,2-1 ) = 0.0e0;
    A_RKN10(-1+13,3-1 ) = 1.25287037730918172778464480231e-2;
    A_RKN10(-1+13,4-1 ) =   6.82601916396982712868112411737e-3;
    A_RKN10(-1+13,5-1 ) =   -4.79955539557438726550216254291e0;
    A_RKN10(-1+13,6-1 ) =   5.69862504395194143379169794156e0;
    A_RKN10(-1+13,7-1 ) =   7.55343036952364522249444028716e-1;
    A_RKN10(-1+13,8-1 ) =   -1.27554878582810837175400796542e-1;
    A_RKN10(-1+13,9-1 ) =   -1.96059260511173843289133255423e0;
    A_RKN10(-1+13,10-1 ) =    9.18560905663526240976234285341e-1;
    A_RKN10(-1+13,11-1 ) =    -2.38800855052844310534827013402e-1;
    A_RKN10(-1+13,12-1 ) =    1.59110813572342155138740170963e-1;
    
    A_RKN10(-1+14,1-1 ) = 8.04501920552048948697230778134e-1;
    A_RKN10(-1+14,2-1 ) = 0.0e0;
    A_RKN10(-1+14,3-1 ) = -1.66585270670112451778516268261e-2;
    A_RKN10(-1+14,4-1 ) = -2.1415834042629734811731437191e-2;
    A_RKN10(-1+14,5-1 ) =  1.68272359289624658702009353564e1;
    A_RKN10(-1+14,6-1 ) =  -1.11728353571760979267882984241e1;
    A_RKN10(-1+14,7-1 ) =     -3.37715929722632374148856475521e0;
    A_RKN10(-1+14,8-1 ) =    -1.52433266553608456461817682939e1;
    A_RKN10(-1+14,9-1 ) =  1.71798357382154165620247684026e1;
    A_RKN10(-1+14,10-1 ) =  -5.43771923982399464535413738556e0;
    A_RKN10(-1+14,11-1 ) =  1.38786716183646557551256778839e0;
    A_RKN10(-1+14,12-1 ) =  -5.92582773265281165347677029181e-1;
    A_RKN10(-1+14,13-1 ) =  2.96038731712973527961592794552e-2;
    
    A_RKN10(-1+15,1-1 ) = -9.13296766697358082096250482648e-1;
    A_RKN10(-1+15,2-1 ) = 0.0e0;
    A_RKN10(-1+15,3-1 ) = 2.41127257578051783924489946102e-3;
    A_RKN10(-1+15,4-1 ) = 1.76581226938617419820698839226e-2;
    A_RKN10(-1+15,5-1 ) = -1.48516497797203838246128557088e1;
    A_RKN10(-1+15,6-1 ) = 2.15897086700457560030782161561e0;
    A_RKN10(-1+15,7-1 ) = 3.99791558311787990115282754337e0;
    A_RKN10(-1+15,8-1 ) = 2.84341518002322318984542514988e1;
    A_RKN10(-1+15,9-1 ) = -2.52593643549415984378843352235e1;
    A_RKN10(-1+15,10-1 ) = 7.7338785423622373655340014114e0;
    A_RKN10(-1+15,11-1 ) = -1.8913028948478674610382580129e0;
    A_RKN10(-1+15,12-1 ) = 1.00148450702247178036685959248e0;
    A_RKN10(-1+15,13-1 ) = 4.64119959910905190510518247052e-3;
    A_RKN10(-1+15,14-1 ) = 1.12187550221489570339750499063e-2;
    
    A_RKN10(-1+16,1-1 ) = -2.75196297205593938206065227039e-1;
    A_RKN10(-1+16,2-1 ) = 0.0e0;
    A_RKN10(-1+16,3-1 ) = 3.66118887791549201342293285553e-2;
    A_RKN10(-1+16,4-1 ) = 9.7895196882315626246509967162e-3;
    A_RKN10(-1+16,5-1 ) = -1.2293062345886210304214726509e1;
    A_RKN10(-1+16,6-1 ) =  1.42072264539379026942929665966e1;
    A_RKN10(-1+16,7-1 ) = 1.58664769067895368322481964272e0;
    A_RKN10(-1+16,8-1 ) = 2.45777353275959454390324346975e0;
    A_RKN10(-1+16,9-1 ) = -8.93519369440327190552259086374e0;
    A_RKN10(-1+16,10-1 ) =  4.37367273161340694839327077512e0;
    A_RKN10(-1+16,11-1 ) =  -1.83471817654494916304344410264e0;
    A_RKN10(-1+16,12-1 ) =  1.15920852890614912078083198373e0;
    A_RKN10(-1+16,13-1 ) =  -1.72902531653839221518003422953e-2;
    A_RKN10(-1+16,14-1 ) =  1.93259779044607666727649875324e-2;
    A_RKN10(-1+16,15-1 ) =  5.20444293755499311184926401526e-3;
    
    A_RKN10(-1+17,1-1 ) = 1.30763918474040575879994562983e0;
    A_RKN10(-1+17,2-1 ) = 0.0e0;
    A_RKN10(-1+17,3-1 ) = 1.73641091897458418670879991296e-2;
    A_RKN10(-1+17,4-1 ) = -1.8544456454265795024362115588e-2;
    A_RKN10(-1+17,5-1 ) = 1.48115220328677268968478356223e1;
    A_RKN10(-1+17,6-1 ) = 9.38317630848247090787922177126e0;
    A_RKN10(-1+17,7-1 ) = -5.2284261999445422541474024553e0;
    A_RKN10(-1+17,8-1 ) = -4.89512805258476508040093482743e1;
    A_RKN10(-1+17,9-1 ) = 3.82970960343379225625583875836e1;
    A_RKN10(-1+17,10-1 ) =  -1.05873813369759797091619037505e1;
    A_RKN10(-1+17,11-1 ) =  2.43323043762262763585119618787e0;
    A_RKN10(-1+17,12-1 ) =  -1.04534060425754442848652456513e0;
    A_RKN10(-1+17,13-1 ) =  7.17732095086725945198184857508e-2;
    A_RKN10(-1+17,14-1 ) =  2.16221097080827826905505320027e-3;
    A_RKN10(-1+17,15-1 ) =  7.00959575960251423699282781988e-3;
    A_RKN10(-1+17,16-1 ) =  0.0e0;
    
}


#endif /* defined(__Project_Voyager__main__) */


