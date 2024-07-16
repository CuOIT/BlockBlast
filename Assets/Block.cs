using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    List<Box> boxes;

    RectTransform _rectTransform;
    
    private IBoardChecker _boardChecker;

    Transform _shadow;

   Vector2 _touchOffset;

    private bool placable;

    public delegate void PlaceEvent(Block block);

    public event PlaceEvent placeEvent;

   private void Start()
    {
        _touchOffset = Vector2.up * 200;
    }
    public void Init(IBoardChecker boardChecker)
    {
        _boardChecker = boardChecker;

        Transform shape = transform.GetChild(0);
        boxes = shape.GetComponentsInChildren<Box>().ToList();
        int randomColor = 0;
        if (boxes.Count > 0)
        {
            randomColor = boxes[0].GetRandomColor();
        }
        foreach (var box in boxes)
        {
            box.InitColor(randomColor);
        }
        _shadow = Instantiate(shape, transform);
        _shadow.SetSiblingIndex(0);
        foreach (var box in _shadow.GetComponentsInChildren<Box>())
        {
            box.Blur();
        }
        _rectTransform = shape.GetComponent<RectTransform>();

    }
    public bool CanPlaced()
    {
        placable = _boardChecker.Placable(boxes);
        if (!placable)
        {
            foreach(var box in boxes)
            {
                box.SetColor(0);//BRICK
            }
        }
        else
        {
            foreach(var box in boxes)
            {
                box.ReverseToOriginColor();
            }
        }
        return placable;
            
    }
    private void OnEnable()
    {
        transform.localScale = Vector3.one * 0.6f;
    }
    public void SetColor(int num)
    {
        foreach(var box in boxes)
        {
            box.InitColor(num);
        }
    }


    private Vector3 targetPos;
    private Vector3 startPos;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!placable) return;
        transform.localScale = Vector3.one;
        startPos = transform.position;
        targetPos = Camera.main.ScreenToWorldPoint(eventData.position + _touchOffset);
        targetPos.z = transform.position.z;
        transform.position = targetPos;
        //_shadow.parent = transform.parent;

    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!placable) return;
        targetPos = Camera.main.ScreenToWorldPoint(eventData.position + _touchOffset);
        targetPos.z = transform.position.z;
        transform.position = Vector3.Slerp(transform.position, targetPos, 0.9f);
        if(_boardChecker.Placable(Camera.main.WorldToScreenPoint(_rectTransform.position),boxes,out Vector3 placablePos))
        {
            _shadow.gameObject.SetActive(true);
            _shadow.position = placablePos;
        }
        else
        {
            _shadow.gameObject.SetActive(false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!placable) return;
        _shadow.gameObject.SetActive(false);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(_rectTransform.position);
        if (_boardChecker.Placable(screenPos, boxes, out Vector3 placablePos))
        {
            placeEvent.Invoke(this);
            _boardChecker.Place(screenPos, boxes);
            Destroy(gameObject);
        }
        else
        {
            _boardChecker.Place(screenPos, null);
            transform.DOMove(startPos, 0.2f);
            transform.localScale = Vector3.one * 0.6f;
        }
    }
}
