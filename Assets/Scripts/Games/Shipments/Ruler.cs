using System;
using Assets.Scripts.Sound;
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
        Invoke("CheckMouseDown", 0.5f);
    }

    private void CheckMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            if (Math.Abs(Vector2.Distance(Input.mousePosition, _mouseReference)) < 1)
            {
                _isFixed = !_isFixed;
                FixImage.gameObject.SetActive(_isFixed);
                RectTransform rectTransform = GetComponent<RectTransform>();
                if (_isFixed)
                {
                    SetPivot(rectTransform, Vector2.up);
                }
                else



                {
/*
                    SetPivot(rectTransform, new Vector2());
*/

/*
                    float distance = (Input.mousePosition.x, Input.mousePosition.y);
*/

                    rectTransform.rotation = Quaternion.identity;;
                    SetPivot(rectTransform, Vector2.one / 2);

                }
            }
        }
    }

    public void UnFix()
    {

        _isFixed = !_isFixed;
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (_isFixed)
        {
            SetPivot(rectTransform, Vector2.up);
        }
        else
        {
            rectTransform.rotation = Quaternion.identity; ;
            SetPivot(rectTransform, Vector2.one / 2);

        }


    }

    public void ToggleRulerVisibility()
    {
        SoundController.GetController().PlayClickSound();
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }
}
