using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AllChunksScriptable : ScriptableObject
{
    //Armazena qualquer informação de mapas
    public List<GameObject> MapChunks;
    public GameObject FinishLine;
}
