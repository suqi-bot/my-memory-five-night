using UnityEngine;

public class CameraSystem : MonoBehaviour, ItemInterface
{
    public void UseItem()
    {
        Debug.Log("打开监控");
        UIManager.Instance.ShowPanel<ControlPanel>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.HidePanel<ControlPanel>(false);
        }
    }
}
