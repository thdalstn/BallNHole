using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameManagerObj : MonoBehaviour
{
    [SerializeField] GameObject _preResultWnd;
    [SerializeField] GameObject _preExitWnd;
    [SerializeField] Text _txtInfo;
    [SerializeField] Text _txtMin;
    [SerializeField] Text _txtSec;
    [SerializeField] Text _txtScore;
    [SerializeField] float _gravityScale = 5;
    ResultWindow _ResultWnd;
    ExitWindow _ExitWnd;
    BallControl _BC;
    eGameState _currentGameState;
    static IngameManagerObj _uniqueInstance;
    const float _Origravity = 9.81f; // const: 변수를 상수화 시켜준다. 최초에 설정해준 현재 값으로 고정이 된다.
                                     // 다른 곳에서 값을 바꾸려고 해도 절대 바뀌지 않는다.
                                     // Project Settings에서 값 확인 (기본값 9.81)
    float _timeCheck = 0;
    float _timeGame = 0;
    int _totalScore = 0;

    static public IngameManagerObj _instance
    {
        get
        {
            return _uniqueInstance;
        }
    }

    public eGameState _nowState
    {
        get
        {
            return _currentGameState;
        }
    }

    public enum eGameState
    {
        PLAY,
        EXIT,
        END,
        CLEAR,
        DIE
    }

    void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        AddScore(50);
        SoundManagerObj._instance.PlayBGM(SoundManagerObj.eBGMTYPE.STAGE1);
    }

    void Update()
    {
        Vector3 vec = Vector3.zero;
        
#if UNITY_EDITOR // 전처리 기능: Unity에서 게임을 실행했을때 적용할 기능
        vec.x = Input.GetAxis("Horizontal");
        vec.y = _Origravity * -1; // 공이 붕 뜨는게 아니라, 제대로 굴러가는 느낌이 들게 해준다.
        vec.z = Input.GetAxis("Vertical");

#else // 핸드폰에서 게임을 실행했을때 적용할 기능
        vec.x = Input.acceleration.x;
        vec.y = _Origravity * -1;
        vec.z = Input.acceleration.y; // y축과는 다르므로 받아온다.

#endif
        Physics.gravity = vec.normalized * _Origravity * _gravityScale;

        switch (_currentGameState)
        {
            case eGameState.PLAY:
                AddGameTime(Time.deltaTime);
                
                if (_totalScore >= 0 && Input.GetButtonDown("Cancel"))
                    GameExit();
                else if (_totalScore <= -1)
                    GameEnd();

                break;

            case eGameState.END:
                if (_BC == null)
                    GameClear();
                else if (_totalScore <= -1)
                    GameDie();

                break;

            case eGameState.CLEAR:

                break;

            case eGameState.DIE:

                break;

            case eGameState.EXIT:
                if (Input.GetButtonDown("Cancel"))
                    GamePlay();

                break;
        }
    }

    public void GamePlay()
    {
        _currentGameState = eGameState.PLAY;

        BallControl[] moves = FindObjectsOfType<BallControl>();

        foreach (BallControl item in moves)
        {
            item.enabled = true;
        }
    }

    public void GameEnd()
    {
        _currentGameState = eGameState.END;

        BallControl[] moves = FindObjectsOfType<BallControl>();

        foreach (BallControl item in moves)
        {
            item.enabled = false;
        }

        _txtInfo.enabled = true;
        _txtInfo.text = "Game Over";
    }

    void GameClear()
    {
        _currentGameState = eGameState.CLEAR;

        GameObject _ResultGO = Instantiate(_preResultWnd);

        _ResultWnd = _ResultGO.GetComponent<ResultWindow>();
        _ResultWnd.OpenResultWindow(_totalScore, _timeGame);
        _ResultWnd.gameObject.SetActive(true);
        _txtInfo.enabled = false;
    }

    void GameDie()
    {
        _currentGameState = eGameState.DIE;
        
        GameObject _ResultGO = Instantiate(_preResultWnd);
        
        _ResultWnd = _ResultGO.GetComponent<ResultWindow>();
        _ResultWnd.OpenResultWindow(_totalScore, _timeGame);
        _ResultWnd.gameObject.SetActive(true);
        _txtInfo.enabled = false;
    }
    
    public void GameExit()
    {
        _currentGameState = eGameState.EXIT;

        GameObject _ExitGO = Instantiate(_preExitWnd);

        _ExitWnd = _ExitGO.GetComponent<ExitWindow>();
        _ExitWnd.gameObject.SetActive(true);
    }

    public void AddGameTime (float time)
    {
        _timeGame += time;

        int min = 0;
        int sec = (int)_timeGame;
        int msec = (int)((_timeGame - sec) * 100);

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
    }

    public void AddScore(int addPoint)
    {
        _totalScore += addPoint;

        _txtScore.text = string.Format("<b><i>{0}</i><size=50>10000</size></b>", _totalScore);
    }

    public void CreateBall()
    {
        // 한번 쓰고 남은 변수를 보관할 필요는 없다.변수를 여러개 만들 필요가 없다는 뜻이다.
        GameObject goPrefab = Resources.Load("Prefabs/RedBallObj") as GameObject; // Prefab 가져오기. as는 강제 형변환을 시켜준다.
                                                                                  // Resources 폴더를 사용하는 경우는 얻어올때만 사용하는게 최선이다.
        goPrefab = Instantiate(goPrefab);
        BallControl bc = goPrefab.GetComponent<BallControl>();
        bc.InitBallData(3, 0.1f, 0.05f); // 해당 정보로 볼 불러오기

        goPrefab = Resources.Load("Prefabs/GreenBallObj") as GameObject;
        goPrefab = Instantiate(goPrefab);
        bc = goPrefab.GetComponent<BallControl>();
        bc.InitBallData(4, 0.1f, 0.05f);

        goPrefab = Resources.Load("Prefabs/BlueBallObj") as GameObject;
        goPrefab = Instantiate(goPrefab);
        bc = goPrefab.GetComponent<BallControl>();
        bc.InitBallData(5, 0.1f, 0.05f);
    }
}