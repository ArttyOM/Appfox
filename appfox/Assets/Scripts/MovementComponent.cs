using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


using static Allocator;

public class MovementComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{ 
    public Vector3Int cellCoords;
    public Transform CachedTransform { get; private set; }


    private void Awake()
    {
       

        CachedTransform = this.transform;

        _myImage = GetComponent<SpriteRenderer>();

        if (!grid) { FindGrid(); };

        cellCoords = grid.WorldToCell(CachedTransform.position);
        //Debug.Log(cellCoords);

        MoveableObjects.Add(cellCoords, this);
    }


    private void OnDestroy()
    {
        MoveableObjects.Remove(cellCoords);
    }

    Vector3 mouseInput = new Vector3();

    private SpriteRenderer _myImage;

    public bool dragOnSurfaces = false;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    public Canvas canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
       // Debug.Log("OnBeginDrag");

        if (!_myImage) return;

        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        if (canvas == null) return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        m_DraggingIcon = new GameObject("icon");

        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();

        image.sprite = GetComponent<SpriteRenderer>().sprite;
        image.SetNativeSize();
        image.raycastTarget = false;

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        //Vector3 globalMousePos;
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(/*m_DraggingPlane*/, data.position, data.pressEventCamera, out globalMousePos))
        //{
        //    rt.position = globalMousePos;
        //    rt.rotation = m_DraggingPlane.rotation;
        //}
        var tmp  = Input.mousePosition;
        mouseInput.x = tmp.x;/// вот кто мне объяснит, почему я это делаю?
        mouseInput.y = tmp.y;
        mouseInput.z = 0;

        //Debug.Log(mouseInput);
        rt.position = mouseInput;//Input.mousePosition;

        //Debug.Log("SetDraggedPosition to" + globalMousePos +" "+  rt.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        //  Event currentEvent = Event.current;
        //Vector2 mousePos = eventData.position;// currentEvent.mousePosition;
        //Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
       // Debug.Log(mouseInput);
        var cellUnderMouse = MouseToCellPos(mouseInput);

        if (!Allocator.MoveableObjects.ContainsKey(cellUnderMouse))
        {
            //Debug.Log("смена позиции");

            Allocator.MoveableObjects.Remove(this.cellCoords);

            cellCoords = cellUnderMouse;
            Allocator.MoveableObjects.Add(this.cellCoords, this);

            var tmp = grid.CellToWorld(cellCoords);
            //tmp.x += grid.cellSize.x / 2;
            tmp.y += grid.cellSize.y /2 ;
            this.CachedTransform.position =tmp;

        }
        else { Debug.Log("Обнаружено препятствие"); }


        if (m_DraggingIcon != null)
            Destroy(m_DraggingIcon);

    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>позиция элемента сетки, над которым находится мышка</returns>
    public static Vector3Int MouseToCellPos(Vector3 mousePos)
    {

        //Debug.Log(mousePos);
       // Event currentEvent = Event.current;
       // Vector2 mousePos = currentEvent.mousePosition;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);

        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //pos.z = 0;

        //Debug.Log(pos);
        pos.z = 0;
        var cellPos = grid.WorldToCell(pos);

        //Debug.Log("позиции "+ mousePos + " соответствует ячейка:"+cellPos);

        return cellPos;
    }

}
