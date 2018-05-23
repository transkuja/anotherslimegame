using UnityEngine;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour {

    public Activable[] activable;
    public ButtonTrigger[] button;

    // This button
    private GameObject thisButton;
    private Vector3 posOrigin;
    private Vector3 posArrive;
    public Transform transformArrive;
    private Material mat;
    public Material newMat;

    public Text text;

    private bool isActivating = false;

    public bool isActive = false;

    private bool doCommand = false;
    private bool doAction = false;

    [HideInInspector]
    public bool hasToMoveButton = false;
    public int persistenceIndex = -1;

    // Anim button
    private float timer = 0.5f;
    private float currentTimer = 0.0f;


    // Reset
    public float resetTimer = 5.0f;
    private float currentResetTimer = 0.0f;

    // Options
    public bool isABackAndForthAction = false;
    public bool hasToResetAutomatically = false;
    public bool hasToLockAfterActivation = false;

    private bool isLock = false;

    public void Start()
    {
        if (button.Length == 0)
            Debug.LogError("At least one button");

        if (activable.Length == 0)
            Debug.LogError("At least one activable");

        thisButton = transform.GetChild(1).gameObject;
        posOrigin = thisButton.transform.localPosition;
        posArrive = transformArrive.localPosition;

        text.text = "";

        mat = transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (hasToLockAfterActivation && isLock)
            return;

        if (!hasToMoveButton && (!isActivating || isABackAndForthAction))
        {
            if (collider.gameObject.GetComponent<Player>())
            {
                PlayerCharacterHub pch = collider.gameObject.GetComponent<PlayerCharacterHub>();
                if (pch.PlayerState is DashState
                    || pch.PlayerState is DashDownState)
                {
                    hasToMoveButton = true;

                    // play hit particles
                    ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);

                    currentTimer = 0.0f;
                    currentResetTimer = resetTimer;

                    //Set vibrations
                    UWPAndXInput.GamePad.VibrateForSeconds(pch.GetComponent<PlayerController>().playerIndex, 0.8f, 0.8f, .1f);


                    if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                }
            }
        }
    }

    public void Update()
    {
        if (hasToLockAfterActivation && isLock)
            return;

        // Behavior button
        if (hasToMoveButton)
        {
            if (!isActivating)
            {
                ActivateButton();  
            }
            else
            {
                DesactivateButton();
            }

            if (doCommand)
            {
                Command();
            }
        }

        // Behavior activable
        if(doAction)
        {
            doAction = false;

            bool allActive = true;
            for (int i = 0; i < button.Length; i++)
            {
                if (!button[i].isActive)
                {
                    allActive = false;
                }
            }

            if (activable[0].isActive && !allActive)
            {
                for (int i = 0; i < activable.Length; i++)
                {
                    activable[i].Active(false);
                }
            } else if(!activable[0].isActive && allActive)
            {
                for (int i = 0; i < activable.Length; i++)
                {
                    activable[i].Active(true);
                }
                if(hasToLockAfterActivation)
                {
                    for (int i = 0; i < button.Length; i++)
                    {
                        button[i].text.text = "";
                        button[i].isLock = true;

                        // Save button changing state in db
                        if (!DatabaseManager.Db.alreadyUnlockButtons[button[i].persistenceIndex])
                        {
                            DatabaseManager.Db.alreadyUnlockButtons[button[i].persistenceIndex] = true;
                            DatabaseManager.instance.SaveData();
                        }
                    }
                }
            }
        }

        // if has to reset
        if(!isLock && !hasToMoveButton && isActive && hasToResetAutomatically)
        {
            currentResetTimer -= Time.deltaTime;
            text.text = Mathf.RoundToInt(currentResetTimer).ToString();
            if (currentResetTimer <= 0)
            {
                hasToMoveButton = true;
                currentTimer = 0.0f;
                text.text = "";
                currentResetTimer = resetTimer;
            }
        }
    }

    public void Command()
    {

        // true
        doCommand = false;
        isActive = !isActive;
        doAction = true;
    }

    public void ActivateButton()
    {
        currentTimer += Time.deltaTime;

        // lerp position and mat
        thisButton.transform.localPosition = Vector3.Lerp(posOrigin, posArrive, currentTimer / timer);
        transform.GetChild(0).GetComponent<Renderer>().material.Lerp(mat, newMat, currentTimer / timer);
        thisButton.transform.GetChild(0).GetComponent<Renderer>().material.Lerp(mat, newMat, currentTimer / timer);

        if (currentTimer >= timer)
        {
            // force set mat
            transform.GetChild(0).GetComponent<Renderer>().material = newMat;
            thisButton.transform.GetChild(0).GetComponent<Renderer>().material = newMat;

            // Activation
            hasToMoveButton = false;
            doCommand = true;
            isActivating = true;
        }
    }
    public void DesactivateButton()
    {
        currentTimer += Time.deltaTime;

        // lerp position and mat
        thisButton.transform.localPosition = Vector3.Lerp(posArrive, posOrigin, currentTimer / timer);
        transform.GetChild(0).GetComponent<Renderer>().material.Lerp(newMat, mat, currentTimer / timer);
        thisButton.transform.GetChild(0).GetComponent<Renderer>().material.Lerp(newMat, mat, currentTimer / timer);

        if (currentTimer >= timer)
        {
            // force set mat
            transform.GetChild(0).GetComponent<Renderer>().material = mat;
            thisButton.transform.GetChild(0).GetComponent<Renderer>().material = mat; ;

            // Activation
            hasToMoveButton = false;
            doCommand = true;

            isActivating = false;
        }
    }
}
