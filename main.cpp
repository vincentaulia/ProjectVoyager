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
    outputFile.open("output-1yrRK10.txt");
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
            Start of Euler Approximation placeholder code
     *************************************************************
     *************************************************************/
    /*The following is a first order Euler approximation of the motion of the bodies
     This was taken from Zach's example code. This should be eventually replaced with
     a Runge-Kutta 10th order approximation.*/
    
    /*
	unsigned int h = 60*60*24;		//h = step size, in seconds. Right now it's about 15 minutes.
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
        //Acceleration += rad_pressure_accel(Position, Mass, Radius);

		Velocity = Velocity + h*Acceleration;
		Position = Position + h*Velocity;
		t = t + h;
	}
     */
    /*************************************************************
     *************************************************************
                    End of Euler placeholder code
     *************************************************************
     *************************************************************/

    
    /*************************************************************
     *************************************************************
Start of RK10 code (note: Compiles, but does not produce accurate results)
     *************************************************************
     *************************************************************/

    // Algorithm for numerical integration of the 2nd order Acceleration ODE to find Position and Velocity
    // Based on "A Runge-Kutta Method of Order 10" by E. Hairer
    // Available: http://imamat.oxfordjournals.org/content/21/1/47.full.pdf (Accessed: 19 Oct. 2014)
    // Implemented by Behrad Vatankhahghadim
    // 19 October 2014
    
    
   
    // Set the number of stages to 17
    int s = 17;
    //declare variables to store summartions.
    //mat Sum_AL, Sum_AK, Sum_BL, Sum_BK;
    mat Sum_AL(Velocity.n_rows, Velocity.n_cols);
    mat Sum_AK(Velocity.n_rows, Velocity.n_cols);
    mat Sum_BL(Velocity.n_rows, Velocity.n_cols);
    mat Sum_BK(Velocity.n_rows, Velocity.n_cols);
    
    //declare the k and l arrays. k and l are both arrays of matrices.
    mat *k;
    mat *l;
    k = new mat [s];
    l = new mat [s];
    
    //Initialize the matrix in each step of the array to be a 0 matrix
    //Note: The pseudocode that the physics team provided doesn't say what to initialize these arrays to.
    for (int counter = 0; counter <= s; counter ++) {
        k[counter] = mat(Velocity.n_rows, Velocity.n_cols, fill::zeros);
        l[counter] = mat(Velocity.n_rows, Velocity.n_cols, fill::zeros);
        
    }
    
    //declare and initialize the a_RK10 and b_RK10 arrays of coefficients.
    double a_RK10[17][17];
    double b_RK10[17];
    populate_RK10_coeffs (a_RK10, b_RK10);
    
    unsigned int h = 60*60*24;		//h = step size, in seconds. Right now it's about 15 minutes.
	unsigned int t = 0;		//t is the running time track. Starts at t = 0
	unsigned int t_final = 24*60*60*365*1;	//Must be an integer!
	mat Acceleration(Velocity.n_rows,Velocity.n_cols);
	Acceleration.zeros();

    /*
     The following are needed before entering the loop:
     - Set the Position and Velocity to their initial conditions
     - Initialize the k and l arrays to 1D arrays of 17 elements (number of stages = s = 17)
     - Assign the coefficients from the source above to a, b, c arrays (also available in JPG on Drive)
     (a is 2D, while b and c are 1D)
     */
    
    while (t <= t_final)
    {
        
        // Start of RK10 approximation
        
        // Reset all summations to zero
        Sum_AL.zeros();
        Sum_AK.zeros();
        Sum_BL.zeros();
        Sum_BK.zeros();
        

        //Start of fancy math here.
        // Loop through all stages
        for (int i = 1; i <= s; i++)
        {
            // Accumulate the a_ij*l_j and a_ij*k_j summations
            for (int j = 1; j < i; j++)
            {
                Sum_AL += (a_RK10[i][j] * l[j - 1]);
                Sum_AK += (a_RK10[i][j] * k[j - 1]);
            }
            
            // Calculate the intermediate k's and l's for each stage
            k[i - 1] = Velocity + h * Sum_AL;
            l[i - 1] = grav_accel (Position + h * Sum_AK , Mass);
            
            // Accumulate the b_i*l_i and b_i*k_i summations for later use
            Sum_BL = Sum_BL + (b_RK10[i - 1] * l[i - 1]);
            Sum_BK = Sum_BK + (b_RK10[i - 1] * k[i - 1]);
        }
        
        //This writes the planet data to the output file, but only every 24 hours, to avoid huge text files.
        if (t % (60*60*24) == 0) {
            writeData(spaceObjects, numSpaceObjs, Velocity, Position, t, outputFile);
        }
        
        
        // Update the position and velocity using the previously calculated weighted sums of k's and l's
        Position = Position + 1 * Sum_BK;
        Velocity = Velocity + 1 * Sum_BL;
        
        // Step forward in time
        t = t + h;
    }
    
    
    
    /*************************************************************
     *************************************************************
                    End of RK10 Code
     *************************************************************
     *************************************************************/

    /*************************************************************
     *************************************************************
                    Start of RK4 Placeholder code
     *************************************************************
     *************************************************************/
    
    /*
     This is the RK4 code. This works, but it turns out that the RK4 approximation
     is less accurate than the Euler approximation for our purposes.
     I'm keeping this code here because it's an easier to understand version of the RK10/RK8 stuff,
     so it might be helpful if you're trying to figure out RK10/RK8 stuff. */
    /*
    
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
                    End of RK4 placeholder code
     *************************************************************
     *************************************************************/
    
    //Cleaining up
    delete [] spaceObjects;
    outputFile.close();
    return 0;
}

