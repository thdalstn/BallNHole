using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    [SerializeField] eColorType colorType = eColorType.RED;
    [SerializeField] GameObject _prefabColliderEffect;
    [SerializeField] GameObject _HIT;
    Rigidbody _rgb3D;
    const int _BumpBNB = -20;
    const int _BumpBNW = -10;
    const int _StayBumpBNW = -5;
    float _timeCheck = 0;

    public enum eColorType
    {
        RED,
        BLUE,
        GREEN
    }

    public eColorType _typeColor
    {
        get
        {
            return colorType;
        }
    }

    // 볼 생성시 호출할 함수. Stage마다 난이도를 다르게 적용하기 위한 함수 설정. 편의성을 위한 함수.
    public void InitBallData (float mass, float anDrag, float drag)
    {
        _rgb3D = GetComponent<Rigidbody>(); // Rigidbody Component 사용하겠다고 선언 (Mass, Drag 등)
        _rgb3D.mass = mass;
        _rgb3D.angularDrag = anDrag;
        _rgb3D.drag = drag;

        Transform tf;
        Vector3 pos = Vector3.zero;

        switch (colorType)
        {
            case eColorType.RED:
                tf = GameObject.FindGameObjectWithTag("StartPosRed").transform;
                pos = tf.position;
                break;
            case eColorType.BLUE:
                tf = GameObject.FindGameObjectWithTag("StartPosBlue").transform;
                pos = tf.position;
                break;
            case eColorType.GREEN:
                tf = GameObject.FindGameObjectWithTag("StartPosGreen").transform;
                pos = tf.position;
                break;
        }
        transform.position = pos;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Ball끼리 부딪혔을때 나타낼 이펙트
        if (collision.collider.CompareTag("BALL"))
        {
            SoundManagerObj._instance.PlayES(SoundManagerObj.eESTYPE.BNB);
            GameObject go = Instantiate(_prefabColliderEffect);
            Vector3 pos = transform.position - collision.transform.position;

            IngameManagerObj._instance.AddScore(_BumpBNB);

            go.transform.position = transform.position - pos;
            pos = pos.normalized * (transform.GetChild(0).localScale.x / 2); // Scale의 전체에서 2를 나누어주어야한다.
            Destroy(go, 0.7f);
        }

        // Ball이 Wall에 부딪혔을때 나타낼 이펙트
        if (collision.collider.CompareTag("WALL"))
        {
            SoundManagerObj._instance.PlayES(SoundManagerObj.eESTYPE.BNW);
            GameObject go = Instantiate(_HIT);

            IngameManagerObj._instance.AddScore(_BumpBNW);

            go.transform.position = transform.position; // collision.transform.position;을 대입시키면 Hit 모션이 이상한곳에서 나타난다.
            Destroy(go, 0.7f);
        }
    }

    // Ball이 Wall에 닿아있는 시간이 1초가 지날때마다 -5점 (_StayBumpBNW)를 한다.
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("WALL"))
        {
            Physics.sleepThreshold = 0; // 설명에 get, set일 경우에는 =으로 작성한다.
            _timeCheck += Time.deltaTime;

            if (_timeCheck >= 1)
            {
                IngameManagerObj._instance.AddScore(_StayBumpBNW);
                _timeCheck = 0;
            }
        }

        Physics.sleepThreshold = 0.005f;
    }
}