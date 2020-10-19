using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Honeti;
using NumberConversions;
using SilknowMap;
using UnityEngine;
using UnityEngine.UI;

public class StackedMapManager : Singleton<StackedMapManager>
{
    public Dropdown mapSlicesDropdown;
    public List<TimeElement> centuriesValues;

    private void Awake()
    {
        
        /* TEST FUNCION ORDENADO DE SIGLOS
        var centuriesTest = new List<TimeElement>();
        
         var listOfCenturies =  Enumerable.Range(13, 5).Select(n => n).ToList();
         foreach (var c in listOfCenturies)
         {
             centuriesTest.Add(new TimeElement(0,0,c));
         }
        
        var splittedList = SplitCenturiesInChunks(centuriesTest, 4);
        foreach (var centuryList in splittedList)
        {
            var s = "{ ";
            foreach (var cent in centuryList)
            {
                s += cent.century + ", ";
            }

            s += "}";
            print(s);
        }
        */
    }

    public void UpdateSlicesRestrictions()
    {
        var possibleValues = SilkMap.instance.map.GetPropertyManager().GetPropertyByName("time").getPossibleValues();
        if (centuriesValues == null)
            centuriesValues = new List<TimeElement>();
        else
            centuriesValues.Clear();

        foreach (var val in possibleValues)
        {
            if (APIManager.instance.timeValues.TryGetValue(val, out var timeElement)) centuriesValues.Add(timeElement);
        }

        centuriesValues = centuriesValues.OrderBy(t => t.century).ToList();
        mapSlicesDropdown.ClearOptions();
        var opt = new List<string>();
        if (centuriesValues.Count <= 1)
        {
            //DESACTIVAR EL BOTÓN DE STACKED MAP
            opt.Clear();
        }
        else if (centuriesValues.Count == 2)
        {
            opt.Clear();
            opt.Add("2");
        }
        else if (centuriesValues.Count == 3)
        {
            opt.Clear();
            opt = Enumerable.Range(2, 2).Select(n => n.ToString()).ToList();
        }
        else if (centuriesValues.Count >= 4)
        {
            opt.Clear();
            opt = Enumerable.Range(2, 3).Select(n => n.ToString()).ToList();
        }

        mapSlicesDropdown.AddOptions(opt);
        
    }

    public void CreateStackedMaps()
    {
        if (mapSlicesDropdown.options == null || mapSlicesDropdown.options.Count < 1) return;
        var numberOfSlices = Int32.Parse(mapSlicesDropdown.options[mapSlicesDropdown.value].text);
        if (numberOfSlices < 2) return;
        var splittedList = SplitCenturiesInChunks(centuriesValues, numberOfSlices);
        foreach (var centuryList in splittedList)
        {
            var s = "{ ";
            foreach (var cent in centuryList)
            {
                s += cent.century + ", ";
            }

            s += "}";
        }

        var listOfMaps = new List<GameObject>();
        foreach (var centuryList in splittedList)
        {
            var firstCentury = centuryList.First();
            var lastCentury = centuryList.Count > 1 ? centuryList.Last() : centuryList.First();
            var go = SilkMap.Instance.createPlane(firstCentury.from, lastCentury.to);
            //go.name = $"Centuries {centuryList.First().century} to {centuryList.Last().century}";

            var first = centuryList.First().century;
            var last = centuryList.Last().century;
            var from = I18N.instance.gameLang == LanguageCode.EN
                ? NumericConversions.AddOrdinal(first)
                : NumericConversions.ArabicToRoman(first);
            var to = I18N.instance.gameLang == LanguageCode.EN
                ? NumericConversions.AddOrdinal(last)
                : NumericConversions.ArabicToRoman(last);
            var timeParams = new[]{from,to};
            go.name = I18N.instance.getValue("^stacked_centuries_label",timeParams);
            listOfMaps.Add(go);
            go.layer = LayerMask.NameToLayer("StackedMap");
            RunOnChildrenRecursive(go, child => child.layer = LayerMask.NameToLayer("StackedMap"));
        }

        MapUIManager.instance.ToggleMapViewingMode(listOfMaps);
    }
    public static void RunOnChildrenRecursive(GameObject go, Action<GameObject> action)
    {
        if (go == null) return;
        foreach (var trans in go.GetComponentsInChildren<Transform>(true))
        {
            action(trans.gameObject);
        }
    }

    private List<List<TimeElement>> SplitCenturiesInChunks(List<TimeElement> listOfCenturies, int numberOfChunks)
    {
        var dds = new List<List<TimeElement>>();

        // Determine how many lists are required
        int numberOfElementsPerChunk = (listOfCenturies.Count / numberOfChunks);
        int numberOfExtraElements = (listOfCenturies.Count % numberOfChunks);

        var extraElementsAdded = 0;
        var remainingExtraElements = numberOfExtraElements;
        for (int i = 0; i < numberOfChunks; i++)
        {
            extraElementsAdded = numberOfExtraElements - remainingExtraElements;
            var extra = numberOfExtraElements > 0 ? 1 : 0;
            int takeExtra;
            if (remainingExtraElements > 0)
            {
                takeExtra = 1;
                remainingExtraElements--;
            }
            else
            {
                takeExtra = 0;
            }

           
            var newdd = listOfCenturies.Skip(i * (numberOfElementsPerChunk) + extraElementsAdded)
                .Take(numberOfElementsPerChunk + takeExtra)
                .ToList();
            dds.Add(newdd);
        }

        return dds;
    }
}

static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this ICollection<T> items, int numberOfChunks)
    {
        if (numberOfChunks <= 0 || numberOfChunks > items.Count)
            throw new ArgumentOutOfRangeException("numberOfChunks");

        int sizePerPacket = items.Count / numberOfChunks;
        int extra = items.Count % numberOfChunks;

        for (int i = 0; i < numberOfChunks - extra; i++) yield return items.Skip(i * sizePerPacket).Take(sizePerPacket);

        int alreadyReturnedCount = (numberOfChunks - extra) * sizePerPacket;
        int toReturnCount = extra == 0 ? 0 : (items.Count - numberOfChunks) / extra + 1;
        for (int i = 0; i < extra; i++)
            yield return items.Skip(alreadyReturnedCount + i * toReturnCount).Take(toReturnCount);
    }
}