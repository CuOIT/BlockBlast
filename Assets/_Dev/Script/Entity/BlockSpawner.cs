using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static UnityEditor.PlayerSettings;
using static Unity.Collections.AllocatorManager;

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
    [Range(3,7)]
    [SerializeField] int randomCount;
    public void GetRandomBlockAdvanced()
    {
            state = new();
            blockNum = poss.Count;
            List<Block> selectableBlocks = new List<Block>(blocks);
            List<Block> selectedBlocks = new List<Block>();
            List<Block> placableBlocks = new List<Block>();

            for(int i = selectableBlocks.Count-1; i >=0 ; i--)
            {
                Block block = selectableBlocks[i];
                bool canMakeRowOrCol = false;
                if (block.CanPlaceAndMakeRowOrCol(out canMakeRowOrCol, _boardChecker))
                {
                    if (canMakeRowOrCol)
                    {
                        selectedBlocks.Add(block);
                        selectableBlocks.RemoveAt(i);
                        if (selectableBlocks.Count >= blockNum)
                            break;
                    }
                    else
                    {
                        placableBlocks.Add(block);
                        selectableBlocks.RemoveAt(i);

                    }
                }

            }
       
            #region pick 1 block that make Row or Col
            if(selectedBlocks.Count > 0)
            {
                int rand = Random.Range(0, selectedBlocks.Count);
                Block block = selectedBlocks[rand];
                foreach(Block b in selectedBlocks)
                {
                    if (b != block)
                    {
                        placableBlocks.Add(b);
                    }
                }

                selectedBlocks.Clear();
                selectedBlocks.Add(block);
                /*Block block = Instantiate(blocks[rand], poss[i]);
                state.Add(block.ToString());
                RectTransform rect = block.GetComponent<RectTransform>();
                rect.anchoredPosition += Screen.width * Vector2.right;
                block.transform.DOMove(poss[i].position, 0.3f);
                block.Init(_boardChecker);
                block.placeEvent += RemoveBlock;
                block.CanPlaced();
                currentBlocks[poss[i]] = block;*/
            }
            #endregion
        while (selectableBlocks.Count > 0 && randomCount >= 0){
                int rand = Random.Range(0, selectableBlocks.Count);
                Block block = selectableBlocks[rand];
                selectableBlocks.RemoveAt(rand);
                placableBlocks.Add(block);
                randomCount--;
            };
        while (selectedBlocks.Count < blockNum)
        {
            int rand = Random.Range(0, placableBlocks.Count);
            Block block = placableBlocks[rand];
            placableBlocks.RemoveAt(rand);
            selectedBlocks.Add(block);
        }

        for(int i = 0; i < blockNum; i++)
        {
            Block block = Instantiate(selectedBlocks[i], poss[i]);
            state.Add(block.ID.ToString());
            RectTransform rect = block.GetComponent<RectTransform>();
            rect.anchoredPosition += Screen.width * Vector2.right;
            block.transform.DOMove(poss[i].position, 0.3f);
            block.Init(_boardChecker);
            block.placeEvent += RemoveBlock;
            block.CanPlaced();
            currentBlocks[poss[i]] = block;
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
            GetRandomBlockAdvanced();
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

