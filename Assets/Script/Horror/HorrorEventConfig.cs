using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HorrorEventConfig", menuName = "Horror/Event Config")]
public class HorrorEventConfig : ScriptableObject
{
    public List<HorrorEventData> events = new List<HorrorEventData>();

    public List<HorrorEventData> GetEventsForIntensity(float intensity)
    {
        List<HorrorEventData> result = new List<HorrorEventData>();
        for (int i = 0; i < events.Count; i++)
        {
            if (intensity >= events[i].minIntensity && intensity <= events[i].maxIntensity)
            {
                result.Add(events[i]);
            }
        }
        return result;
    }
}
