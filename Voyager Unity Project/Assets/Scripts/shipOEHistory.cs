using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics; //For testing purposes only
using Debug = UnityEngine.Debug; //HUH

/* A class for storing the Orbital Elements of a ship and a time at which the Orbital
Elements change. The class also includes functions for calculating new OE based deltaV.*/


//NOTE: In the deltaV functions I added some changes as described below in the comments

public class shipOEHistory : MonoBehaviour
{
    public List<Elements> shipOE = new List<Elements>();
    public List<long> shipT = new List<long>();       //the global time where an orbit strats
    public List<double> shipA = new List<double>();   //the end mean anomaly of an orbit
    public GameObject shipObject;
    private int currentTimePos;
    private Vector3 def;
    private bool deltaVGui = false;
    private int j;
    private double mAnom;
    private string a1 = "0";
    private string a2 = "0";
    private string a3 = "0";
    public Stopwatch stopwatch = new Stopwatch(); //for testing purposes only
    public bool updateOrbit = false;
    public bool debug = false;

    /* Constructor, takes the first OE of a ship, the GamObject ship and the time at which it is created
    NOTE: ship GameObject is only needed for PcaPosition.findPos */

    //The first three argumets are for Orbital Elemnts script which is copied down below

    public void shipOEHistoryConstructor(Elements el, long time, GameObject shipObj)
    {
        //stopwatch.Start ();
        shipOE.Add(el);
        shipT.Add(time);
        shipA.Add(0);
        shipObject = shipObj;
        currentTimePos = 0;
        //stopwatch.Stop(); // REMOVE LATER
        //Debug.Log (stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
    }

    public string currentSphereOfInfluence() // 150 milisecond speed :/
    {
        //stopwatch.Start (); //REMOVE LATER
        Elements current = currentOE(Global.time);   //SHIP ELEMENTS
        Vector3 currentPos = findShipPos(Global.time);    //SHIP POSITION 

        GameObject parentPlanet = GameObject.Find(current.IDFocus);  //GAME OBJECT CYRRENT FOCCUS
        string IDF = null;
        IDF = focusDown(currentPos, parentPlanet);
        //			Debug.Log ("IDF1");
        //			Debug.Log (IDF);
        if (IDF != null)
        {
            //stopwatch.Stop(); // REMOVE LATER
            //Debug.Log (stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
            return IDF;
        }
        IDF = focusUp(currentPos, parentPlanet);
        //			Debug.Log ("IDF2");
        //			Debug.Log (IDF);
        //stopwatch.Stop(); //REMOVE LATER
        //Debug.Log (stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
        return IDF;
    }

    public string focusUp(Vector3 shipP, GameObject GO)
    {
        GameObject parentPlanet = GameObject.Find(GO.GetComponent<OrbitalElements>().orb_elements.IDFocus); //probably good
        string IDF = focusDown(shipP, parentPlanet);
        if (IDF == null)
            return focusUp(shipP - PcaPosition.findPos(parentPlanet.GetComponent<OrbitalElements>().orb_elements, Global.time, GO), parentPlanet);
        else
            return IDF;
    }

    public string focusDown(Vector3 shipP, GameObject GO)
    {
        int children = GO.transform.childCount;
        int numberRender = 0;
        bool flag = false;
        for (int i = 0; i < children; ++i)
        {
            string currentTag = GO.transform.GetChild(i).gameObject.tag;
            if (currentTag != "Rendering" && currentTag != "name" && currentTag != "DistantPlanetIcon")
            {
                double radiusChild = GO.transform.GetChild(i).gameObject.GetComponent<OrbitalElements>().orb_elements.soi;
                Vector3 childPosition = PcaPosition.findPos(GO.GetComponent<OrbitalElements>().orb_elements, Global.time, GO.transform.GetChild(i).gameObject);
                Vector3 shipRef = shipP - childPosition;
                double difference = (double)(shipRef).magnitude;
                difference = difference * Global.scale * 1000;

                if (difference <= radiusChild)
                {
                    flag = true;
                    return focusDown(shipRef, GO.transform.GetChild(i).gameObject);
                }
            }
            else
                numberRender++;
        }
        if (!flag)
        {
            double radiusParent = GO.GetComponent<OrbitalElements>().orb_elements.soi;
            //Vector3 parentPosition = PcaPosition.findPos (GO.GetComponent<OrbitalElements> ().orb_elements, Global.time, GO);
            double difference = (double)(shipP).magnitude;
            difference = difference * Global.scale * 1000;
            if (difference <= radiusParent)
            {
                return GO.GetComponent<OrbitalElements>().orb_elements.name;
            }
        }
        return null;
    }

    public void deltaVAdd(double mAnom)
    {
        j = deltavChange(mAnom);
        this.mAnom = mAnom;
        deltaVGui = true;
    }

    //		public int deltavChange (double mAnom)
    //		{
    //				Elements Initial = currentOE (Global.time);
    //				int i = indexFinder (Global.time);
    //				double period = 2 * Math.PI * Math.Sqrt (Initial.axis / 6.67384e-11 * (Initial.mass + Initial.massFocus););
    //				double currMAnom = ((Global.time - shipT[i]) * Initial.n ) - shipA[i];
    //				long timeInOrbit = (long)(mAnom/Initial.n);  //LOSS OF PRECISSION
    //				Debug.Log ("Time in Orbit");
    //				Debug.Log (timeInOrbit);
    //				shipA [i] = mAnom;
    //				if (i == 0)
    //					AddNewOE (Initial, timeInOrbit);
    //				else
    //					AddNewOE (Initial, (shipT[i] + timeInOrbit) );
    //				return i;
    //		}

