using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Mark : MonoBehaviour {
    public SpriteRenderer X;
    public SpriteRenderer O;
    public SpriteRenderer box;

    public static string sendInfo;

    public void OnMouseDown()
    {
        if(box.sprite == null)
        {
            if (GameManager.round % 2 == 0)
            {
                box.sprite = X.sprite;
                Debug.Log("X");
                GameManager.round++;
            }
            else
            {
                box.sprite = O.sprite;
                Debug.Log("O");
                GameManager.round++;
            }

            sendInfo = this.gameObject.name;
        }
        else
        {
            Debug.Log("this slot is already Taken");
        }
        
        //else if(itsPlayer2)
        //box.sprite = O.sprite;
        //Debug.Log("O")
    }
}
