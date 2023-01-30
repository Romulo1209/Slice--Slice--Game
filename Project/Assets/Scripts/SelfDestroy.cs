using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    //Script para auto destruição de particulas e objetos que precisam ser destruidos depois de algum tempo
    private void Start()
    {
        StartCoroutine(SD());
    }
    IEnumerator SD()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
