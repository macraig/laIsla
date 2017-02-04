using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ruler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler{

    public GameObject FirstPoint;
    public GameObject SecondPoint;
    public Image FixImage;
    private float _timeDown;
    private bool _isFixed;

    private Vector3 _mouseReference;

    public float GetUnityDistances()
    {
        return Vector2.Distance(FirstPoint.transform.position, SecondPoint.transform.position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isFixed)
        {
            transform.position = Input.mousePosition;
        }
        else
        {
            Vector3 dir = Input.mousePosition - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);      
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _mouseReference = Input.mousePosition;
        Invoke("CheckMouseDown", 1f);
    }

    private void CheckMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            if (Math.Abs(Vector2.Distance(Input.mousePosition, _mouseReference)) < 1)
            {
                _isFixed = !_isFixed;
                FixImage.gameObject.SetActive(_isFixed);
            }
        }
    }

    public void UnFix()
    {
        _isFixed = !_isFixed;
    }

    public void ToggleRulerVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
