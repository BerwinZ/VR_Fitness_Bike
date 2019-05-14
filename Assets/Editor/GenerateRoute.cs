using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateRoute : MonoBehaviour
{
    static GameObject route;
    static GameObject blockPrefab;
    static GameObject preBlock;
    static int cnt = 1;
    static float RoadLength = 1f;
    static int PreAngle = 0;
    static int roadIndex = 0;

    static Vector3 AbsLocation = new Vector3(0, 0, 0);
    static Vector3 AbsRotation = new Vector3(0, 0, 0);
    static List<float> DistanceList = new List<float>{ 440, 250, 190, 200, 250, 190, 200, 250, 190, 200,
     250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 750};
    static List<float> DegreeList = new List<float>{ 0, 15, 0, -5, 15, 0, -5, 15, 0, -5, 15, 0, -5, 15, 0, -5,
     15, 0, -5, 15, 0, -5, 15, 0, -5, 15, 0, -5, 0};
    static List<float> TurningList = new List<float> { 0, 15, 15, 30, 30 , 35, 25, -30 ,-35, -30, 10, 10, 10, -30, 26, 15, 15, -40, -35,
     0, 0, 0, 40, 15, 15, 15, 15, -15, -35, -30};


    [MenuItem("Tools/Generate Road")]
    static void EstablishRoad()
    {
        // Load prefab
        blockPrefab = (GameObject)Resources.Load("Block", typeof(GameObject));
        // Create a empty obj for the route
        route = new GameObject();
        // Change name
        route.name = "Route";
        // Record the last road obj
        preBlock = Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        // Set the name
        preBlock.name = "Block_" + roadIndex++;
        // Set the parent of preBlock
        preBlock.transform.parent = route.transform;
        // Smooth the slope slopeAngle changes
        SmoothDegChange(DistanceList, DegreeList);
        // Add value to turning list
        //RoadTurning();
        // Iterate the list to generate roads
        for (int i = 0; i < DistanceList.Count; i++)
        {
            // Generate road
            GenerateRoad(DistanceList[i], DegreeList[i], TurningList[i]);

            Debug.Log(preBlock.transform.position);
            //GenerateRoad(List1[i], List2[i]);
        }

    }

    // Generate several road obj according to the parameters
    static void GenerateRoad(float length, float slopeAngle, float TurningAngle)// For level road use 360 degree instead of 0
    {
        // Record the last obj before turning
        GameObject objBeforeTurn = null;
        
        // Every obj turning slopeAngle related to the previous obj
        float unitTurnAngle = TurningAngle / 60;

        // Accumulated numbers of turning slopeAngle
        float accTurnAngle = 0;

        // Generate road according to its length
        for (int i = 0; i < length / RoadLength; i++)
        {
            // Generate a new obj
            GameObject obj = Instantiate(blockPrefab);
            // Set name
            obj.name = "Block_" + roadIndex++;
            // Add obj to the Route object
            obj.transform.parent = route.transform;
            // Generate turning blocks from 20th to 80th
            if (i > 20 && i <= 80 && TurningAngle!= 0)
            {
                // Calculate the new turning angle
                accTurnAngle = accTurnAngle + unitTurnAngle;
                // Reset the obj's position to the position of objBeforeTurn
                obj.transform.position = objBeforeTurn.transform.position;
                // Reset the obj's rotation to the rotation of objBeforeTurn
                obj.transform.rotation = objBeforeTurn.transform.rotation;
                // Canculate the turning point according to the preset parameters
                Vector3 turningPoint = objBeforeTurn.transform.position +
                    objBeforeTurn.transform.right.normalized * 60 * TurningAngle / Mathf.Abs(TurningAngle);
                // Rotate the block to make turning
                obj.transform.RotateAround(turningPoint, 
                                            objBeforeTurn.transform.up.normalized, 
                                            accTurnAngle);
            }
            else if(i == 0)
            {
                // Set the slopeAngle
                obj.transform.eulerAngles = new Vector3(-slopeAngle, 
                                                        preBlock.transform.eulerAngles.y, 
                                                        preBlock.transform.eulerAngles.z);
                // Set the position according to the previous obj
                obj.transform.position = preBlock.transform.position +
                                         preBlock.transform.forward.normalized * RoadLength;
            }
            else
            {
                // Set the slopeAngle 
                obj.transform.rotation = preBlock.transform.rotation; 
                // Set the position according to the previous obj
                obj.transform.position = preBlock.transform.position +
                                         preBlock.transform.forward.normalized * RoadLength;
            }

            // Update the preBlock
            preBlock = obj;
            // Record the block before turn
            if (i == 20)
            { 
                // Assign the 20th obj to objBeforeTurn
                objBeforeTurn = obj;
            }
        }
    }

    static void SmoothDegChange(List<float> DisList, List<float> DegList)
    {
        float DegChange = 0;
        int index = 1;
        while (true)
        {
            if (index + 4 > DisList.Count - 1)
                break;

            if (DegList[index] * DegList[index - 1] < 0)
            {

                float DegChange2 = Mathf.Abs(DegList[index]) - 0;
                float DegChange1 = 0 - Mathf.Abs(DegList[index - 1]);

                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);

                DegList.Insert(index, 4 * DegChange2 / 5);
                DegList.Insert(index, 3 * DegChange2 / 5);
                DegList.Insert(index, 2 * DegChange2 / 5);
                DegList.Insert(index, 1 * DegChange2 / 5);
                DegList.Insert(index, 0);
                //Expend turning list
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);

                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);

                DegList.Insert(index, 1 * DegChange1 / 5);
                DegList.Insert(index, 2 * DegChange1 / 5);
                DegList.Insert(index, 3 * DegChange1 / 5);
                DegList.Insert(index, 4 * DegChange1 / 5);
                //Expend turning list
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);
                TurningList.Insert(index, 0);
                

                index = index + 10;
            }
            else
            {
                if (Mathf.Abs(DegList[index]) - Mathf.Abs(DegList[index - 1]) < 0)
                {
                    DegChange = Mathf.Abs(DegList[index] - DegList[index - 1]);

                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);

                    DegList.Insert(index, 1 * DegChange / 5);
                    DegList.Insert(index, 2 * DegChange / 5);
                    DegList.Insert(index, 3 * DegChange / 5);
                    DegList.Insert(index, 4 * DegChange / 5);

                    //Expend turning list
                    TurningList.Insert(index, 0);
                    TurningList.Insert(index, 0);
                    TurningList.Insert(index, 0);
                    TurningList.Insert(index, 0);
                }
                else
                {
                    DegChange = DegList[index] - Mathf.Abs(DegList[index - 1]);

                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);

                    DegList.Insert(index, 4 * DegChange / 5);
                    DegList.Insert(index, 3 * DegChange / 5);
                    DegList.Insert(index, 2 * DegChange / 5);
                    DegList.Insert(index, 1 * DegChange / 5);

                    //Expend turning list
                    TurningList.Insert(index, 0);
                    TurningList.Insert(index, 0);
                    TurningList.Insert(index, 0);
                    TurningList.Insert(index, 0);
                }
                index = index + 5;
            }

        }
    }


    // Random generate values to the turning list
    static void RoadTurning()
    {
        int Turingdegree = 0;

        for (int i = 0; i < DistanceList.Count; i++)
        {
            //Decide if there is a turning
            float booleanTurn = Random.Range(0f, 1f);
            
            if (DistanceList[i] > 5 * RoadLength)
            {
                if (booleanTurn >= 0.5f)
                {
                    Turingdegree = Random.Range(-30, 30);
                }
            }
            TurningList.Add(Turingdegree);
            Debug.Log("Turning Degree:   " + Turingdegree);
        }

    }

}