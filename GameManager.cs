using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemy;
    public GameObject[] EnemyPrb;
    public float spawnRadius = 5f;


    public IEnumerator Spawn()
    {
        while (true)
        {
            if (enemy >= 5)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

                for (int i = 0; i < enemy; i++)
                {
                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                    Vector3 spawnPosition = randomPosition + randomDirection * Random.Range(0f, spawnRadius);

                    GameObject enemyObject = Instantiate(EnemyPrb[Random.Range(0, EnemyPrb.Length)]);
                    enemyObject.transform.position = spawnPosition;

                    enemyObject.AddComponent<Enemy>();

                }
                string s = Manager.Instance.UserId;
                Manager.Instance.Score(s);

                enemy = 0;

            }
            yield return null;
        }
    }
}

public class Enemy : MonoBehaviour
{
    GameManager gameManager;

    public float Hp = 20;
    public float speed = 1f;
    private Transform target;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        TakeDamage();

        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        }
    }
    public void TakeDamage()
    {
        if (Hp <= 0)
        {
            gameManager.enemy++;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Attack"))
        {
            Debug.Log("공격받음!");
            Hp -= 10;
        }
    }
}
