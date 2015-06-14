//
//  main.cpp
//  Project Voyager
//  Created by Zeev Suprun on 2014-06-21.
//

#include <armadillo>
using namespace std;
using namespace arma;
#include "shipSimulating.h"


int main(int argc, char** argv)
{
    ofstream outputFile;
    outputFile.open("output-1yrRK10.txt");
    ifstream planetInput;
    planetInput.open("orb_info.txt");
    
    ifstream shipInput;
    shipInput.open("major_bodies.txt");

    mat Test;
    Test.set_size(3,5);
    
    int numSpaceObjs;
    planetInput >> numSpaceObjs;
    
    //Allocating memory for an array of planet objects.
    planet *spaceObjects;
    spaceObjects = new planet[numSpaceObjs];
    
    //Temporary variables used to read values into the position and velocity matrices
    double xPos, yPos, zPos, xVel, yVel, zVel;
    
    //The following loop initializes every element in the array of planet objects
    for (int i = 0; i < numSpaceObjs; i++) {
        
        //Read data here
        
    }
    planetInput.close();
    
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
     Start of RKN12(10) code
     *************************************************************
     *************************************************************/
    
    //Constant matrices for RKN1012. (Maybe some of these should be arrays?)
    mat c_RKN10(17, 1);
    mat A_RKN10(17, 17);
    mat Aprime_RKN10(17, 17);
    mat Bhat_RKN10(17,1);
    mat Bphat_RKN10(17, 1);
    mat B_RKN10(17, 1);
    mat Bprime_RKN10(17, 1);
    
    //Populate the matrices with a function call. (all but g_RKN10 because that's different for each one)
    populate_RKN10_coeffs (c_RKN10, A_RKN10, Bhat_RKN10, Bphat_RKN10, B_RKN10, Bprime_RKN10);
    
    unsigned int h = 60*60*1/4;		//h = step size, in seconds.
	unsigned int t = 0;		//t is the running time track. Starts at t = 0
	unsigned int t_final = 24*60*60*365;	//Must be an integer!
	mat Acceleration(Velocity.n_rows,Velocity.n_cols);
    
    //ignore the hatless stuff.
    //mat Position_hatless(Velocity.n_rows,Velocity.n_cols);
    //Position_hatless = Position;
	//mat Velocity_hatless(Velocity.n_rows,Velocity.n_cols);
    //Velocity_hatless= Velocity;
    
	Acceleration.zeros();
    
    //g_RKN10 needs to be an array of 17 matrices, and each matrix is Nx3
    mat* g_RKN10;
    g_RKN10 = new mat [17];
    for (int i = 0; i < 17; i++) {
        g_RKN10[i].set_size(numSpaceObjs, 3);
        g_RKN10[i].zeros();
    }
    //sums
    mat sum_gbHat(numSpaceObjs, 3);
    mat sum_gbpHat(numSpaceObjs, 3);
    //hatless
    mat sum_gb(numSpaceObjs, 3);
    mat sum_gbPrime(numSpaceObjs, 3);
    
    //holds the sum of a[i][j]*g[j] from j = 0 to j = i-1.
    mat sum_ag(numSpaceObjs, 3);
    
    mat phi_hat(Velocity.n_rows,Velocity.n_cols);
    mat phi_phat(Velocity.n_rows,Velocity.n_cols);
    //hatless
    //mat phi(Velocity.n_rows,Velocity.n_cols);
    //mat phiPrime(Velocity.n_rows,Velocity.n_cols);
    
    while (t <= t_final)
    {
        //This writes the planet data to the output file, but only every 24 hours, to avoid huge text files.
        if (t % (60*60*24) == 0) {
            writeData(spaceObjects, numSpaceObjs, Velocity, Position, t, outputFile);
        }
        //cout << "New iteration. t = " << t << "\n";
        
        //zero the sums every timestep.
        sum_gbHat.zeros();
        sum_gbpHat.zeros();
        sum_gb.zeros();
        sum_gbPrime.zeros();
        
        
        //Start of fancy math here.
        
        //Calculating g array.
        for (int i = 0; i < 17; i++) {
            sum_ag.zeros();
            //cout << ("sum_ag has been zeroed.\n");
            for(int j = 0; j < i - 1; j++) {
                sum_ag += A_RKN10(i, j)*g_RKN10[j];
            }
            
            //g_RKN10 = f(t + c[i]*h, Position(t) + c[i]*h*Velocity(t) + h^2*Sum_ag)
            //We're just ignoring the part with 't', because our acceleration function doesn't include t.
            g_RKN10[i] = grav_accel(Position + c_RKN10[i]*h*Velocity + h*h*sum_ag,Mass);
            
        }
        //cout << ("g array created.\n");
        
        //Creating sum_gb
        for (int i = 0; i < 17; i++) {
            sum_gbHat += Bhat_RKN10(i,0)*g_RKN10[i];
            sum_gbpHat += Bphat_RKN10(i,0)*g_RKN10[i];
            //hatless sums
            //sum_gb += B_RKN10(i,0)*g_RKN10[i];
            //sum_gbPrime += Bprime_RKN10(i,0)*g_RKN10[i];
        }
        //cout << ("sum matrices created.\n");
        //Matrix math.
        phi_hat = Velocity + h*sum_gbHat;
        phi_phat = sum_gbpHat;
        
        //phi = Velocity + h*sum_gbHat;
        //phiPrime = sum_gbPrime;
        
        // Update the position and velocity using the previously calculated weighted sums of k's and l's
        Position = Position + h*phi_hat;
        Velocity = Velocity + h*phi_phat;
        
        //Position_hatless = Position + h*phi;
        //Velocity_hatless = Velocity + h*phiPrime;
        
        // Step forward in time
        t += h;
    }
    
    /*************************************************************
     *************************************************************
     End of RK12(10) Code
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