    public int deltavChange(double mAnom)
    {
        //Elements Initial = currentOE (Global.time);
        //int i = indexFinder (Global.time);

        //Get the latest OE and time instead of using the current one
        Elements Initial = shipOE[shipOE.Count - 1];
        int i = shipOE.Count - 1;
        Debug.Log("DeltaVChange");
        Debug.Log(i);
        Debug.Log(shipT[i]);
        Debug.Log(Initial.n);
        Debug.Log(mAnom);

        //double period = 2 * Math.PI * Math.Sqrt (Initial.axis / 6.67384e-11 * (Initial.mass + Initial.massFocus));
        //double currMAnom = ((Global.time - shipT[i]) * Initial.n ) - shipA[i];

        long timeInOrbit = (long)((mAnom - Initial.anom) / Initial.n);  //LOSS OF PRECISSION, Change need anom where orbit started
        //	if (i == 1)
        //		timeInOrbit += 3000;
        Debug.Log("Time in Orbit");
        Debug.Log(timeInOrbit);
        shipA[i] = mAnom;
        AddNewOE(Initial, (shipT[i] + timeInOrbit));
        return i;
    }
    public void deltaVInput(double normal, double tangent, double radial)
    {
        Elements prev = shipOE[j + 1];
        CalcNormalDeltaV(ref prev, normal, mAnom);
        CalcTangentialDeltaV(ref prev, tangent, mAnom);
        CalcRadialDeltaV(ref prev, radial, mAnom);
        GetComponent<ShipMissionFunctions>().update_deltaV_budget(normal + tangent + radial);

        double bigE = 1;
        double new_bigE;
        for (int i = 0; i < 50; i++)
        {
            new_bigE = bigE - ((bigE - prev.ecc * Math.Sin(bigE) - mAnom) / (1 - prev.ecc * Math.Cos(bigE)));
            bigE = new_bigE;
        }
        Debug.Log("Approximation");
        Debug.Log(mAnom);
        Debug.Log(prev.ecc);
        Debug.Log(bigE);
        double[] velocityVector = GetComponent<ShipMissionFunctions>().calc_current_velocity_double(bigE);
        double[] positionVector = findShipPosDouble(shipT[j + 1] - 1);

        Debug.Log("Time");
        Debug.Log(shipT[j + 1]);
        positionVector[0] = positionVector[0] * Global.scale * 1000;
        //Debug.Log (positionVector[0]);
        positionVector[1] = positionVector[1] * Global.scale * 1000;
        positionVector[2] = positionVector[2] * Global.scale * 1000;
        Debug.Log("PositionVectors");
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("v: " + velocityVector[i]);
        }
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("p: " + positionVector[i]);
        }
        Debug.Log("dotted");
        double dot = dotProduct(velocityVector, positionVector);
        Debug.Log(dot);
        dot = dot / magnitude(velocityVector) / magnitude(positionVector);
        Debug.Log(dot);

        double angle = Math.Acos(dot) - (Math.PI / 2);  //Real Flight Angle 
        Debug.Log("angle: " + angle);

        Debug.Log("a: " + prev.axis);
        Debug.Log("e: " + prev.ecc);
        Debug.Log("r: " + magnitude(positionVector));
        double inter = (((((prev.axis * (1 - Math.Pow(prev.ecc, 2))) / (magnitude(positionVector))) - 1) / prev.ecc));
        //Debug.Log("old inter: " + inter);
        //inter = (prev.axis*(1 - prev.ecc*prev.ecc) / magnitude(positionVector) - 1) / prev.ecc;
        Debug.Log("inter: " + inter);
        if (inter > 1.0)
        {
            inter = 1.0;
            Debug.Log("Warning!!! value greater then 1");
        }
        if (inter < -1.0)
        {
            inter = -1.0;
            Debug.Log("Warning!!! value less then -1");
        }
        double solution1 = Math.Acos(inter);
        double solution2 = Math.PI * 2 - solution1;
        //Debug.Log (((((prev.axis * (1 - Math.Pow(prev.ecc,2))) / (magnitude(positionVector) * Global.scale *1000)) - 1) / prev.ecc));
        //Debug.Log ((((((prev.axis * (1 - Math.Pow(prev.ecc,2))) / (magnitude(positionVector) * Global.scale *1000)) - 1) / prev.ecc)));
        Debug.Log("Sollutions");
        Debug.Log(solution1);
        Debug.Log(solution2);
        Debug.Log(prev.ecc);
        Debug.Log(prev.axis);
        //Debug.Log (positionVector.magnitude* Global.scale *1000);
        double flightAngle1 = Math.Atan(((prev.ecc * Math.Sin(solution1)) / (1 + (prev.ecc * Math.Cos(solution1)))));
        double flightAngle2 = Math.Atan(((prev.ecc * Math.Sin(solution2)) / (1 + (prev.ecc * Math.Cos(solution2)))));
        Debug.Log("flightAngles");
        Debug.Log(flightAngle1);
        Debug.Log(flightAngle2);
        Debug.Log("Angle Diff");
        Debug.Log(flightAngle1 - angle);
        Debug.Log(flightAngle2 - angle);
        double trueValue = (Math.Abs(flightAngle1 - angle) > Math.Abs(flightAngle2 - angle)) ? solution2 : solution1;
        double eccentricAnom = Math.Acos(((prev.ecc + Math.Cos(trueValue)) / (1 + prev.ecc * Math.Cos(trueValue))));
        double initialMeanAnom = (eccentricAnom - prev.ecc * Math.Sin(eccentricAnom));


        prev.anom = initialMeanAnom;
        Debug.Log("Initial Mean Anom");
        Debug.Log(initialMeanAnom);
        shipOE[j + 1] = prev;
    }

    public double dotProduct(double[] V1, double[] V2)
    {
        return ((V1[0] * V2[0]) + (V1[1] * V2[1]) + (V1[2] * V2[2]));
    }


    public double magnitude(double[] V)
    {
        return (Math.Sqrt(Math.Pow(V[0], 2) + Math.Pow(V[1], 2) + Math.Pow(V[2], 2)));
    }

    public int indexFinder(long time)
    {
        if (currentTimePos >= 1 && (currentTimePos + 1) < shipT.Count)
        {
            if (time > shipT[currentTimePos - 1] && time < shipT[currentTimePos + 1])
                return currentTimePos;
            else
            {
                int tpf = timePosFind(time);
                currentTimePos = tpf;
                //stopwatch.Stop(); //remove later
                //Debug.Log (stopwatch.Elapsed.TotalMilliseconds);
                return tpf;
            }
        }
        else
        {
            int tpf = timePosFind(time);
            currentTimePos = tpf;
            //stopwatch.Stop(); //remove later
            //Debug.Log (stopwatch.Elapsed.TotalMilliseconds);
            return tpf;
        }
    }

    public Elements currentOE(long time)  //EXECUTION TIME -- 0-1 miliseconds :)
    {
        return shipOE[indexFinder(time)];
    }


    public void AddNewOE(Elements toAdd, long time) //THIS NEEDS TO DELETE NEXT ITEMS IF OE IS NOT APPENDED TO END!!!
    {
        int i = indexFinder(time);
        shipT.Insert(i + 1, time);
        shipOE.Insert(i + 1, toAdd);
        shipA.Insert(i + 1, 0);
    }

    public void removeOEpos(int pos)
    {
        shipT.RemoveAt(pos);
        shipOE.RemoveAt(pos);
        shipA.RemoveAt(pos);
    }

    public void removeOEtime(long time)
    {
        int pos = timePosFind(time);
        removeOEpos(pos);
    }

    public double[] findShipPosDouble(long time)
    {
        return PcaPosition.findPosDouble(currentOE(time), (time - shipT[indexFinder(time)]), shipObject);
    }

    public Vector3 findShipPos(long time)
    {
        Elements curr = currentOE(time);
        //Debug.Log ("Time Passed to Find Pos");
        //Debug.Log ((long)(time - shipT[indexFinder (time)] + (curr.anom/curr.n)));
        //Debug.Log("buffer");
        if (debug)
        {
            Debug.Log("find ship pos");
            Debug.Log("time: " + time);
            Debug.Log("curr.anom: " + curr.anom);
            Debug.Log("curr.n: " + curr.n);
            Debug.Log("buffer: " + curr.anom / curr.n);
            debug = false;
        }
        return PcaPosition.findPos(curr, (long)(time - shipT[indexFinder(time)] + (curr.anom / curr.n)), shipObject);
        //return PcaPosition.findPos(curr, (long)(time - shipT[indexFinder(time)]), shipObject);//
    }

    //overLoad method for findShipPos to manually specify a certain orbit
    public Vector3 findShipPos(long time, int i)
    {
        Elements curr = shipOE[i];
        return PcaPosition.findPos(curr, (long)(time - shipT[i] + (curr.anom / curr.n)), shipObject);
    }

    private int timePosFind(long time)
    {
        int start = 0;
        int end = shipT.Count;
        while (true)
        {
            int mid = (start + end) / 2;
            if (shipT[mid] == time)
                return mid;
            else if ((end - start) == 1)
                return start;
            else if (shipT[mid] > time)
                end = mid;
            else if (shipT[mid] < time)
                start = mid;
        }
    }


    //return the number of Element structs in the list array
    public int getNumberOfElements()
    {
        return shipOE.Count;
    }

    //return the start time for the selected Elements struct
    public long startTimeOE(Elements el)
    {
        for (int i = 0; i < shipOE.Count; i++)
        {
            if (shipOE[i] == el)
            {
                return shipT[i];
            }
        }
        return -1;
    }

    //this functions adds new orbital elements.. given time, and the new semi-major axis
    public void dummyNode(long time, double newAxis)
    {
        Elements Initial = currentOE(Global.time);
        changeDummyAxis(ref Initial, newAxis);
        AddNewOE(Initial, time);
    }

    //changes the semi-major axis
    void changeDummyAxis(ref Elements Initial, double axis)
    {
        Initial.axis = axis;
        Initial.calcData();
    }




    /*	This function takes the initial orbital elements of the 
        ship and returns the final orbital elements based on the
        delta-V applied in the normal/anti-normal direction.
        DeltaV varible should be in Perifocal Coordinates.
    */
    void CalcNormalDeltaV(ref Elements Initial, double DeltaV, double mAnom)
    {

        // Approximation of the true anomaly as a sine series of the mean anomaly
        double TrueAnomaly;
        TrueAnomaly = mAnom;
        TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Math.Pow(Initial.ecc, 3))) * Math.Sin(mAnom);
        TrueAnomaly += (1.25 * Math.Pow(Initial.ecc, 2)) * Math.Sin(2 * mAnom);
        TrueAnomaly += ((13 / 12) * Math.Pow(Initial.ecc, 3)) * Math.Sin(3 * mAnom);

        /* Calculate the final longitude (right ascension) of the ascending node */

        // Calculate the magnitude of the initial velocity //

        // Calculate the standard gravitational parameter
        double Mu;
        Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);

        // Calculate the specific relative angular momentum
        double h;
        h = Math.Sqrt(Math.Sqrt(1 - Math.Pow(Initial.ecc, 2)) * Initial.axis * Mu);

        // Calculate the initial velocity in Perifocal Coordinates
        Vector3 InitialVelocity;
        double IVp = -1 * Mu * Math.Sin(TrueAnomaly) / h;
        double IVq = (Initial.ecc + Math.Cos(TrueAnomaly)) * Mu / h;
        double IVw = 0;
        InitialVelocity = new Vector3((float)IVp, (float)IVq, (float)IVw);

        double VelocityMag = InitialVelocity.magnitude;

        // Calculate the angle between the initial and the final orbits
        double AngleBtwnOrbits;
        AngleBtwnOrbits = 2 * Math.Asin(DeltaV / (2 * VelocityMag));

        // Calculate the argument of latitude
        double ArgOfLatitude;
        ArgOfLatitude = TrueAnomaly + Initial.arg;

        //calculate the final inclination
        double FinalIncl = Math.Acos(Math.Cos(Initial.incl) * Math.Cos(AngleBtwnOrbits) - Math.Sin(Initial.incl) * Math.Sin(AngleBtwnOrbits) * Math.Cos(ArgOfLatitude));

        // Calculate the difference between the intial and the final longitudes of ascending node
        double DeltaOmega = 0;

        if (Initial.incl == 0)
        {
            DeltaOmega = mAnom;
        }
        else
        {
            DeltaOmega = Math.Acos((Math.Cos(AngleBtwnOrbits) - Math.Cos(Initial.incl) * Math.Cos(FinalIncl)) / Math.Sin(Initial.incl) / Math.Sin(FinalIncl));
        }

        double FinalAsc = DeltaOmega + Initial.asc;

        // Because the maneuver is only in the normal/anti-normal direction,
        // only the Inclination and Longitude of Ascending Node change
        Initial.incl = FinalIncl;
        if (Initial.incl + AngleBtwnOrbits < 0)
        {
            Initial.incl *= -1;
        }
        Initial.asc = FinalAsc;

        // Calculate the final P, Q and n orbital elements after the maneuver
        Initial.calcData();

    }

    /*	This function takes the initial orbital elements of the 
            ship and returns the final orbital elements based on the
            delta-V applied in the tangential/anti-tangential direction.
            DeltaV varible should be in Perifocal Coordinates.
        */
    void CalcTangentialDeltaV(ref Elements Initial, double DeltaV, double mAnom)
    {

        // Approximation of the true anomaly as a sine series of the mean anomaly
        double TrueAnomaly;
        TrueAnomaly = mAnom;
        TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Math.Pow(Initial.ecc, 3))) * Math.Sin(mAnom);
        TrueAnomaly += (1.25 * Math.Pow(Initial.ecc, 2)) * Math.Sin(2 * mAnom);
        TrueAnomaly += ((13 / 12) * Math.Pow(Initial.ecc, 3)) * Math.Sin(3 * mAnom);
        Debug.Log("True anomaly: " + TrueAnomaly);
        //Debug.Log (TrueAnomaly); // check this.

        // Calculate the standard gravitational parameter
        double Mu;
        Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
        Debug.Log("Mu: " + Mu);
        // Calculate the specific relative angular momentum
        double h;
        h = Math.Sqrt((1 - Math.Pow(Initial.ecc, 2)) * Initial.axis * Mu);
        Debug.Log("ecc: " + Initial.ecc);
        Debug.Log("axis: " + Initial.axis);
        Debug.Log("h: " + h);
        // Calculate the initial velocity in Perifocal Coordinates
        Vector3 InitialVelocity;
        double IVp = -1 * Mu * Math.Sin(TrueAnomaly) / h;
        double IVq = (Initial.ecc + Math.Cos(TrueAnomaly)) * Mu / h;
        double IVw = 0;
        InitialVelocity = new Vector3((float)IVp, (float)IVq, (float)IVw);

        // Calculate the magnitude of the initial velocity
        double VelocityMag = InitialVelocity.magnitude;
        Debug.Log("p: " + IVp);
        Debug.Log("q: " + IVq);
        Debug.Log("InitialVelocity: " + VelocityMag);
        // Calculate the orbital distance from object to primary
        double r;
        r = Math.Pow(h, 2) / Mu / (1 + (Initial.ecc * Math.Cos(TrueAnomaly)));
        Debug.Log("r: " + r);

        // Calculate the new specific orbital energy
        double EnergNew;
        double newVel = VelocityMag + DeltaV;
        EnergNew = ((Math.Pow(newVel, 2)) / 2) - (Mu / r);
        Debug.Log("Energynew: " + EnergNew);

        /* Calculate the new semi-major axis */
        Initial.axis = -1 * Mu / 2 / EnergNew;
        Debug.Log("axis: " + Initial.axis);

        // Calculate the position of the object in Perifocal Coordinates
        Vector3 rVec;
        double rVp = r * Math.Cos(TrueAnomaly);
        double rVq = r * Math.Sin(TrueAnomaly);
        double rVw = 0;
        rVec = new Vector3((float)rVp, (float)rVq, (float)rVw);
        double newR = rVec.magnitude; //only need the scalar value of the distance

        // Calculate the magnitude of the new angular velocity vector
        double hNew;
        hNew = newR * newVel;

        /***** Calculate the new eccentricity *****/
        Initial.ecc = Math.Sqrt(1 + (2 * (Math.Pow(hNew, 2) * EnergNew)) / Math.Pow(Mu, 2));

        /***** Calculate the final P, Q, W and n orbital elements after the maneuver *****/
        Initial.calcData();
    }
    //	/*	This function takes the initial orbital elements of the 
    //		ship and returns the final orbital elements based on the
    //		delta-V applied in the radial/anti-radial direction.
    //		DeltaV varible should be in Perifocal Coordinates.
    //	*/
    void CalcRadialDeltaV(ref Elements Initial, double DeltaV, double mAnom)
    {

        // Approximation of the true anomaly as a sine series of the mean anomaly
        double TrueAnomaly;
        TrueAnomaly = mAnom;
        TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Math.Pow(Initial.ecc, 3))) * Math.Sin(mAnom);
        TrueAnomaly += (1.25 * Math.Pow(Initial.ecc, 2)) * Math.Sin(2 * mAnom);
        TrueAnomaly += ((13 / 12) * Math.Pow(Initial.ecc, 3)) * Math.Sin(3 * mAnom);

        // Calculate the standard gravitational parameter
        double Mu;
        Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);

        // Calculate the specific relative angular momentum
        double h;
        h = Math.Sqrt((1 - Math.Pow(Initial.ecc, 2)) * Initial.axis * Mu);

        // Calculate the initial velocity in Perifocal Coordinates
        Vector3 InitialVelocity;
        double IVp = -1 * Mu * Math.Sin(TrueAnomaly) / h;
        double IVq = (Initial.ecc + Math.Cos(TrueAnomaly)) * Mu / h;
        double IVw = 0;
        InitialVelocity = new Vector3((float)IVp, (float)IVq, (float)IVw);

        // Calculate the magnitude of the initial velocity
        double VelocityMag = InitialVelocity.magnitude;

        // Calculate the orbital distance from object to primary
        double r;
        r = Math.Pow(h, 2) / Mu / (1 + (Initial.ecc * Math.Cos(TrueAnomaly)));

        // Calculate the new specific orbital energy
        // Velocity vectors are at a right angle; use pythagorean theorem
        double EnergNew;
        double newVel = Math.Sqrt(Math.Pow(VelocityMag, 2) + Math.Pow(DeltaV, 2));
        EnergNew = ((Math.Pow(newVel, 2)) / 2) - (Mu / r);

        /***** Calculate the new semi-major axis *****/
        Initial.axis = -1 * Mu / 2 / EnergNew;

        // Specific relative angular momentum does not change in a radial burn.

        /***** Calculate the new eccentricity *****/
        Initial.ecc = Math.Sqrt(1 + (2 * (Math.Pow(h, 2) * EnergNew)) / Math.Pow(Mu, 2));

        /***** Calculate the final P, Q, W and n orbital elements after the maneuver *****/
        Initial.calcData();
    }

    void OnGUI()
    {
        //GUI.skin = MenuSkin;

        if (deltaVGui)
        {
            //Debug.Log ("OE COUNT");
            //Debug.Log (shipT[1]);
            //Debug.Log(findShipPos (Global.time));
            GUI.BeginGroup(new Rect(Screen.width - 130, Screen.height - 310, 120, 270));
            GUI.Box(new Rect(0, 0, 120, 300), "Maneuver Node");
            GUI.Label(new Rect(10, 30, 100, 20), "Normal");
            a1 = GUI.TextField(new Rect(10, 60, 80, 20), a1, 8);
            a1 = Regex.Replace(a1, "[?+-][^.0-9]", "");
            GUI.Label(new Rect(10, 90, 100, 20), "Tangential");
            a2 = GUI.TextField(new Rect(10, 120, 80, 20), a2, 8);
            a2 = Regex.Replace(a2, "[?+-][^.0-9]", "");
            GUI.Label(new Rect(10, 150, 100, 20), "Radial");
            a3 = GUI.TextField(new Rect(10, 180, 80, 20), a3, 8);
            a3 = Regex.Replace(a3, "[?+-][^.0-9]", "");

            if (GUI.Button(new Rect(10, 210, 100, 20), "Change DV"))
            {

                //ONLY LEAVE ONE OF THE METHODS UNCOMMENTED

                //1] first method to calculate deltaV
                //this.gameObject.GetComponent<shipOEHistory> ().deltaVInput(double.Parse (a1), double.Parse (a2), double.Parse (a3)) ;

                //2] second method to calculate deltaV
                deltaVInputNew(double.Parse(a1), double.Parse(a2), double.Parse(a3));

                updateOrbit = true;
                deltaVGui = false;
                Debug.Log("Time of orbits");
                for (int i = 0; i < shipT.Count; i++)
                {
                    Debug.Log(shipT[i]);
                }

                //show the DV button again
                this.GetComponent<ShipNodeGui>().showButton = true;
            }
            //this button should be added when we can test Change DV before applying it
            //if (GUI.Button(new Rect(10, 240, 100, 20),"Done"))
            //{
            //flag to update visualizing the tracks
            //updateOrbit = true;
            //deltaVGui = false; 
            //}

            //canel adding the node and delete the inserted parameters
            if (GUI.Button(new Rect(10, 240, 100, 20), "Cancel"))
            {
                deltaVGui = false;
                removeOEpos(shipA.Count - 1);

                //show the DV button again
                this.GetComponent<ShipNodeGui>().showButton = true;
            }
            GUI.EndGroup();
        }
    }

    public void deltaVInputNew(double normal, double tangent, double radial)
    {
        //get the orbital elements
        Elements el = shipOE[j + 1];

        //find r1 and r2
        //use the old orbit for both positions
        Vector3 temp_r1 = findShipPos(shipT[j], j);
        Vector3 temp_r2 = findShipPos(shipT[j + 1], j);

        //convert to vector3d for more accuracy
        Vector3d r1, r2;
        r1.x = temp_r1.x;
        r1.y = temp_r1.z;
        r1.z = temp_r1.y;

        r2.x = temp_r2.x;
        r2.y = temp_r2.z;
        r2.z = temp_r2.y;

        //scale it for units of m
        r1 *= Global.scale * 1000;
        r2 *= Global.scale * 1000;

        Debug.Log("r1: " + r1);
        Debug.Log("mag: " + r1.magnitude);
        Debug.Log("r2: " + r2);
        Debug.Log("mag: " + r2.magnitude);

        // Approximation of the true anomaly as a sine series of the mean anomaly
        double v1, v2, v;
        //true anomaly of r1
        v1 = el.anom;
        v1 += ((2 * el.ecc) - (0.25 * Math.Pow(el.ecc, 3))) * Math.Sin(el.anom);
        v1 += (1.25 * Math.Pow(el.ecc, 2)) * Math.Sin(2 * el.anom);
        v1 += ((13 / 12) * Math.Pow(el.ecc, 3)) * Math.Sin(3 * el.anom);

        //true anomaly of r2
        v2 = mAnom;
        v2 += ((2 * el.ecc) - (0.25 * Math.Pow(el.ecc, 3))) * Math.Sin(mAnom);
        v2 += (1.25 * Math.Pow(el.ecc, 2)) * Math.Sin(2 * mAnom);
        v2 += ((13 / 12) * Math.Pow(el.ecc, 3)) * Math.Sin(3 * mAnom);

        //delta true anomaly
        v = v2 - v1;

        Debug.Log("v1: " + v1);
        Debug.Log("v2: " + v2);
        Debug.Log("v: " + v);

        //calculating the eccentric anomaly, from the true anomaly
        double E1, E2, E;
        E1 = Math.Acos((el.ecc + Math.Cos(v1)) / (1 + el.ecc * Math.Cos(v1)));
        E2 = Math.Acos((el.ecc + Math.Cos(v2)) / (1 + el.ecc * Math.Cos(v2)));

        //delta eccentric anomaly
        E = E2 - E1;

        Debug.Log("E1: " + E1);
        Debug.Log("E2: " + E2);
        Debug.Log("E: " + E);

        // Calculate the standard gravitational parameter (GM)
        double Mu;
        Mu = 6.67384e-11 * (el.mass + el.massFocus);

        //calculate f and g
        double f, g, f1, g1;
        long t = shipT[j + 1] - shipT[j];

        f = 1 - el.axis * (1 - Math.Cos(E)) / r1.magnitude;
        g = t - Math.Sqrt(Math.Pow(el.axis, 3) / Mu) * (E - Math.Sin(E));

        f1 = -Math.Sqrt(Mu * el.axis) * Math.Sin(E) / (r1.magnitude * r2.magnitude);
        g1 = 1 - el.axis * (1 - Math.Cos(E)) / r2.magnitude;

        Debug.Log("f: " + f);
        Debug.Log("g: " + g);
        Debug.Log("f1: " + f1);
        Debug.Log("g1: " + g1);

        //calculate the velocities
        Vector3d vel1, vel2, vel;
        vel1 = (r2 - f * r1) / g;

        vel2 = f1 * r1 + g1 * vel1;

        //the components of the new velocity scale by the same factor
        //compute this factor
        double factor;
        factor = (vel2.magnitude + tangent) / vel2.magnitude;

        vel = vel2 * factor;

        Debug.Log("vel1: " + vel1);
        Debug.Log("vel2: " + vel2);
        Debug.Log("vel: " + vel);

        //now calculate the orbital elements of the new orbit

        //calculate the specific angular momentum
        Vector3d h;
        h.x = r2.y * vel.z - r2.z * vel.y;
        h.y = r2.z * vel.x - r2.x * vel.z;
        h.z = r2.x * vel.y - r2.y * vel.x;

        //calculate the node vector
        Vector3d n;
        n.x = -h.y;
        n.y = h.x;
        n.z = 0;

        //calculating the vector e
        Vector3d e;
        double coeff1, coeff2;
        coeff1 = Math.Pow(vel.magnitude, 2) - Mu / r2.magnitude;
        coeff2 = r2.x * vel.x + r2.y * vel.y + r2.z * vel.z;

        e = (coeff1 * r2 - coeff2 * vel) / Mu;

        el.axis = 1 / (2 / r2.magnitude - Math.Pow(vel.magnitude, 2) / Mu);
        el.ecc = e.magnitude;

        Debug.Log("new axis: " + el.axis);
        Debug.Log("new ecc: " + el.ecc);

        //update the other parameters
        el.calcData();
        
        //calculate the mean anomaly at the start of the new orbit
        Debug.Log("start of new orbit");
        //true anomaly
        v = Math.Acos(((el.axis * (1 - Math.Pow(el.ecc, 2))) / r2.magnitude - 1) / el.ecc);
        Debug.Log("v: " + v);
        //eccentric anomaly
        E = Math.Acos( (el.ecc + Math.Cos (v)) / (1 + el.ecc * Math.Cos(v)) );
        Debug.Log("E: " + E);
        //mean anomaly
        el.anom = E - el.ecc * Math.Sin(E);
        Debug.Log("anomaly: " + el.anom);

        //save the updated orbital elements
        shipOE[j + 1] = el;
        debug = true;
        //Debug.Log("r1");
        //Debug.Log(r1.x);
        //Debug.Log(r1.y);
        //Debug.Log(r1.x);
        //Debug.Log("length: " + (r1.magnitude*Global.scale) + " km");
        //Debug.Log("r2");
        //Debug.Log(r2.x);
        //Debug.Log(r2.y);
        //Debug.Log(r2.x);
        //Debug.Log("length: " + (r2.magnitude * Global.scale) + " km");

    }


    //OUTDATED


    //		//////////////////// Changed a bunch of Cin to Sin and Inital to Initial
    //	
    //		/*	This function takes the initial orbital elements of the 
    //		ship and returns the final orbital elements based on the
    //		delta-V applied in the normal/anti-normal direction.
    //		DeltaV varible should be in Perifocal Coordinates.
    //		*/
    //		void CalcNormalDeltaV (ref Elements Initial, Vector3 DeltaV)
    //		{
    //		
    //		
    //				// Approximation of the true anomaly as a sine series of the mean anomaly
    //				double TrueAnomaly;
    //				TrueAnomaly = Initial.anom;
    //				TrueAnomaly += ((2 * Initial.ecc) - (0.25f * System.Math.Pow (Initial.ecc, 3))) * System.Math.Sin (Initial.anom);
    //				TrueAnomaly += (1.25f * System.Math.Pow (Initial.ecc, 2)) * System.Math.Sin (2 * Initial.anom);
    //				TrueAnomaly += ((13 / 12) * System.Math.Pow (Initial.ecc, 3)) * System.Math.Sin (3 * Initial.anom);
    //		
    //				/* Calculate the final inclination after the maneuver */
    //		
    //				// Calculate the difference in inclination
    //				double DeltaI;
    //				double Numerator = (double)DeltaV.magnitude * (1 + (Initial.ecc * System.Math.Cos (TrueAnomaly)));
    //				double Denominator = 2 * System.Math.Sqrt (1 + System.Math.Pow (Initial.ecc, 2)) * Initial.n * Initial.axis * System.Math.Cos (Initial.arg + TrueAnomaly);
    //				DeltaI = 2 * System.Math.Asin (Numerator / Denominator);
    //		
    //				double FinalIncl = DeltaI + Initial.incl;
    //		
    //				/* Calculate the final longitude (right ascension) of the ascending node */
    //		
    //				// Calculate the magnitude of the initial velocity //
    //		
    //				// Calculate the standard gravitational parameter
    //				double Mu;
    //				Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
    //		
    //				// Calculate the specific relative angular momentum
    //				double h;
    //				h = System.Math.Sqrt (System.Math.Sqrt (1 - System.Math.Pow (Initial.ecc, 2)) * Initial.axis * Mu);
    //		
    //				// Calculate the initial velocity in Perifocal Coordinates
    //				Vector3 InitialVelocity;
    //				double IVp = -1 * Mu * System.Math.Sin (TrueAnomaly) / h;
    //				double IVq = (Initial.ecc + System.Math.Sin (TrueAnomaly)) * Mu / h; /////////////////////////////////CIN? changed to sin
    //				double IVw = 0;
    //				InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
    //		
    //				double VelocityMag = InitialVelocity.magnitude;
    //		
    //				// Calculate the angle between the initial and the final orbits
    //				double AngleBtwnOrbits;
    //				AngleBtwnOrbits = 2 * System.Math.Asin ((double)DeltaV.magnitude / (2 * VelocityMag));
    //		
    //				// Calculate the argument of latitude
    //				double ArgOfLatitude;
    //				ArgOfLatitude = TrueAnomaly + Initial.arg;
    //		
    //				// Calculate the difference between the intial and the final longitudes of ascending node
    //				double DeltaOmega;
    //				DeltaOmega = System.Math.Sin (ArgOfLatitude) * System.Math.Sin (AngleBtwnOrbits) / System.Math.Sin (FinalIncl);
    //				DeltaOmega = System.Math.Asin (DeltaOmega);
    //		
    //				double FinalAsc = DeltaOmega + Initial.asc;
    //		
    //				// Because the maneuver is only in the normal/anti-normal direction,
    //				// only the Inclination and Longitude of Ascending Node change
    //				Initial.incl = FinalIncl;
    //				Initial.asc = FinalAsc;
    //		
    //				// Calculate the final P, Q and n orbital elements after the maneuver
    //				Initial.calcData ();
    //		
    //		}
    //	
    //		/*	This function takes the initial orbital elements of the 
    //		ship and returns the final orbital elements based on the
    //		delta-V applied in the tangential/anti-tangential direction.
    //		DeltaV varible should be in Perifocal Coordinates.
    //	*/
    //		void CalcTangentialDeltaV (ref Elements Initial, Vector3 DeltaV)
    //		{
    //		
    //				// Approximation of the true anomaly as a sine series of the mean anomaly
    //				double TrueAnomaly;
    //				TrueAnomaly = Initial.anom;
    //				TrueAnomaly += ((2 * Initial.ecc) - (0.25f * System.Math.Pow (Initial.ecc, 3))) * System.Math.Sin (Initial.anom);
    //				TrueAnomaly += (1.25f * System.Math.Pow (Initial.ecc, 2)) * System.Math.Sin (2 * Initial.anom);
    //				TrueAnomaly += ((13 / 12) * System.Math.Pow (Initial.ecc, 3)) * System.Math.Sin (3 * Initial.anom);
    //		
    //				// Calculate the standard gravitational parameter
    //				double Mu;
    //				Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
    //		
    //				// Calculate the specific relative angular momentum
    //				double h;
    //				h = System.Math.Sqrt (System.Math.Sqrt (1 - System.Math.Pow (Initial.ecc, 2)) * Initial.axis * Mu);
    //		
    //				// Calculate the initial velocity in Perifocal Coordinates
    //				Vector3 InitialVelocity;
    //				double IVp = -1 * Mu * System.Math.Sin (TrueAnomaly) / h;
    //				double IVq = (Initial.ecc + System.Math.Sin (TrueAnomaly)) * Mu / h;
    //				double IVw = 0;
    //				InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
    //		
    //				// Calculate the magnitude of the initial velocity
    //				double VelocityMag = InitialVelocity.magnitude;
    //		
    //				double r;
    //				r = System.Math.Pow (h, 2) / Mu / (1 + (Initial.ecc * System.Math.Cos (TrueAnomaly)));
    //		
    //				// Calculate the new specific orbital energy
    //				double EnergNew;
    //				EnergNew = ((System.Math.Pow (VelocityMag, 2) + System.Math.Pow ((double)DeltaV.magnitude, 2)) / 2) - (Mu / r); 
    //		
    //				// Calculate the new semi-major axis //
    //				Initial.axis = -1 * Mu / 2 / EnergNew;
    //		
    //				// Calculate the orbital distance from object to primary
    //		
    //		
    //				 ///////////////////////////////////////////////////////////OLD LOCATION
    //		//double r;
    //		//r = System.Math.Pow(h, 2) / Mu / (1 + (Initial.ecc * System.Math.Cos(TrueAnomaly)));
    //
    //		
    //		
    //				// Calculate the position of the object in Perifocal Coordinates
    //				Vector3 rVec;
    //				double rVp = r * System.Math.Cos (TrueAnomaly);
    //				double rVq = r * System.Math.Sin (TrueAnomaly);
    //				double rVw = 0;
    //				rVec = new Vector3 ((float)rVp, (float)rVq, (float)rVw);
    //		
    //				// Calculate the magnitude of the new angular velocity vector
    //				double hNewMag;
    //				hNewMag = (double)Vector3.Cross (rVec, InitialVelocity + DeltaV).magnitude;
    //		
    //				/////////// Calculate the new eccentricity ///////
    //				////////////////////////////////////////// hNew -------------->  hNewMagg ?????????????????????????????????????
    //				Initial.ecc = System.Math.Sqrt (1 + (System.Math.Pow (hNewMag, 2) * EnergNew / System.Math.Pow (Mu, 2)));
    //		
    //				////// Calculate the final P, Q, W and n orbital elements after the maneuver ///////
    //				Initial.calcData ();
    //		}
    //
    //		/*	This function takes the initial orbital elements of the 
    //		ship and returns the final orbital elements based on the
    //		delta-V applied in the radial/anti-radial direction.
    //		DeltaV varible should be in Perifocal Coordinates.
    //		*/
    //		void CalcRadialDeltaV (ref Elements Initial, Vector3 DeltaV)
    //		{
    //		
    //				// Approximation of the true anomaly as a sine series of the mean anomaly
    //				double TrueAnomaly;
    //				TrueAnomaly = Initial.anom;
    //				TrueAnomaly += ((2 * Initial.ecc) - (0.25f * System.Math.Pow (Initial.ecc, 3))) * System.Math.Sin (Initial.anom);
    //				TrueAnomaly += (1.25f * System.Math.Pow (Initial.ecc, 2)) * System.Math.Sin (2 * Initial.anom);
    //				TrueAnomaly += ((13 / 12) * System.Math.Pow (Initial.ecc, 3)) * System.Math.Sin (3 * Initial.anom);
    //		
    //				// Calculate the standard gravitational parameter
    //				double Mu;
    //				Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
    //		
    //				// Calculate the specific relative angular momentum
    //				double h;
    //				h = System.Math.Sqrt (System.Math.Sqrt (1 - System.Math.Pow (Initial.ecc, 2)) * Initial.axis * Mu);
    //		
    //				// Calculate the initial velocity in Perifocal Coordinates
    //				Vector3 InitialVelocity;
    //				double IVp = -1 * Mu * System.Math.Sin (TrueAnomaly) / h;
    //				double IVq = (Initial.ecc + System.Math.Sin (TrueAnomaly)) * Mu / h;
    //				double IVw = 0;
    //				InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
    //		
    //				// Calculate the magnitude of the initial velocity
    //				double VelocityMag = InitialVelocity.magnitude;
    //		
    //				// Calculate the orbital distance from object to primary
    //				double r;
    //				r = System.Math.Pow (h, 2) / Mu / (1 + (Initial.ecc * System.Math.Cos (TrueAnomaly)));
    //		
    //				// Calculate the new specific orbital energy
    //				double EnergNew;
    //				EnergNew = ((System.Math.Pow (VelocityMag, 2) + System.Math.Pow ((double)DeltaV.magnitude, 2)) / 2) - (Mu / r); 
    //		
    //				/***** Calculate the new semi-major axis *****/
    //				Initial.axis = -1 * Mu / 2 / EnergNew;
    //		
    //				// Calculate the magnitude of the new angular velocity vector
    //				double hNew;
    //				hNew = System.Math.Sqrt (Mu * Initial.axis * (1 - System.Math.Pow (Initial.ecc, 2)));
    //		
    //				/***** Calculate the new eccentricity *****/
    //				Initial.ecc = System.Math.Sqrt (1 + (System.Math.Pow (hNew, 2) * EnergNew / System.Math.Pow (Mu, 2)));
    //		
    //				/***** Calculate the final P, Q, W and n orbital elements after the maneuver *****/
    //				Initial.calcData ();
    //		}
}