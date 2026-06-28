using System;
using UnityEngine;
public class MUINav_Controller : MonoBehaviour
{
    public bool isActive;
    //初始点
    public MUINav_Item startItem;
    public MUINav_Item nowItem;
    public MUINav_Item lastItem;

    public MUINav_Item lastClickItem;
    public MUINav_Grid nav_Grid;

    public float navigationRepeatDelay = 0.5f;
    public float navigationRepeatRate = 0.1f;
    private float nextNavTime;

    public virtual void Awake()
    {
        // Test();
    }
    public void Test()
    {
        Init();
    }
    public virtual void Init()
    {
        nav_Grid.InitGrid(10);
        SetActive(true);
        InitSelect();
    }
    public virtual void InitSelect()
    {
        if (nowItem != null)
            nowItem.OnDeselect();
        SetNowItem(startItem);
        nowItem.OnSelect();
    }
    public virtual void SetStartItem(int index)
    {
        if (index >= nav_Grid.itemList.Count || index < 0) return;
        SetStartItem(nav_Grid.itemList[index]);
    }
    public void SetStartItem(MUINav_Item item)
    {
        startItem = item;
        InitSelect();
    }
    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void SetNowItem(MUINav_Item item)
    {
        lastItem = nowItem;
        nowItem = item;
    }
    int directionIndex;
    //移动选择
    public void Move()
    {
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(inputH, inputV);

        bool canInput = false;
        if (input != Vector2.zero)
        {
            if (input.magnitude > 0.3f)
                canInput = true;
        }
        if (canInput)
        {
            directionIndex = -1;
            if (inputV != 0 && Math.Abs(inputV) > Math.Abs(inputH))
            {
                if (inputV > 0)
                    directionIndex = 0;
                else directionIndex = 2;
                MoveTo(directionIndex);
            }
            else if (inputH != 0)
            {
                if (inputH > 0)
                    directionIndex = 1;
                else directionIndex = 3;
                MoveTo(directionIndex);
            }
            nextNavTime = Time.time + (input.magnitude > 0 ? navigationRepeatRate : navigationRepeatDelay);
        }
    }
    public virtual void MoveTo(int index)
    {
        if (index == -1 || nowItem.neighbors[index] == null)
        {
            Debug.LogWarning($"无法定位，index={index}");
            return;
        }
        MoveTo(nowItem.neighbors[index]);
    }
    public virtual void MoveTo(MUINav_Item item)
    {
        if (nowItem != null)
            nowItem.OnDeselect();
        SetNowItem(item);
        nowItem.OnSelect();
    }
    //点击确认
    public virtual void TryClick()
    {
        TryClick(nowItem);
    }
    public virtual void TryClick(int index, bool update = true)
    {
        if (index >= nav_Grid.itemList.Count) return;
        if (index >= 0)
        {
            MUINav_Item item = nav_Grid.itemList[index];
            TryClick(item, update);
        }
        else
        {
            TryClick(update);
        }
    }
    public virtual void TryClick(MUINav_Item item, bool update = true)
    {
        if (item != null)
        {
            Debug.Log("unClick " + update);
            if (lastClickItem != null && item != lastClickItem)
                lastClickItem.UnClick(update);
            else if (item == lastClickItem )
                lastClickItem.UnClick(update);
            item.OnClick();
            lastClickItem = item;
        }
    }
    public virtual void TryClick(bool update = true)
    {
        if (lastClickItem != null)
            lastClickItem.UnClick(update);
    }

    public virtual void Update()
    {
        if (!isActive) return;
        if (Time.time < nextNavTime) return;
        Move();
        if (Input.GetButtonDown("Submit"))
        {
            TryClick();
        }
        OtherUpdate();
    }
    public virtual void OtherUpdate()
    {


    }
}
