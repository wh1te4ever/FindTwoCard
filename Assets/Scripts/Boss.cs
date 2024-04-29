using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss instance;

    public GameObject prefab;

    public static int bossHealthMax;

    public static int bossHealthCur;

    public void SetBossHealth(int hp)
    {
        bossHealthCur = hp * GameManager.instance.round;
        bossHealthMax = hp * GameManager.instance.round;
    }

    public void BossKilled()
    {
        if (bossHealthCur <= 0) {
            GameManager.instance.round++;
            Destroy(gameObject, 1.0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetBossHealth(20);
    }

    // Update is called once per frame
    void Update()
    {
        BossKilled();
        void onDestroy()
        {
            Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        }

    }
}
