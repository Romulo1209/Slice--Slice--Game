using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    //Controaldor básico de janelas
    public Window[] AllWindows;

    //Abrir uma janela
    public void OpenWindow(Window windowToOpen)
    {
        foreach(Window window in AllWindows) {
            window.Close();
        }
        windowToOpen.Open();
    }
}
