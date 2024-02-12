using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Card _card;
    [SerializeField] private float _cardZOffset;
    [SerializeField] private Camera _camera;
    [SerializeField] private bool _draggable;
    [SerializeField] private AutoLayout3D.LayoutElement3D _layoutElement;
    private Vector2 _currentPositionOffset;

    public bool Draggable => _draggable;

    private void OnValidate()
    {
        if (!_card) _card = GetComponent<Card>();
        if (!_layoutElement) _layoutElement = GetComponent<AutoLayout3D.LayoutElement3D>();
        if (!_camera) _camera = FindObjectOfType<Camera>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        transform.parent = null;
        DestroyImmediate(_layoutElement);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = _camera.ScreenToWorldPoint(new Vector3(eventData.pointerCurrentRaycast.screenPosition.x, eventData.pointerCurrentRaycast.screenPosition.y, 120));
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
