using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyWin : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            StartCoroutine(Dead());
        }
    }

    private IEnumerator Dead()
    {
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(0);
    }
}
