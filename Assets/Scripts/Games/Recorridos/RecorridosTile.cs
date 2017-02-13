using Assets.Scripts.Games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace Assets.Scripts.Games.Recorridos
{
public class RecorridosTile {

    private RecorridosController.RecorridosTileEnum type;

    private Vector3 position;
    private Sprite sprite;

    private int gridPositionX;
    private int gridPositionY;

    private Image tileImage;
    public RecorridosTile(RecorridosController.RecorridosTileEnum type, Vector3 currentPosition, Sprite sprite, Image tileImage, int gridPositionX, int gridPositionY)
    {
        this.type = type;
        this.position = currentPosition;
        this.sprite = sprite;
        this.tileImage = tileImage;
        this.gridPositionX = gridPositionX;
        this.gridPositionY = gridPositionY;
        tileImage.sprite = sprite;

    }

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
        set
        {
            sprite = value;
            tileImage.sprite = sprite;
        }

    }

    public Vector3 Position
    {
        get
        {
            return position;
        }
    }

    public RecorridosController.RecorridosTileEnum Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
       
    }

    public int GridPositionX
    {
        get
        {
            return gridPositionX;
        }

        set
        {
            gridPositionX = value;
        }
    }

    public int GridPositionY
    {
        get
        {
            return gridPositionY;
        }

        set
        {
            gridPositionY = value;
        }
    }

    internal void RunAction()
    {
        switch (type)
        {
            case (RecorridosController.RecorridosTileEnum.Path):
                RecorridosController.instance.MovePuppet();
                break;
            case (RecorridosController.RecorridosTileEnum.End):
                RecorridosController.instance.GameOver(true);
                break;
            case RecorridosController.RecorridosTileEnum.Nut:
                RecorridosController.instance.PickNut(gridPositionX,gridPositionY);
                RecorridosController.instance.MovePuppet();
                break;
            case RecorridosController.RecorridosTileEnum.Fire:
				RecorridosController.instance.GetBurnt();
                break;
			case RecorridosController.RecorridosTileEnum.Bomb:
				RecorridosController.instance.Explode ();

                break;
            case RecorridosController.RecorridosTileEnum.Hole:
				RecorridosController.instance.FallInHole();
                break;
            default:
                RecorridosController.instance.MovePuppet();
                break;
        }
    }
	}
}

