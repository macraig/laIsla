using Assets.Scripts.Games.Shipments;
using UnityEngine;
using UnityEngine.UI;

public class MapPlace : MonoBehaviour
{
    public Image CrossReference;
    private int _id;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public ShipmentNodeType Type { get; set; }

    void Start()
    {
        Button.ButtonClickedEvent click = GetComponent<Button>().onClick;
         click.AddListener(OnClickMapPlace);
    }

    public void SetData(int id, Sprite placesSprite, Sprite crossSprite, ShipmentNodeType type)
    {
        this.Id = id;
        CrossReference.sprite = crossSprite;
        GetComponent<Image>().sprite = placesSprite;
        Type = type;
    }

    private void OnClickMapPlace()
    {
        ShipmentsView.instance.OnClickMapPlace(_id, Type);
    }
}