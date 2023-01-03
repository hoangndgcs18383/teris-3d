using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private float fallTime;
    private float prevTime;
    public bool IsUsingRotate = false;

    public float SetFallTime
    {
        get => fallTime;
        set => fallTime = value;
    }

    private void Start()
    {
         //StartCoroutine(IEMoveDownBlock(fallTime));
         UIButton.Instance.SetActiveBlock(gameObject, this);
         fallTime = GameManager.Instance.GetFallSpeed;

         if (!CheckValidMove())
         {
             GameManager.Instance.SetGameIsOver();
         }
    }

    private void Update()
    {
        if (Time.time -prevTime > fallTime)
        {
            transform.position += Vector3.down;
            
            if (!CheckValidMove())
            {
                transform.position += Vector3.up;
                //Delete first
                Playground.Instance.DeleteLayer();
                
                enabled = false;
                if(!GameManager.Instance.GetGameOver) Playground.Instance.SpawnNewBlock();
            }
            else
            {
                Playground.Instance.UpdateGrid(this);
            }

            prevTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetInput(Vector3.forward);
        }
        
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetInput(Vector3.back);
        }
        
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetInput(Vector3.left);
        }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetInput(Vector3.right);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetRotateInput(new Vector3(90f, 0f, 0f));
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetSpeed();
        }
    }

    IEnumerator IEMoveDownBlock(float time)
    {
        while (CheckValidMove())
        {
            
            transform.position += Vector3.down;
            yield return new WaitForSeconds(time);
        }
        
        if (!CheckValidMove())
        {
            enabled = false;
            
            Playground.Instance.SpawnNewBlock();
        }
        else
        {
            Playground.Instance.UpdateGrid(this);
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
            if (t != null && t.parent != transform)
            {
                return false;
            }
        }
        
        return true;
    }

    public void SetInput(Vector3 dir)
    {
        transform.position += dir;
        
        if (!CheckValidMove())
        {
            transform.position -= dir;
        }
        else
        {
            Playground.Instance.UpdateGrid(this);
        }
    }

    public void SetRotateInput(Vector3 rotate)
    {
        transform.Rotate(rotate, Space.World);
        
        if (!CheckValidMove())
        {
            transform.Rotate(-rotate, Space.World);
        }
        else
        {
            Playground.Instance.UpdateGrid(this);
        }
    }

    public void SetSpeed()
    {
        fallTime = 0.1f;
    }
}
