using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SilknowMap;
using UnityEngine.UI.Extensions;

public class TimeSlider : MonoBehaviour
{
    public Slider slider;

    public Text sliderText;
    [SerializeField]
    private List<TimeElement> currentValues;
    // Start is called before the first frame update
    private void OnEnable()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(HandleValueChanged);
        sliderText.text = slider.value.ToString();
        if(currentValues == null) 
            currentValues = new List<TimeElement>();
    }

    public void initSliderOnShow()
    {
        HandleValueChanged(slider.value);
    }
    
    private void HandleValueChanged(float value)
    {
        UpdateTimeFrame(value);
        sliderText.text = slider.value.ToString();
    }

    private void UpdateTimeFrame(float value)
    {
        var selectedCentury = currentValues.Find(t => t.century == (int) value);
        if (selectedCentury != null)
        {
            //Debug.LogFormat("Llamo a ActivateTimeFrame from:{0} to {1}",selectedCentury.@from,selectedCentury.to);
            SilkMap.instance.map.activateTimeFrame(selectedCentury.@from, selectedCentury.to);
        }
        else{
            var timeElement = APIManager.instance.timeValues.First(t=>t.Value.century == (int)value).Value;
            Debug.LogFormat("OCULTAR:  Llamo a ActivateTimeFrame from:{0} to {1}",timeElement.@from,timeElement.to);
            SilkMap.instance.map.activateTimeFrame(timeElement.@from,timeElement.to);
        }

    }

    public void SetPropertyValues(Property timeProp)
    {
        var values = timeProp.getPossibleValues();
        
        if(currentValues == null)
            currentValues = new List<TimeElement>();
        else
            currentValues.Clear();
        
        foreach (var val in values)
        {
            if(APIManager.instance.timeValues.TryGetValue(val,out var timeElement))
                currentValues.Add(timeElement);
        }

        currentValues = currentValues.OrderBy(t => t.century).ToList();

        slider.minValue = currentValues.Min(t=>t.century);
        slider.maxValue = currentValues.Max(t=>t.century);

    }

}
