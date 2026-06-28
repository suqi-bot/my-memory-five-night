using UnityEngine;
using UnityEngine.UI;

public class GameSencePanel : UIPanel
{
    public Image enduranceBG;
    public Image enduranceFG;

    public override void Init(object data = null)
    {
        base.Init(data);
    }

    public void ChangeEnduranceValue(float max, float now)
    {
        enduranceFG.rectTransform.sizeDelta = new Vector2(
            enduranceBG.rectTransform.sizeDelta.x * now / max,
            enduranceFG.rectTransform.sizeDelta.y
        );
    }
}
