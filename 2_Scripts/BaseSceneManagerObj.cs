using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManagerObj : MonoBehaviour
{
    eStateScene _sceneState = eStateScene.NONE;
    eStateScene _prevSceneState = eStateScene.NONE;
    eStateLoading _currentLoadState = eStateLoading.NONE;
    GameObject _prefabLoadingWnd; // LoadingWnd Prefab을 저장할 변수
    static BaseSceneManagerObj _uniqueInstance;
    public int _nowStageNumber = 0;
    int _oldStageNumber = 0;

    static public BaseSceneManagerObj _instance
    {
        get
        {
            return _uniqueInstance;
        }
    }

    public enum eStateScene
    {
        NONE,
        MENU,
        INGAME
    }

    public enum eStateLoading
    {
        NONE,
        UNLOADSCENE,
        UNLOADSTAGE,
        LOADSCENE,
        LOADSTAGE,
        ENDLOAD
    }

    void Awake()
    {
        _uniqueInstance = this;

        _prefabLoadingWnd = Resources.Load("Prefabs/LoadingWnd") as GameObject; // LoadingWnd Prefab을 Resources 폴더에서 가져와 저장한다.
    }

    void Start()
    {
        StartMenuScene();
    }

    void Update()
    {
        if (_currentLoadState == eStateLoading.ENDLOAD)
        {
            _currentLoadState = eStateLoading.NONE;
            StopCoroutine("LoadingScene");
        }
    }

    public void StartMenuScene(string unloadName = "")
    {
        _prevSceneState = _sceneState;
        _sceneState = eStateScene.MENU;
        StartCoroutine(LoadingScene(unloadName, "MenuScene"));

        SoundManagerObj._instance.PlayBGM(SoundManagerObj.eBGMTYPE.MENU);
    }

    // Scene Loading 함수 호출
    public void StartIngameScene (int stageNumber, string unloadName = "")
    {
        _prevSceneState = _sceneState;
        _sceneState = eStateScene.INGAME;
        _oldStageNumber = _nowStageNumber;
        _nowStageNumber = stageNumber;

        // 씬 로딩 함수 호출
        StartCoroutine(LoadingScene(unloadName, "IngameScene", stageNumber));
    }
    
    IEnumerator LoadingScene(string unloadName, string loadName, int stageNum = 1)
    {
        /*
        BaseManagerScene에서 게임을 시작할 것이다.
        
        unloadName에 이름이 없으면 Unload를 하지않는다.
        unloadName에 이름이 있으면 Unload를 한다.
        _prevSceneState가 Ingame인 경우 Stage도 같이 Unload한다.
        Unload를 해야하는 Stage는 _oldStageNumber다. (UnloadAsync 사용)
        loadName에 해당하는 Scene을 Load한다. (LoadAsync 사용.옵션 주의)
        Load하는 Scene이 Ingame인 경우 Stage도 같이 Load한다.
        Load하는 Stage는 stageNum이다.

        위의 작업을 끝냈다면, Ingame을 실행했을때 3개의 Scene이 Hierarchy에 생성되고, Error가 발생될 것이니 Error까지 처리하라.
        */

        AsyncOperation AO;
        string str;

        LoadingWindow wnd = Instantiate(_prefabLoadingWnd, transform).GetComponent<LoadingWindow>(); // LoadingWnd를 Hierarchy에 생성한다.
                                                                                                     // BaseSceneManager에 자식으로 붙여둔다.
                                                                                                     // wnd 변수에 생성한 LoadingWnd를 참조하도록 한다.
        // transform을 빼버리면, Loading 창은 BaseScne에서 만들어지긴 하지만, 메뉴 -> 인게임과 인게임 -> 메뉴 갈때 문제가 발생한다.
        // 현재 액티브 되어있는 씬을 위주로 만들어지기 때문에, 언로드할때 안 날라가게 하려는 씬을 현재 여기에 붙여놓은 것이다.

        _currentLoadState = eStateLoading.UNLOADSCENE;

        if (unloadName != string.Empty)
        {
            if (_prevSceneState == eStateScene.INGAME)
            {
                _currentLoadState = eStateLoading.UNLOADSTAGE;
                str = "Stage" + _oldStageNumber.ToString();
                AO = SceneManager.UnloadSceneAsync(str);

                while (!AO.isDone)
                {
                    wnd.SettingBarValue(AO.progress); // Stage의 Unload 상황을 표시한다.
                    yield return null;
                }
                wnd.SettingBarValue(10); // Stage의 Unload가 마무리 됐음을 표시한다.
            }

            AO = SceneManager.UnloadSceneAsync(unloadName);
            while (!AO.isDone)
            {
                wnd.SettingBarValue(AO.progress); // Scene의 Unload 상황을 표시한다.
                yield return null;
            }
            wnd.SettingBarValue(10); // Scene의 Unload가 마무리 됐음을 표시한다.
        }

        _currentLoadState = eStateLoading.LOADSCENE;

        AO = SceneManager.LoadSceneAsync(loadName, LoadSceneMode.Additive);
        while (!AO.isDone)
        {
            wnd.SettingBarValue(AO.progress); // Scene의 Loading 상황을 표시한다.
            yield return null;
        }
        wnd.SettingBarValue(10); // Scene의 Loading이 마무리 됐음을 표시한다.

        //yield return new WaitForSeconds(1);

        str = loadName;

        if (_sceneState == eStateScene.INGAME)
        {
            _currentLoadState = eStateLoading.LOADSTAGE;
            str = "Stage" + stageNum.ToString();
            AO = SceneManager.LoadSceneAsync(str, LoadSceneMode.Additive);

            while (!AO.isDone)
            {
                wnd.SettingBarValue(AO.progress); // Stage의 Loading 상황을 표시한다.
                wnd.SettingBarValue(0);
                yield return new WaitForSeconds(0.1f);
                wnd.SettingBarValue(2);
                yield return new WaitForSeconds(0.3f);
                wnd.SettingBarValue(5);
                yield return new WaitForSeconds(0.1f);
                wnd.SettingBarValue(6);
                yield return new WaitForSeconds(0.1f);
                wnd.SettingBarValue(7.5f);
                yield return new WaitForSeconds(0.4f);
                wnd.SettingBarValue(10);
                yield return new WaitForSeconds(0.2f);
                yield return null;
            }
            wnd.SettingBarValue(10); // Stage의 Loading이 마무리 됐음을 표시한다.
        }

        //yield return new WaitForSeconds(1);
        Scene s = SceneManager.GetSceneByName(str);
        SceneManager.SetActiveScene(s);

        _currentLoadState = eStateLoading.ENDLOAD;

        if (_sceneState == eStateScene.INGAME)
            IngameManagerObj._instance.CreateBall();
        
        Destroy(wnd.gameObject); // LoadingWnd를 Hierarchy (메모리)에서 제거한다.
    }
}