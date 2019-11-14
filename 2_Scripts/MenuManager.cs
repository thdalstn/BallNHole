using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Dropdown _selectStage;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    public void ClickStartButton()
    {
        BaseSceneManagerObj._instance.StartIngameScene((_selectStage.value + 1), "MenuScene");

        SoundManagerObj._instance.PlayES(SoundManagerObj.eESTYPE.BTNCLICK);
    }
}