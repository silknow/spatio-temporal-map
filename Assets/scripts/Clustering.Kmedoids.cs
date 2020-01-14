using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;

namespace Clustering
{
    class Algorithm
    {

        public static List<DataPoint> KMedoidsAlgorithm(List<DataPoint> points, int k, int maxSteps)
        {
            if (k > points.Count() || k < 1)
            {
                throw new Exception("K must be between 0 and set size");
            }


            Debug.Log("Initial set:");
            foreach (DataPoint p in points)
                Debug.Log(" " + p);
            Debug.Log("");


            DataPoint[] medoids = new DataPoint[k];
            string[] resultClusterPoints = new string[k];
            DataPoint[] resultPoints = new DataPoint[points.Count];
            DataPoint[] stepmedoids = new DataPoint[k];
            //Random random = new Random();
            List<int> medoidsIndexes = new List<int>();
            int randIndex = 0;

            //initilizing random different medoids
            for (int i = 0; i < k; i++)
            {
                randIndex = UnityEngine.Random.Range(0, points.Count());
                if (!medoidsIndexes.Contains(randIndex))
                {
                    stepmedoids[i] = points[randIndex];
                    medoidsIndexes.Add(randIndex);
                }
                else
                {
                    i--;
                }
            }

            int step = 0;
            double resultSumFunc = double.MaxValue;
            double stepSumFunc = 0;

            while (step < maxSteps)
            {
                Debug.Log("Medoids");
                for (int i = 0; i < k; i++)
                {
                    Debug.Log(" " + stepmedoids[i]);
                }
                Debug.Log("");

                //initial clustering to medoids
                double[] clusterSumFunc = new double[k];
                string[] clusterPoints = new string[k];

                double dist = 0;
                for (int i = 0; i < points.Count(); i++)
                {
                    double minDist = 3; // double.MaxValue;
                    dist = 0;
                    for (int c = 0; c < k; c++)
                    {
                        dist = Math.Sqrt(Math.Pow(points[i].X - stepmedoids[c].X, 2) + Math.Pow(points[i].Y - stepmedoids[c].Y, 2));
                        if (dist < minDist)
                        {
                            points[i].Cluster = c;
                            minDist = dist;
                        }
                    }
                    //getting sumFunc result for all clusters
                    clusterSumFunc[points[i].Cluster] += minDist;
                    stepSumFunc += minDist;
                    if (clusterPoints[points[i].Cluster] == null)
                        clusterPoints[points[i].Cluster] = "";
                    clusterPoints[points[i].Cluster] += " " + points[i].ToString();
                }

                //printing clusters
                for (int i = 0; i < k; i++)
                {
                    Debug.Log("Cluster "+ i+ "Weight " + clusterSumFunc[i]+ " : "+ clusterPoints[i]);
                }

                //Debug.Log("""Result Weight {0:F3}", stepSumFunc);

                //if result of sumFinc is better than previous, save the configuration
                if (stepSumFunc < resultSumFunc)
                {
                    Debug.Log("CONFIG CHANGED TO");
                    resultSumFunc = stepSumFunc;
                    for (int i = 0; i < k; i++)
                    {
                        medoids[i] = stepmedoids[i];
                        resultClusterPoints[i] = clusterPoints[i];
                        Debug.Log(" " + medoids[i]);
                    }
                    points.CopyTo(resultPoints);
                    Debug.Log("");
                }
                stepSumFunc = 0;
                step++;

                //random swapping medoids with nonmedoids
                int[] clusterSwapRandomCost = new int[k];
                int[] indexOfSwapCandidate = new int[k];

                int randomValue = 0;

                for (int i = 0; i < points.Count(); i++)
                {
                    randomValue = UnityEngine.Random.Range(0, points.Count());
                    if (clusterSwapRandomCost[points[i].Cluster] < randomValue && stepmedoids[points[i].Cluster] != points[i])
                    {
                        indexOfSwapCandidate[points[i].Cluster] = i;
                        clusterSwapRandomCost[points[i].Cluster] = randomValue;
                    }
                }

                for (int i = 0; i < k; i++)
                {
                    stepmedoids[i] = points[indexOfSwapCandidate[i]];
                }
            }

            Debug.Log("RESULT:");
            Debug.Log("Medoids");
            for (int i = 0; i < k; i++)
            {
                Debug.Log(" " + medoids[i]);
            }
            Debug.Log("");

            string[] resultClusters = new string[k];

            for (int i = 0; i < k; i++)
                Debug.Log("Cluster "+ i +":"+ resultClusterPoints[i]);

            return resultPoints.ToList();
        }
    }
}
