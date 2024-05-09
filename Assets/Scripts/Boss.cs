using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss instance;

    public GameObject prefab;

    public static int bossHealthMax;

    public static int bossHealthCur;

    public bool enableHardMode = false;
    // private SpriteRenderer bossColor;

    public void SetBossHealth(int hp)
    {
        if (enableHardMode) {
            bossHealthCur = hp * GameManager.instance.round;
            bossHealthMax = hp * GameManager.instance.round;
        }
        else {
            bossHealthCur = hp + (5 * GameManager.instance.round);
            bossHealthMax = hp + (5 * GameManager.instance.round);
        }
        Debug.Log(bossHealthMax);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (enableHardMode) SetBossHealth(10);
        else SetBossHealth(15);

        /* bossColor = GetComponent<SpriteRenderer>();

        Color currentColor = bossColor.color;

        currentColor.g -= 0.05f * GameManager.instance.round;
        currentColor.b -= 0.05f * GameManager.instance.round;

        bossColor.color = currentColor; */
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.updateBossHealthBar();
        if (bossHealthCur <= 0)
        {
            GameManager.instance.round++;
            Destroy(gameObject, 1.0f);
            Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        }
    }
}