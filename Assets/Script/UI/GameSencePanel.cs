using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSencePanel : BasePanel
{
    public Image enduranceBG;
    public Image enduranceFG;
    public override void Init()
    {
        
    }

    public void ChangeEnduranceValue(float max, float now)
    {
        enduranceFG.rectTransform.sizeDelta = new Vector2(enduranceBG.rectTransform.sizeDelta.x * now / max
            , enduranceFG.rectTransform.sizeDelta.y);
    }

}
