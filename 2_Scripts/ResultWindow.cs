using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{
    [SerializeField] Text _txtScore;
    [SerializeField] Text _txtMin;
    [SerializeField] Text _txtSec;
    [SerializeField] Text _txtMilSec;

    public void OpenResultWindow(int score, float time)
    {
        int min = 0;
        int sec = (int)time;
        int msec = (int)((time - sec) * 100);

        if (sec >= 60)
        {
            min++;
            sec = sec - 60;
        }

        if (min < 10)
            _txtMin.text = "0" + min.ToString();
        else
            _txtMin.text = min.ToString();

        if (sec < 10)
            _txtSec.text = "0" + sec.ToString();
        else
            _txtSec.text = sec.ToString();

        if (msec < 10)
            _txtMilSec.text = "0" + msec.ToString();
        else
            _txtMilSec.text = msec.ToString();

        _txtScore.text = score.ToString();
    }

    public void MenuButton()
    {
        BaseSceneManagerObj._instance.StartMenuScene("IngameScene");
    }

    public void RestartButton()
    {
        BaseSceneManagerObj._instance.StartIngameScene(BaseSceneManagerObj._instance._nowStageNumber, "IngameScene");
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}