using System.Collections.Generic;
using UnityEngine;

public class Previous : MonoBehaviour
{
    public static Previous Instance;

    public Transform content;
    public Dictionary<int, GameObject> dictBlocks;

    private void Awake()
    {
        Instance = this;
        
        dictBlocks = new Dictionary<int, GameObject>();
    }

    public void ShowPreview(int index)
    {
        HideAll();
        GameObject item = null;

        if (!dictBlocks.ContainsKey(index))
        {
            item = Instantiate(Playground.Instance.GetBlockList[index].Block, content.position, Quaternion.identity);
            dictBlocks.Add(index, item);
        }
        else
        {
            item = dictBlocks[index];
        }
        
        item.SetActive(true);
    }

    public void HideAll()
    {
        foreach (var b in dictBlocks)
        {
            b.Value.SetActive(false);
        }
    }
}
