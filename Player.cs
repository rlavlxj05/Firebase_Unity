using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool check;
    public float attackRange;
    public GameObject attackArea;
    public Transform target;
    public int speed = 1;

    public IEnumerator System()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
            bool enemyInRange = false;
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    enemyInRange = true;
                    break;
                }
            }

            check = enemyInRange;

            if (check)
            {
                Attack();
                yield return new WaitForSeconds(1f);

            }
            else
            {
                Move();
                yield return null;
            }
        }
    }
    void Move()
    {
        target = GameObject.FindGameObjectWithTag("Enemy").transform;

        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
            transform.LookAt(target);

        }

        Debug.Log("이동");
    }

    void Attack()
    {
        target = GameObject.FindGameObjectWithTag("Enemy").transform;

        if (target != null)
        {
            transform.LookAt(target);
        }
        attackArea.SetActive(!attackArea.activeSelf);
        Debug.Log("공격");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
