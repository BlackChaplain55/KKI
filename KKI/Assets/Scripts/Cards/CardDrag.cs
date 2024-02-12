using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Этот компонент предназначен для перетаскивания карт, в текущей версии прототипа не используется

public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Card _card;
    [SerializeField] private float _cardZOffset;
    [SerializeField] private Camera _camera;
    [SerializeField] private bool _draggable;
    [SerializeField] private AutoLayout3D.LayoutElement3D _layoutElement;

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
}
