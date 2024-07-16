using Assets._Dev.ScriptableObject.Event;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Board : MonoBehaviour, IBoardChecker,IEntity
{

    [SerializeField] int _boardWidth;
    [SerializeField] int _boardHeight;
    [SerializeField] float boxSize;
    [SerializeField] RectTransform _basePosition;
    
    [SerializeField] ulong _boardState;
    List<List<Box>> _boxGrid;
    [SerializeField] Box boxPrefab;

    [SerializeField]Vector2IntEvent clearRowAndColEvent;
    [SerializeField]IntEvent        placeBoxesEvent;
    [SerializeField]Event           PlaceEvent;
    const string BOARD = "BOARD";


    [ShowInInspector, TableMatrix(HorizontalTitle = "Board State", SquareCells = true)]
    private bool[,] BoardState
    {
        get
        {
            bool[,] board = new bool[_boardHeight, _boardWidth];
            for(int y = 0; y < _boardHeight; y++)
            {
                for(int x = 0; x < _boardWidth; x++)
                {
                    board[x,_boardHeight-1-y] = (_boardState & (1UL << (x+y*_boardWidth))) != 0;
                }
            }
            return board;
        }
        set
        {
            _boardState = 0;
            for (int i = 0; i < 64; i++)
            {
                if (value[i / 8, i % 8])
                {
                    _boardState |= (1UL << i);
                }
            }
        }
    }
    private void Awake()
    {

        _boxGrid = new List<List<Box>>();
        for(int i = 0; i < _boardHeight; i++)
        {
            List<Box> boxes = new List<Box>();
            for (int j = 0; j < _boardWidth; j++) {
                boxes.Add(null);
            }
            _boxGrid.Add(boxes);
        }
    }
    private Vector2Int ScreenPosToBoardPos(Vector2 screenPos)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_basePosition, screenPos, Camera.main, out localPos);
        Vector2Int pos = new Vector2Int((int)(localPos.x/ boxSize+0.5f), (int)(localPos.y / boxSize+0.5f));
        return pos;
    }
    private void ReverseColor()
    {
        foreach(var boxes in _boxGrid)
        {
            foreach(var box in boxes)
            {
                if (box != null)
                {
                    box.ReverseToOriginColor();
                }
            }
        }
    }
    public bool IsEmptyPos(int x,int  y)
    {
        ulong temp = _boardState >> (x + y * _boardWidth);
        return (temp & 1 )==0;
    }
    public bool Placable(Vector2 screenPos, List<Box> boxes,out Vector3 placablePos)
    {
        Vector2Int boardPos = ScreenPosToBoardPos(screenPos);
        ReverseColor();
        if(Placable(boardPos, boxes))
        {
            #region visualize row or column
            int color = boxes[0].OrginColor;
            int offset = boardPos.x+boardPos.y*_boardWidth;
            ulong boxValue = GetBoxesValue(boxes);
            boxValue <<= offset;
            ulong tempState = _boardState | boxValue;
            ulong rowsClear = GetRowClear(tempState);
            SetRowColor(rowsClear,color);
            ulong colsClear = GetColClear(tempState);
            SetColColor(colsClear,color);
            #endregion

            #region return placablePos to place Shadow
            Vector2 prePos = _basePosition.anchoredPosition;
            _basePosition.anchoredPosition += new Vector2(boardPos.x * boxSize, boardPos.y * boxSize);
            placablePos = _basePosition.position;
            _basePosition.anchoredPosition = prePos;
            #endregion

            return true;
        }
        else
        {
            placablePos = default(Vector3);
            return false;
        }
    }
    protected virtual ulong GetRowClear(ulong state)
    {
        int tempRow = 0;
        ulong entireRowI = (ulong)(Mathf.Pow(2, _boardWidth) - 1);
        for (int i = 0; i < _boardHeight; i++)
        {
          
            if ((state & entireRowI) == entireRowI)
            {
                tempRow += 1 << i;
            }
            entireRowI <<= _boardWidth;
        }
        return (ulong)tempRow;
    }
    protected virtual ulong GetColClear(ulong state)
    {
        ulong entireColI = 0;
        int tempCol = 0;
        for (int j = 0; j < _boardHeight; j++)
        {
            entireColI += (ulong)Mathf.Pow(2, j*_boardWidth);
        }
        for (int i = 0; i < _boardWidth; i++)
        {
            if ((state & entireColI) == entireColI)
            {
                tempCol += 1 << i;
            }
            entireColI <<= 1;
        }
        return (ulong)tempCol;
    }
    private int GetNum(ulong rowOrColClear)
    {
        int count = 0;
        while (rowOrColClear != 0)
        {
            count = (int)(rowOrColClear & 1);
            rowOrColClear >>= 1;
        }
        return count;
    }
    public bool Placable(Vector2Int pos, List<Box> boxes)
    {
        int offset = pos.x + _boardWidth * pos.y;


        # region if pos is invalid
        if (pos.x < 0 || pos.x>=_boardWidth) return false;
        if (pos.y < 0 || pos.y>=_boardHeight) return false;
        #endregion

        # region if a part of boxes out of board
        ulong boxesValue = GetBoxesValue(boxes,out int boxesWidth,out int boxesHeight);

        if (pos.x+boxesWidth > _boardWidth) return false;
        if(pos.y+boxesHeight > _boardHeight) return false;
        #endregion

        boxesValue <<= offset;

        return (boxesValue & _boardState)==0;// true if all pos to place are free;
    }

    public bool Placable(List<Box> boxes)
    {
        for(int i = 0; i < _boardHeight; i++)
            for(int j=0;j< _boardWidth; j++)
        {
                if (Placable(new Vector2Int(i, j), boxes)) return true;
        }
        return false;
    }

    public void PlaceBox(Box box, Vector2Int pos)
    {
        box.transform.parent = transform;
        RectTransform rectTf = box.GetComponent<RectTransform>();
        rectTf.anchoredPosition = _basePosition.anchoredPosition + boxSize * (Vector2)pos;
        _boxGrid[pos.x][pos.y] = box;
    }
    public void Place(Vector2 screenPos, List<Box> boxes)
    {
        if (boxes == null)
        {
            ReverseColor();
        }
        else { 
            Vector2Int boardPos = ScreenPosToBoardPos(screenPos);
            placeBoxesEvent.RaiseEvent(boxes.Count);
            foreach (var box in boxes)
            {
                Vector2Int boardPosOfSingleBox = boardPos + box.GetLocalPos();
                PlaceBox(box, boardPosOfSingleBox);
                _boardState |= (ulong)(1UL << (boardPosOfSingleBox.x + boardPosOfSingleBox.y * _boardWidth));

            }
            int color = boxes[0].OrginColor;
            ulong rowsClear = GetRowClear(_boardState);
            ulong colsClear = GetColClear(_boardState);
  
            if(rowsClear != 0 || colsClear != 0)
            {
                clearRowAndColEvent.RaiseEvent(new Vector2Int(GetNum(rowsClear), GetNum(colsClear)));
                SetRowColor(rowsClear, color);
                SetColColor(colsClear, color);
                for(int y = 0; y < _boardHeight; y++)
                {
                    for(int x = 0; x < _boardWidth; x++)
                    {
                        ulong mask = ~(1UL<<(x+y*_boardWidth));
                        if (((rowsClear >> y) & 1 )== 1)
                        {
                            _boardState &= mask;
                            if (_boxGrid[x][y] != null)
                            {
                                StartCoroutine(ClearBox(_boxGrid[x][y],x));
                                _boxGrid[x][y] = null;
                            }
                        }
                        if(((colsClear>>x) & 1) == 1)
                        {
                            _boardState &= mask;
                            if (_boxGrid[x][y] != null)
                            {
                                StartCoroutine(ClearBox(_boxGrid[x][y],y));
                                _boxGrid[x][y]=null;    
                            }
                        }
                    }
                }
            }
             PlaceEvent.RaiseEvent();
        }

    }
    [SerializeField] float delay;
    public IEnumerator ClearBox(Box box,int pos)
    {
        yield return new WaitForSeconds(pos*delay);
        box.Disappear();

    }
    public void SetRowColor(ulong row, int color)
    {
        for(int y=0;y < _boardHeight; y++)
        {
            if (((row >> y) & 1)==1)
            {
                for(int x = 0;x<_boardWidth;x++)
                {
                    Box box = _boxGrid[x][y];
                    if (box != null)
                    {
                        box.SetColor(color);
                    }
                }
            }
        }
    }
    public void SetColColor(ulong col, int color)
    {
        for(int x=0;x < _boardWidth; x++)
        {
            if (((col >> x) & 1)==1)
            {
                foreach(var box in _boxGrid[x])
                {
                    if (box != null)
                    {
                        box.SetColor(color);
                    }
                }
            }
        }
    }
    private ulong GetBoxesValue(List<Box> boxes)
    {
        return GetBoxesValue(boxes, out _,out _);
    }
    private ulong GetBoxesValue(List<Box> boxes,out int width,out int height)
    {
        width = 0;
        height=0;
        ulong value = 0;

        foreach (var box in boxes)
        {
            Vector2Int localPos = box.GetLocalPos();
            int x = localPos.x;
            int y = localPos.y;
            width = Mathf.Max(width, x + 1);
            height = Mathf.Max(height, y + 1);
            value += (1UL << x + _boardWidth * y);
        }

        return (ulong)value;
    }

    public void LoadProcess(string gameMode)
    {
        string state = PlayerPrefs.GetString(BOARD + gameMode, "0");
        if (ulong.TryParse(state, out _boardState))
        {
#if UNITY_EDITOR
            Debug.Log(_boardState);
#endif
        }
        else
        {
            _boardState = 0;
        }
        for(int y = 0; y < _boardHeight; y++)
        {
            for(int x=0;x<_boardWidth; x++)
            {
                if (!IsEmptyPos(x, y))
                {
                    Box box = Instantiate(boxPrefab, transform);
                    box.InitColor(1);
                    PlaceBox(box,new Vector2Int(x, y));
                }
            }
        }
    }

    public void SaveProcess(string gameMode)
    {
        string state = _boardState.ToString();

        PlayerPrefs.SetString(BOARD + gameMode, state);
    }

    public void ResetProcess(string gameMode)
    {
        _boardState = 0;
        SaveProcess(gameMode);
        foreach (var boxRow in _boxGrid)
        {
            for (int i = 0; i < boxRow.Count; i++)
            {
                Box box = boxRow[i];
                if (box != null)
                {
                    Destroy(box.gameObject);
                    box = null;
                }
            }
        }
    }
}

public interface IBoardChecker
{
    public bool Placable(Vector2 screenPos, List<Box> boxes,out Vector3 placablePos);

    public bool Placable(List<Box> boxes);

    public void Place(Vector2 screenPos, List<Box> boxes);
}