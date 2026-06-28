using UnityEngine;
using UnityEngine.EventSystems;

public class MUINav_Item : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler //, IPointerExitHandler
{
    /// <summary>
    /// 相邻对象 [0]=上, [1]=右, [2]=下, [3]=左 (顺时针)
    /// </summary>
    public MUINav_Item[] neighbors = new MUINav_Item[4];

    public GameObject onSelect;
    protected MUINav_Controller controller;

    public Vector3 defScale = Vector3.one;
    public Vector3 selectScale = Vector3.one;

    public void Init(MUINav_Controller controller)
    {
        this.controller = controller;
    }

    //点击效果
    public virtual void OnClick()
    {
        Debug.Log("点击" + name);
        OnSelect();
    }

    public virtual void UnClick()
    {
        Debug.Log("取消" + name);
        OnDeselect();
    }

    public virtual void UnClick(bool update)
    {
        UnClick();
    }

    //选择效果
    public virtual void OnSelect()
    {
        if (onSelect != null)
            onSelect.SetActive(true);
        transform.localScale = selectScale;
        Debug.Log("MUINav_Item OnSelect " + name);
    }

    //取消选中效果（默认
    public virtual void OnDeselect()
    {
        if (onSelect != null)
            onSelect.SetActive(false);
        transform.localScale = defScale;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null) return;
        controller.TryClick(this);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (controller == null) return;
        controller.MoveTo(this);
    }

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    OnDeselect();
    //}
}