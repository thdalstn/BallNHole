using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour
{
    [SerializeField] Slider _bar;
    [SerializeField] Text _txtTip;
    [SerializeField] Text _loadingText;
    [SerializeField] string[] _tips;
    eRootLoadingState _currentRootLoadingState;
    float _timeCheck;

    enum eRootLoadingState
    {
        Loading1,
        Loading2,
        Loading3,
        Loading4
    }
    
    void Awake()
    {
        _timeCheck = 0;
    }

    void Start()
    {
        OpenWindow();
    }

    void Update()
    {
        switch (_currentRootLoadingState)
        {
            case eRootLoadingState.Loading1:
                _timeCheck += Time.deltaTime;

                if (_timeCheck >= 0.5f)
                {
                    Loading2();
                }
                break;

            case eRootLoadingState.Loading2:
                _timeCheck += Time.deltaTime;

                if (_timeCheck >= 1)
                {
                    Loading3();
                }
                break;

            case eRootLoadingState.Loading3:
                _timeCheck += Time.deltaTime;

                if (_timeCheck >= 1.5f)
                {
                    Loading4();
                }
                break;

            case eRootLoadingState.Loading4:
                _timeCheck += Time.deltaTime;

                if (_timeCheck >= 2)
                {
                    _timeCheck = 0;
                    Loading1();
                }
                break;
        }
    }

    void Loading1()
    {
        _currentRootLoadingState = eRootLoadingState.Loading1;
        _loadingText.text = "Loading";
    }

    void Loading2()
    {
        _currentRootLoadingState = eRootLoadingState.Loading2;
        _loadingText.text = "Loading  .";
    }

    void Loading3()
    {
        _currentRootLoadingState = eRootLoadingState.Loading3;
        _loadingText.text = "Loading  .  .";
    }

    void Loading4()
    {
        _currentRootLoadingState = eRootLoadingState.Loading4;
        _loadingText.text = "Loading  .  .  .";
    }

    public void OpenWindow()
    {
        _bar.value = 0;
        int rnum = Random.Range(0, _tips.Length);

        _txtTip.text = _tips[rnum];
    }

    public void SettingBarValue(float val)
    {
        _bar.value = val;
    }
}