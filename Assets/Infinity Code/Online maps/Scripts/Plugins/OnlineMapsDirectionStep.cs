/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Obsolete]
public class OnlineMapsDirectionStep
{
    /// <summary>
    /// The total distance covered by this step.
    /// </summary>
    public int distance;

    /// <summary>
    /// The total duration of the passage of this step.
    /// </summary>
    public int duration;

    /// <summary>
    /// Location of the endpoint of this step (lng, lat). 
    /// </summary>
    public OnlineMapsVector2d end;

    /// <summary>
    /// HTML instructions to the current step.
    /// </summary>
    public string instructions;

    /// <summary>
    /// Maneuver the current step.
    /// </summary>
    public string maneuver;

    /// <summary>
    /// A list of locations of points included in the current step.
    /// </summary>
    public List<Vector2> points;

    public List<OnlineMapsVector2d> pointsD;

    /// <summary>
    /// Location of the startpoint of this step (lng, lat). 
    /// </summary>
    public OnlineMapsVector2d start;

    /// <summary>
    /// Instructions to the current step without HTML tags.
    /// </summary>
    public string stringInstructions;

    private OnlineMapsDirectionStep()
    {
        
    }

    /// <summary>
    /// Constructor. \n
    /// Use OnlineMapsDirectionStep.TryParse.
    /// </summary>
    /// <param name="node">XMLNode of route</param>
    private OnlineMapsDirectionStep(OnlineMapsXML node)
    {
        start = node.GetLatLng("start_location");
        end = node.GetLatLng("end_location");
        duration = node.Find<int>("duration/value");
        instructions = node.Find<string>("html_instructions");
        GetStringInstructions();
        distance = node.Find<int>("distance/value");

        maneuver = node.Find<string>("maneuver");
        
        string encodedPoints = node.Find<string>("polyline/points");
        pointsD = OnlineMapsUtils.DecodePolylinePointsD(encodedPoints);
        points = pointsD.Select(p => (Vector2)p).ToList();
    }

    [Obsolete("Use OnlineMapsGoogleDirections.GetResult")]
    public static List<Vector2> GetPoints(List<OnlineMapsDirectionStep> steps)
    {
        List<Vector2> routePoints = new List<Vector2>();

        foreach (OnlineMapsDirectionStep step in steps)
        {
            if (routePoints.Count > 0) routePoints.RemoveAt(routePoints.Count - 1);
            routePoints.AddRange(step.points);
        }

        return routePoints;
    }

    [Obsolete("Use OnlineMapsGoogleDirections.GetResult")]
    public static List<OnlineMapsVector2d> GetPointsD(List<OnlineMapsDirectionStep> steps)
    {
        List<OnlineMapsVector2d> routePoints = new List<OnlineMapsVector2d>();

        foreach (OnlineMapsDirectionStep step in steps)
        {
            if (routePoints.Count > 0) routePoints.RemoveAt(routePoints.Count - 1);
            routePoints.AddRange(step.pointsD);
        }

        return routePoints;
    }

    private void GetStringInstructions()
    {
        if (string.IsNullOrEmpty(instructions)) return;
        stringInstructions = OnlineMapsUtils.StrReplace(instructions, 
            new[] {"&lt;", "&gt;", "&nbsp;", "&amp;", "&amp;nbsp;"},
            new[] {"<",    ">",    " ",      "&",     " "});
        stringInstructions = Regex.Replace(stringInstructions, "<div.*?>", "\n");
        stringInstructions = Regex.Replace(stringInstructions, "<.*?>", string.Empty);
    }

    [Obsolete("Use OnlineMapsGoogleDirections.GetResult")]
    public static List<OnlineMapsDirectionStep> TryParse(string route)
    {
        try
        {
            OnlineMapsXML xml = OnlineMapsXML.Load(route);

            OnlineMapsXML direction = xml.Find("//DirectionsResponse");
            if (direction.isNull) return null;

            string status = direction.Find<string>("status");
            if (status != "OK") return null;

            OnlineMapsXMLList legNodes = direction.FindAll("route/leg");
            if (legNodes == null || legNodes.count == 0) return null;

            List<OnlineMapsDirectionStep> steps = new List<OnlineMapsDirectionStep>();

            foreach (OnlineMapsXML legNode in legNodes)
            {
                OnlineMapsXMLList stepNodes = legNode.FindAll("step");
                if (stepNodes.count == 0) continue;

                foreach (OnlineMapsXML step in stepNodes)
                {
                    OnlineMapsDirectionStep navigationStep = new OnlineMapsDirectionStep(step);
                    steps.Add(navigationStep);
                }
            }

            return steps;
        }
        catch { }

        return null;
    }

    [Obsolete("Use OnlineMapsOpenRouteService.GetDirectionResults")]
    public static List<OnlineMapsDirectionStep> TryParseORS(string route)
    {
        return null;
    }

    /// <summary>
    /// Converts the route obtained by OnlineMapsFindDirection, to array of list of the steps of the route.
    /// </summary>
    /// <param name="route">Route obtained by OnlineMapsFindDirection.</param>
    /// <returns>Array of list of OnlineMapsDirectionStep or null.</returns>
    public static List<OnlineMapsDirectionStep>[] TryParseWithAlternatives(string route)
    {
        try
        {
            OnlineMapsXML xml = OnlineMapsXML.Load(route);

            OnlineMapsXML direction = xml.Find("//DirectionsResponse");
            if (direction.isNull) return null;

            string status = direction.Get<string>("status");
            if (status != "OK") return null;

            OnlineMapsXMLList routes = direction.FindAll("route");
            List<OnlineMapsDirectionStep>[] result = new List<OnlineMapsDirectionStep>[routes.count];

            for(int i = 0; i < routes.count; i++)
            {
                OnlineMapsXMLList legNodes = routes[i].FindAll("leg");
                if (legNodes == null || legNodes.count == 0) continue;

                List<OnlineMapsDirectionStep> steps = new List<OnlineMapsDirectionStep>();

                foreach (OnlineMapsXML legNode in legNodes)
                {
                    OnlineMapsXMLList stepNodes = legNode.FindAll("step");
                    if (stepNodes.count == 0) continue;

                    foreach (OnlineMapsXML step in stepNodes)
                    {
                        OnlineMapsDirectionStep navigationStep = new OnlineMapsDirectionStep(step);
                        steps.Add(navigationStep);
                    }
                }

                result[i] = steps;
            }

            return result;
        }
        catch { }

        return null;
    }
}