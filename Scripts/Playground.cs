using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct BlockObjects
{
    public string blockName;
    public GameObject Block;
}

public class Playground : MonoBehaviour
{
    public static Playground Instance;
    
    [SerializeField] private GameObject bottomPlane;
    [SerializeField] private GameObject north, south, east, west;
    [SerializeField] private int gridX, gridY, gridZ;
    [SerializeField] private List<BlockObjects> blocksList;
    [SerializeField] private Transform[,,] blocks;
    [SerializeField] private List<Material> materials;
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private AnimatorController _clearControll;

    int randomIndexPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        blocks = new Transform[gridX, gridY, gridZ];
        RandomPrevious();
        SpawnNewBlock();
    }

    public List<BlockObjects> GetBlockList => blocksList;

    public Vector3 GetRound(Vector3 pos)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }

    public bool CheckInsidePlayground(Vector3 pos)
    {
        return (int)pos.x >= 0 && (int)pos.x < gridX &&
               (int)pos.z >= 0 && (int)pos.z < gridZ &&
               (int)pos.y >= 0; // should not be check up because blocks is moving down;
    }

    public void UpdateGrid(Block block)
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                for (int y = 0; y < gridY; y++)
                {
                    if (blocks[x, y, z] != null)
                    {
                        if (blocks[x, y, z].parent == block.transform)
                        {
                            blocks[x, y, z] = null;
                        }
                    }
                }
            }
        }

        foreach (Transform b in block.transform)
        {
            Vector3 pos = GetRound(b.position);

            if (pos.y < gridY)
            {
                blocks[(int)pos.x, (int)pos.y, (int)pos.z] = b;
            }
        }
    }

    public Transform GetTransformOnPlayground(Vector3 pos)
    {
        return pos.y > gridY - 1 ? null : blocks[(int)pos.x, (int)pos.y, (int)pos.z];
    }

    public void SpawnNewBlock()
    {
        Vector3 spawnPoint = new Vector3(
            (int)(transform.position.x + (float)gridX / 2),
            (int)transform.position.y + gridY,
            (int)(transform.position.z + (float)gridZ / 2));

        //int randomIndexPrefab = Random.Range(0, blocksList.Count);
        int randomIndexMat = Random.Range(0, materials.Count);
        
        //spawn new block
        GameObject newBlock = Instantiate(blocksList[randomIndexPrefab].Block, spawnPoint, Quaternion.identity);
       newBlock.AddComponent<Block>();

        MeshRenderer[] child = newBlock.GetComponentsInChildren<MeshRenderer>();

        foreach (var c in child)
        {
            c.material = materials[randomIndexMat];
        }
        //spawn fade block
        GameObject newFadeBlock = Instantiate(blocksList[randomIndexPrefab].Block, spawnPoint, Quaternion.identity);
        FadeBlock fadeBlock = newFadeBlock.AddComponent<FadeBlock>();
        fadeBlock.SetParent(newBlock);
        
        MeshRenderer[] childFade = newFadeBlock.GetComponentsInChildren<MeshRenderer>();

        foreach (var c in childFade)
        {
            c.material = fadeMaterial;
        }

        RandomPrevious();
        Previous.Instance.ShowPreview(randomIndexPrefab);
    }

    public void RandomPrevious()
    {
        randomIndexPrefab = Random.Range(0, blocksList.Count);
    }


    public void DeleteLayer()
    {
        int layersClear = 0;
        for (int y = gridY - 1; y >= 0; y--)
        {
            if(CheckFullLayer(y))
            {
                Debug.Log(y);
                
                layersClear++;
                //Delete
                DeleteLayerAt(y);

                //Move down one
                MoveAllLayerDown(y);
            }
        }
        
        //Update Data
        if (layersClear > 0)
        {
            GameManager.Instance.LayersCleared(layersClear);
        }
        
        //Update UI
        UIManager.Instance.UpdateGameLevel();
    }

    void DeleteLayerAt(int y)
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                //Debug.Log(blocks[x, y, z]);
                var animtor = blocks[x, y, z].AddComponent<Animator>();
                animtor.runtimeAnimatorController = _clearControll;
                animtor.Play("Clear");
                Destroy(blocks[x, y, z].gameObject, 0.5f);
                blocks[x, y, z] = null;
            }
        }
    }
    
    void MoveAllLayerDown(int y)
    {
        for (int i = y; i < gridY; i++)
        {
            MoveOneLayerDown(i);
        }
    }
    
    void MoveOneLayerDown(int y)
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (blocks[x, y, z] != null)
                {
                    blocks[x, y - 1, z] = blocks[x, y, z];
                    blocks[x, y, z] = null;
                    blocks[x, y - 1, z].position += Vector3.down;;
                }
            }
        }

    }

    bool CheckFullLayer(int y)
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (blocks[x, y, z] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void UsingSkill(int amount)
    {
        DeleteLayerAtWithSkill(amount);
        MoveAllLayerDown(amount);
    }
    
    void DeleteLayerAtWithSkill(int y)
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (blocks[x, y, z] != null)
                {
                    Destroy(blocks[x, y, z].gameObject);
                    blocks[x, y, z] = null;
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (bottomPlane != null)
        {
            Vector3 scaler = new Vector3((float)gridX / 10, 1, (float)gridZ / 10);
            bottomPlane.transform.localScale = scaler;

            bottomPlane.transform.position = new Vector3(transform.position.x + (float)gridX / 2, transform.position.y,
                transform.position.z + (float)gridZ / 2);
            bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridX, gridZ);
        }
        
        if (north != null)
        {
            Vector3 scaler = new Vector3((float)gridX / 10, 1, (float)gridY / 10);
            north.transform.localScale = scaler;

            north.transform.position = new Vector3(transform.position.x + (float)gridX / 2, transform.position.y + (float)gridY/2,
                transform.position.z + gridZ);
            north.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridX, gridY);
        }
        
        if (south != null)
        {
            Vector3 scaler = new Vector3((float)gridX / 10, 1, (float)gridY / 10);
            south.transform.localScale = scaler;

            south.transform.position = new Vector3(transform.position.x + (float)gridX / 2, transform.position.y + (float)gridY/2,
                transform.position.z);
        }
        
        if (east != null)
        {
            Vector3 scaler = new Vector3((float)gridZ / 10, 1, (float)gridY / 10);
            east.transform.localScale = scaler;

            east.transform.position = new Vector3(transform.position.x + gridX, transform.position.y + (float)gridY/2,
                transform.position.z + (float)gridZ/2);
            east.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridZ, gridY);
        }
        
        if (west != null)
        {
            Vector3 scaler = new Vector3((float)gridZ / 10, 1, (float)gridY / 10);
            west.transform.localScale = scaler;

            west.transform.position = new Vector3(transform.position.x, transform.position.y + (float)gridY/2,
                transform.position.z + (float)gridZ/2);
        }
    }
}
