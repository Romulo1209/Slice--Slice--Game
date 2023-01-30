using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableObject : MonoBehaviour
{
    //Script responsavel pela lógica dos objetos cortaveis
    [SerializeField] int objectValue;

    [SerializeField] BoxCollider boxCollider;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject[] slicedParts;
    [SerializeField] ParticleSystem[] particles;

    //Executa o corte do objeto
    public void Slice()
    {
        foreach(ParticleSystem particle in particles) {
            particle.Play();
        }
        foreach(GameObject part in slicedParts) {
            part.SetActive(true);
            part.transform.parent = null;
        }
        slicedParts[0].GetComponent<Rigidbody>().AddForce(new Vector3(-3, 1.5f, 0), ForceMode.Impulse);
        slicedParts[1].GetComponent<Rigidbody>().AddForce(new Vector3(3, 1.5f, 0), ForceMode.Impulse);
        boxCollider.enabled = false;
        meshRenderer.enabled = false;
        GameController.instance.CurrentPoints = objectValue;

        StartCoroutine(DestroyObjects(slicedParts[0].transform));
        StartCoroutine(DestroyObjects(slicedParts[1].transform));
        StartCoroutine(DestroyScript());
    }

    //Destroi o item depois de um tempo e executa uma animação
    IEnumerator DestroyObjects(Transform objectToDestroy)
    {
        yield return new WaitForSeconds(1.5f);

        float needTime = 1;
        float elapsedTime = 0;
        Vector3 finalScale = Vector3.zero;
        while (elapsedTime < needTime) {
            objectToDestroy.localScale = Vector3.Lerp(objectToDestroy.localScale, finalScale, (elapsedTime / (needTime * 30)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(objectToDestroy.gameObject);
    }
    //Finalização da destruição
    IEnumerator DestroyScript()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
