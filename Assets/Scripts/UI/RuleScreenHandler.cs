using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleScreenHandler : MonoBehaviour {
    enum RuleScreenState { FirstPage, ControlsPage, PickupPage }
    RuleScreenState curState = RuleScreenState.FirstPage;

    private RuleScreenState CurState {
        set
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                if (i - 1 == (int)value)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                    transform.GetChild(i).gameObject.SetActive(false);
            }
            
            curState = value;
        }
    }

    void Start () {
        CurState = RuleScreenState.FirstPage;
    }

    public void ChangeState(bool _stateForward)
    {
        if (_stateForward && curState != RuleScreenState.PickupPage)
            CurState = (RuleScreenState)(int)curState + 1;

        if (!_stateForward && curState != RuleScreenState.FirstPage)
            CurState = (RuleScreenState)(int)curState - 1;
    }
}
