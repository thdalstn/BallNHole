using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWindow : MonoBehaviour
{
    public void YesButton()
    {
        BaseSceneManagerObj._instance.StartMenuScene("IngameScene");
    }

    public void NoButton()
    {
        IngameManagerObj._instance.GamePlay();
    }
}