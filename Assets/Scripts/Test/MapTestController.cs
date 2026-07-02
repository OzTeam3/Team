using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapTestController : MonoBehaviour
{
    [Header("테스트 조작 설정")]
    [Header("I키: Y값 증가 / O키: Y값 감소")]
    [SerializeField] private float _testPlayerY;

    private void Update()
    {
        ControllPositonY();
        transform.position = new Vector3(0, _testPlayerY, 0);
    }

    private void ControllPositonY()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.iKey.isPressed)
            {
                _testPlayerY += 20 * Time.deltaTime;
            }

            if (Keyboard.current.oKey.isPressed)
            {
                _testPlayerY -= 20 * Time.deltaTime;
            }
        }
    }
}