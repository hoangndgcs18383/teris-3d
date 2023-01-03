using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlock : MonoBehaviour
{
    private GameObject parent;
    private Block block;

    private void Start()
    {
        StartCoroutine(IEDoProjectionBlock());
    }

    public void SetParent(GameObject _parent)
    {
        parent = _parent;
        block = parent.GetComponent<Block>();
    }

    IEnumerator IEDoProjectionBlock()
    {
        while (block.enabled)
        {
            SetPositionFade();
            //move end
            MoveToEnd();
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
        yield return null;
    }

    void SetPositionFade()
    {
        transform.position = parent.transform.position;
        transform.rotation = parent.transform.rotation;
    }

    void MoveToEnd()
    {
        while (CheckValidMove())
        {
            transform.position += Vector3.down;
        }

        if (!CheckValidMove())
        {
            transform.position += Vector3.up;
        }
    }

    bool CheckValidMove()
    {
        foreach (Transform c in transform)
        {
            Vector3 pos = Playground.Instance.GetRound(c.position);
            if (!Playground.Instance.CheckInsidePlayground(pos))
            {
                return false;
            }
        }
        
        foreach (Transform c in transform)
        {
            Vector3 pos = Playground.Instance.GetRound(c.position);
            Transform t = Playground.Instance.GetTransformOnPlayground(pos);

            if (t != null && t.parent == parent.transform)
            {
                return true;
            }
            
            if (t != null && t.parent != transform)
            {
                return false;
            }
        }
        
        return true;
    }
}
