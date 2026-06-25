using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Ins.PlaySpatialSFX("BGM_FoundMe",this.transform.position, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
