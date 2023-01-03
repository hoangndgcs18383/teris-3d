using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public static UIButton Instance;
    
    [SerializeField] private GameObject activeBlock;
    [SerializeField] private Block activeTetris;

    private void Awake()
    {
        Instance = this;
    }

    void SetToActiveBlock()
    {
        if (activeBlock != null)
        {
            transform.position = activeBlock.transform.position;
        }
    }

    public void SetActiveBlock(GameObject block, Block tBlock)
    {
        activeBlock = block;
        activeTetris = tBlock;
    }

    private void Update()
    {
        SetToActiveBlock();
    }

    public void OnHighSpeed()
    {
        activeTetris.SetSpeed();
    }
}
