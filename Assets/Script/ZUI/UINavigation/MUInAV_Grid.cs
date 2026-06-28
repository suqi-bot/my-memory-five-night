using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
[ExecuteAlways]
//[RequireComponent(typeof(GridLayoutGroup))]
public class MUINav_Grid : MonoBehaviour
{
    [Tooltip("列数")]//，不填则自动从GridLayoutGroup.constraintCount读取
    public int columns = 0;
    
    public GameObject perfab;
    public MUINav_Controller nav_Controller;
    public List<MUINav_Item> itemList = new();

    private void Awake()
    {
        itemList=GetComponentsInChildren<MUINav_Item>().ToList();
    }
    public void InitGrid(int count)
    {
        for (int i = transform.childCount; i < count; i++)
            itemList.Add(Instantiate(perfab, transform).GetComponent<MUINav_Item>());
        while (itemList.Count > count)
        {
            int index = itemList.Count - 1;
            Destroy(itemList[index].gameObject);
            itemList.RemoveAt(itemList.Count - 1);
        }
        InitAllNeighbors();
    }
    
    
    public void InitGrid()
    {
        itemList=GetComponentsInChildren<MUINav_Item>().ToList();
        InitAllNeighbors();
    }// <summary>
    /// 初始化所有 MUINav_Item 的邻居
    /// </summary>
    public void InitAllNeighbors()
    {
        // 自动获取列数
        //var grid = GetComponent<GridLayoutGroup>();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(grid.transform as RectTransform);
        //if (columns <= 0)
        //{
        //    if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        //    {
        //        columns = grid.constraintCount;
        //    }
        //    else
        //    {
        //        // 动态计算列数
        //        float parentWidth = (grid.transform as RectTransform).rect.width;
        //        float cellWidth = grid.cellSize.x + grid.spacing.x;
        //        columns = Mathf.Max(1, Mathf.FloorToInt((parentWidth + grid.spacing.x) / cellWidth));
        //    }
        //}

     
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Init(nav_Controller);
            int row = i / columns;
            int col = i % columns;
            //Debug.Log($"{i}: row={row}, col={col}, item={itemList[i].name}");
            // 顺时针：上、右、下、左
            itemList[i].neighbors[0] = GetItemAt(itemList, row - 1, col, columns); // 上
            itemList[i].neighbors[1] = GetItemAt(itemList, row, col + 1, columns); // 右
            itemList[i].neighbors[2] = GetItemAt(itemList, row + 1, col, columns); // 下
            itemList[i].neighbors[3] = GetItemAt(itemList, row, col - 1, columns); // 左
        }
    }
    private MUINav_Item GetItemAt(List<MUINav_Item> items, int row, int col, int columns)
    {
        if (row < 0 || col < 0) return null;

        int index = row * columns + col;

        // 越界检查
        if (index >= items.Count) return null;

        // 额外检查：这一行是否真的有这个列
        int maxColThisRow = Mathf.Min(columns, items.Count - row * columns);
        if (col >= maxColThisRow) return null;

        return items[index];
    }
}
