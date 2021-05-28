using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Honeti;
using NumberConversions;
using UnityEngine;
using UnityEngine.UI;
using SilknowMap;
using UnityEngine.UI.Extensions;
using Debug = UnityEngine.Debug;

public class TimeSlider : MonoBehaviour
{
    public Slider slider;

    public Text sliderText;
    public GameObject missingDataText;

    [SerializeField] private List<TimeElement> currentValues;

    // Start is called before the first frame update
    private void OnEnable()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(HandleValueChanged);
        sliderText.text = I18N.instance.gameLang == LanguageCode.EN
            ? NumericConversions.AddOrdinal((int) slider.value)
            : NumericConversions.ArabicToRoman((int) slider.value);
        if (currentValues == null) currentValues = new List<TimeElement>();
    }

    public void initSliderOnShow()
    {
        if (currentValues.Count < 1)
        {
            missingDataText.SetActive(true);
            slider.gameObject.SetActive(false);
            return;
        }

        missingDataText.SetActive(false);
        slider.gameObject.SetActive(true);

        HandleValueChanged(slider.value);
    }

    private void HandleValueChanged(float value)
    {
        var crono = Stopwatch.StartNew();

        UpdateTimeFrame(value);

        Debug.Log($"UpdateTimeFrame {crono.ElapsedMilliseconds * 0.001f} segundos");
        //sliderText.text = slider.value.ToString(CultureInfo.InvariantCulture);
        sliderText.text = I18N.instance.gameLang == LanguageCode.EN
            ? NumericConversions.AddOrdinal((int) slider.value)
            : NumericConversions.ArabicToRoman((int) slider.value);
    }

    private void UpdateTimeFrame(float value)
    {
        var crono = Stopwatch.StartNew();
        var selectedCentury = currentValues.Find(t => t.century == (int) value);
        Debug.Log($"currentValues.Find {crono.ElapsedMilliseconds * 0.001f} segundos");
        if (selectedCentury != null)
        {
            //Debug.LogFormat("Llamo a ActivateTimeFrame from:{0} to {1}",selectedCentury.@from,selectedCentury.to);
            crono = Stopwatch.StartNew();
            SilkMap.instance.map.activateTimeFrame(selectedCentury.@from, selectedCentury.to);
            Debug.Log($"activateTimeFrame {crono.ElapsedMilliseconds * 0.001f} segundos");
            AnalyticsMonitor.instance.sendEvent("Timeline_Update",
                new Dictionary<string, object> {{"from", selectedCentury.@from}, {"to", selectedCentury.to}});
        }
        else
        {
            var timeElement = APIManager.instance.timeValues.First(t => t.Value.century == (int) value).Value;
            //Debug.LogFormat("OCULTAR:  Llamo a ActivateTimeFrame from:{0} to {1}",timeElement.@from,timeElement.to);
            SilkMap.instance.map.activateTimeFrame(timeElement.@from, timeElement.to);
        }
    }

    public void SetPropertyValues(Property timeProp)
    {
        var values = timeProp.getPossibleValues();

        if (currentValues == null)
            currentValues = new List<TimeElement>();
        else
            currentValues.Clear();

        if (values.Count < 1) return;

        foreach (var val in values)
        {
            if (APIManager.instance.timeValues.TryGetValue(val, out var timeElement)) currentValues.Add(timeElement);
        }

        currentValues = currentValues.OrderBy(t => t.century).ToList();

        slider.minValue = currentValues.Min(t => t.century);
        slider.maxValue = currentValues.Max(t => t.century);
    }
}