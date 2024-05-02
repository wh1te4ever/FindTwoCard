using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{

    [SerializeField]
    public SpriteRenderer healthRenderer;

    [SerializeField]
    private Sprite healthSprite;

    [SerializeField]
    public Sprite unfilledHealthSprite;
    public Sprite filledHealthSprite;

    public int healthID;

    //public int healthCount = 5;

    

    public void SetHealthID(int id) {
        healthID = id;
    }

    public void SetHealthUnfilledSprite() {
        healthRenderer.sprite = unfilledHealthSprite;
    }

    public void SetHealthfilledSprite() {
        healthRenderer.sprite = filledHealthSprite;
    }

}
