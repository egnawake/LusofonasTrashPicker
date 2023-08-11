using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float bufferTime;

    private YieldInstruction waitInstruction;
    private RobotAction buffer;

    public RobotAction Action
    {
        get
        {
            RobotAction b = buffer;
            buffer = RobotAction.None;
            return b;
        }
    }

    private void Start()
    {
        this.waitInstruction = new WaitForSeconds(bufferTime);
        StartCoroutine(ClearBuffer());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Skip Turn"))
        {
            buffer = RobotAction.SkipTurn;
        }
        else if (Input.GetButtonDown("Collect"))
        {
            buffer = RobotAction.CollectTrash;
        }
        else if (Input.GetButtonDown("Up") || Input.GetButtonDown("Up Numpad"))
        {
            buffer = RobotAction.MoveNorth;
        }
        else if (Input.GetButtonDown("Right") || Input.GetButtonDown("Right Numpad"))
        {
            buffer = RobotAction.MoveEast;
        }
        else if (Input.GetButtonDown("Down") || Input.GetButtonDown("Down Numpad"))
        {
            buffer = RobotAction.MoveSouth;
        }
        else if (Input.GetButtonDown("Left") || Input.GetButtonDown("Left Numpad"))
        {
            buffer = RobotAction.MoveWest;
        }
        else if (Input.GetButtonDown("Move Random"))
        {
            buffer = RobotAction.MoveRandom;
        }
    }

    private IEnumerator ClearBuffer()
    {
        while (true)
        {
            buffer = RobotAction.None;
            yield return waitInstruction;
        }
    }
}
