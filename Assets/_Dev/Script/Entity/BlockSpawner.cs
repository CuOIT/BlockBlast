using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class BlockSpawner : MonoBehaviour,IEntity
{
    [SerializeField] BlockDic blockDic;
    List<Block> blocks;

    [SerializeField]List<RectTransform> poss;

    Dictionary<RectTransform, Block> currentBlocks;

    [SerializeField]GameObject Board;
    IBoardChecker _boardChecker;

    [SerializeField]List<string> state;

    [SerializeField] Event noPosToPlaceEvent;
    const string BLOCKSPAWNER = "BLOCKSPAWNER";

    private int blockNum;
    public void Awake()
    {
        state = new();
        _boardChecker = Board.GetComponent<IBoardChecker>();
        blocks = blockDic.GetBlockList();
        currentBlocks = new();
        foreach(var pos in poss)
        {
            currentBlocks.Add(pos, null);
        }
    }
    public void ResetState()
    {
        foreach(var pos in poss)
        {
            Destroy(currentBlocks[pos].gameObject);
            currentBlocks[pos] = null;
        }
    }
    [Button]
    public void GetRandomBlock()
    {
        int num = blocks.Count;
        blockNum=poss.Count;
        state = new();
        foreach(var pos in poss)
        {
            int rand = Random.Range(0, num);
            state.Add(rand.ToString());
            Block block = Instantiate(blocks[rand], pos);
            RectTransform rect = block.GetComponent<RectTransform>();
            rect.anchoredPosition += Screen.width * Vector2.right;
            block.transform.DOMove(pos.position, 0.3f);
            block.Init(_boardChecker);
            block.placeEvent += RemoveBlock;
            block.CanPlaced();
            currentBlocks[pos] = block;
        }

    }
    public void RemoveBlock(Block block)
    {
        for(int i = 0; i < poss.Count; i++)
        {
            if (currentBlocks[poss[i]] == block)
            {
                state.RemoveAt(i);
                state.Add("");
                block.placeEvent -= RemoveBlock;
                currentBlocks[poss[i]]= null;
                for(int j=i+1; j<poss.Count; j++)
                {
                    if (currentBlocks[poss[j]] != null)
                    {
                        currentBlocks[poss[j]].transform.DOMove(poss[j-1].transform.position, 0.5f);
                        currentBlocks[poss[j]].transform.parent = poss[j-1].transform;
                    }
                }
            }
        }
    }
    public void OnBlockPlaced()
    {
        blockNum--;
        if(blockNum <= 0)
        {
            GetRandomBlock();
        }
        bool canPlace = false;
        foreach(var pos in poss)
        {
            if (currentBlocks[pos] != null)
                if (currentBlocks[pos].CanPlaced())
                {
                    canPlace = true;
                }
        }
        if (!canPlace)
        {
            noPosToPlaceEvent.RaiseEvent();
        }
    }

    public void LoadProcess(string gameMode)
    {
        if (PlayerPrefs.HasKey(BLOCKSPAWNER+gameMode))
        {
            blockNum = 0;
            string json = PlayerPrefs.GetString(BLOCKSPAWNER+gameMode);
            state =  JsonConvert.DeserializeObject<List<string>>(json);
            for (int i = 0; i < poss.Count; i++)
            {
                string blockID = state[i];
                if (!string.IsNullOrEmpty(blockID))
                {
                    int blockId = int.Parse(blockID);
                    Block block = blocks[blockId];
                    if (block != null)
                    {
                        blockNum++;
                        Block newBlock = Instantiate(block, poss[i]);
                        newBlock.Init(_boardChecker);
                        newBlock.placeEvent += RemoveBlock;
                        newBlock.CanPlaced();
                        currentBlocks[poss[i]] = newBlock;
                    }
                }
                else
                {
                    currentBlocks[poss[i]] = null;
                }
            }
        }
        else
        {
            GetRandomBlock();
        }
    }

    public void SaveProcess(string gameMode)
    {
        string json = JsonConvert.SerializeObject(state);
        PlayerPrefs.SetString(BLOCKSPAWNER+gameMode, json);
    }

    public void ResetProcess(string gameMode)
    {
        foreach(var pos in poss)
        {
            Block block = currentBlocks[pos];
            if (block != null)
            {
                currentBlocks[pos] = null;
                Destroy(block.gameObject);
            }
        }
        blockNum = 0;
        for(int i=0;i<poss.Count;i++)
        {
            state[i] = "";
        }
        PlayerPrefs.DeleteKey(BLOCKSPAWNER+gameMode);   
    }
}

public class BlockSpawnerState
{
    public List<string> blockIDs;
    public BlockSpawnerState(int size)
    {
        blockIDs= new List<string>(new string[size]);
    }
}