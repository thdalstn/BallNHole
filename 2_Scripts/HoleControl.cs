using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleControl : MonoBehaviour
{
    [SerializeField] BallControl.eColorType colorType = BallControl.eColorType.RED;
    float TimeCheck = 0;
    int _BumpBNH = 100;
    int _ReverseBumpBNH = -50;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BALL"))
        {
            TimeCheck += Time.deltaTime;

            BallControl bc = other.GetComponent<BallControl>(); // other에게 있는 BallControl 가져오기
            Rigidbody rgb = other.GetComponent<Rigidbody>(); // 공의 Rigidbody. 공 자신에게 붙은 녀석인 other.
            Vector3 dir = transform.position - other.transform.position; // 공이 Hole 자신을 향해올때 Hole 자신은 어디를 향해 보는지에 대한 함수. - 연산.
                                                                         // 벡터끼리의 합은 양변이 바뀌어도 아무런 변화가 없는 대신에,
                                                                         // 벡터끼리의 차는 서로 반대방향으로 바뀐다.
                                                                         // 이를 이용해서 목표점과 내 현재 위치와 어느 방향으로 가야할지 알수가 있다.
            if (colorType == bc._typeColor) // 색이 같을때, Hole 자신에게 Ball을 끌어온다.
            {
                rgb.velocity *= 0.9f;
                rgb.AddForce(dir * 20.0f * rgb.mass);
                Destroy(rgb.gameObject, 2.5f); // 공이 들어가고 2.5초 후에 공을 삭제한다.
            }
            else // 색이 다를때, Ball을 밀어낸다. Rigidbody Direction의 반대방향으로 날린다. -를 이용한다.
                rgb.AddForce(-dir * 100.0f * rgb.mass); // = rgb.AddForce(dir * -100.0f * rgb.mass);

            //dir.Normalize();

            // 공이 들어간 후에 1.5초뒤에 Ball에 제동을 건다.
            //if (TimeCheck >= 1.5f)
            //{
            //    rgb.velocity *= 0.01f;
            //}
        }
    }

    void OnTriggerEnter(Collider other) // or OnTriggerStay
    {
        if (other.CompareTag("BALL"))
        {
            BallControl bc = other.GetComponent<BallControl>();

            if (colorType == bc._typeColor)
            {
                IngameManagerObj._instance.AddScore(_BumpBNH);

                SoundManagerObj._instance.PlayES(SoundManagerObj.eESTYPE.GOAL);
            }
            else if (colorType != bc._typeColor)
            {
                IngameManagerObj._instance.AddScore(_ReverseBumpBNH);

                SoundManagerObj._instance.PlayES(SoundManagerObj.eESTYPE.NOGOAL);
            }
        }
    }
}