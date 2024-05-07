using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss instance;

    public GameObject prefab;

    public static int bossHealthMax;

    public static int bossHealthCur;

    private SpriteRenderer bossColor;

    public void SetBossHealth(int hp)
    {
        bossHealthCur = hp * GameManager.instance.round;
        bossHealthMax = hp * GameManager.instance.round;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetBossHealth(20);

        bossColor = GetComponent<SpriteRenderer>();

        Color currentColor = bossColor.color;

        currentColor.g -= 0.05f * GameManager.instance.round;
        currentColor.b -= 0.05f * GameManager.instance.round;

        bossColor.color = currentColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (bossHealthCur <= 0)
        {
            GameManager.instance.round++;
            Destroy(gameObject, 1.0f);
            Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        }
    }
}