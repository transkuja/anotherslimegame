﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

/*
 * Design idea
 * Can throw platforms in front of him while charges > 0
 * Press a button while throwing change platform property (ex: bouncy)
 * Maintain throw button consume all charges to either construct a bridge or a stair (find an intuitive way may be hard)
 */
public class EvolutionPlatformist : EvolutionComponent {
    // Cooldown before setting a new platform
    float cooldownPlatform = 0.0f; // TODO: remove?
    float timerPlatform = 0.0f;

    // Number of charges
    uint charges = 3;
    uint maxCharges = 3;

    // Cooldown before getting another charge
    float cooldownCharge = 7.0f;
    float timerBeforeCharge = 0.0f;
    float chargeTime = 0.1f;

    float platformLifetime = 6.0f;
    int indexPattern = 0;
    public int moduloIndexPattern = 2;

    bool hasPlayedSecondTuto = false;

    // Private variables
    List<GameObject> showPattern;

    public override void Start()
    {
        base.Start();
        if (GameManager.Instance.IsInHub())
        {
            cooldownCharge = 1.0f;
        }

        SetPower(Powers.Platformist);
        Player playerComponent = GetComponent<Player>();

        if (playerComponent.evolutionTutoShown != null && playerComponent.evolutionTutoShown.Length > 0 && !playerComponent.evolutionTutoShown[(int)Powers.Platformist] && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            playerComponent.evolutionTutoShown[(int)Powers.Platformist] = true;
            if ((int)GetComponent<PlayerController>().playerIndex == Controls.keyboardIndex)
                Utils.PopTutoText("Hold", KeyboardControlType.SpawnPl, "to create platforms", playerComponent);
            else
                Utils.PopTutoText("Hold", ControlType.RightTrigger, "to create platforms", playerComponent);
        }
        else
        {
            hasPlayedSecondTuto = true;
        }

        if (GameManager.Instance.CurrentGameMode is Runner3DGameMode && GameManager.Instance.CurrentGameMode.minigameVersion == 1)
        {
            feedbackCooldownImg.transform.parent.GetChild(0).GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.Platformist;
            feedbackCooldownImg.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
            feedbackCooldownImg.fillAmount = 1;

        }

        if (feedbackCooldownImg != null)
            feedbackCooldownImg.sprite = ResourceUtils.Instance.spriteUtils.Platformist;
    }

    public float TimerPlatform
    {
        get
        {
            return timerPlatform;
        }

        set
        {
            timerPlatform = value;
            if (timerPlatform >= cooldownPlatform)
            {
                timerPlatform = 0.0f;
            }
        }
    }

    public uint Charges
    {
        get
        {
            return charges;
        }

        set
        {
            charges = value;
            // update ui?
        }
    }

    public float CooldownCharge
    {
        get
        {
            return cooldownCharge;
        }

        set
        {
            cooldownCharge = value;
        }
    }

    public float PlatformLifetime
    {
        get
        {
            return platformLifetime;
        }

        set
        {
            platformLifetime = value;
        }
    }

    public int IndexPattern
    {
        get
        {
            return indexPattern;
        }

        set
        {
            indexPattern = value;
        }
    }

    public float ChargeTime
    {
        get
        {
            return chargeTime;
        }
    }

    public override void Update()
    {
        base.Update();
        if (Charges < maxCharges)
        {
            timerBeforeCharge += Time.deltaTime;

            if (GameManager.Instance.CurrentGameMode is Runner3DGameMode)
            {
                feedbackCooldownImg.fillAmount = timerBeforeCharge/cooldownCharge;
            }

            if (timerBeforeCharge >= cooldownCharge)
            {
                if (GameManager.Instance.CurrentGameMode is Runner3DGameMode && GameManager.Instance.CurrentGameMode.minigameVersion == 1)
                    feedbackCooldownImg.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                Charges = maxCharges;
                timerBeforeCharge = 0.0f;
            }
        }

    }

    //public void CreatePlatform(GamePadState receivedInput)
    //{
    //    if (Charges > 0)
    //    {
    //        GameObject platform = Instantiate(ResourceUtils.Instance.refPrefabPlatform.prefabPlatformistDefault);
    //        platform.transform.position = transform.position + transform.forward * summonDistance + Vector3.up * summonHeight;
    //        platform.transform.rotation = transform.rotation;
    //        platform.transform.GetComponent<AutoDestroyPlatform>().Init(platformLifetime);

    //        Charges--;
    //        if (receivedInput.Buttons.LeftShoulder == ButtonState.Pressed)
    //        {
    //            platform.GetComponent<PlatformGameplay>().isBouncy = true;
    //        }
    //        TrappedPlatform trappedComponent = platform.GetComponent<TrappedPlatform>();
    //        if (trappedComponent)
    //            trappedComponent.owner = GetComponent<Player>();
    //    }
    //}

    public void CreatePlatforms()
    {
        ClearShowPattern();

        if (Charges > 0)
        {
            float isGroundedOffset = 0.0f;

            if (!playerCharacter.IsGrounded)
            {
                GameObject initial = Instantiate(ResourceUtils.Instance.refPrefabPlatform.prefabPlatformistDefault);
                initial.transform.position = transform.position - 0.4f * Vector3.up;
                initial.transform.rotation = transform.rotation;
                initial.transform.GetComponent<AutoDestroyPlatform>().Init(platformLifetime);

                TrappedPlatform trappedComponent = initial.GetComponent<TrappedPlatform>();
                if (trappedComponent)
                    trappedComponent.owner = GetComponent<Player>();
            }
            else
            {
                isGroundedOffset = 1.0f;
            }

            GameObject[] platforms = new GameObject[Charges];
            PlatformistPattern pattern = PlatformistPatternFactory.GetPatternFromIndex(IndexPattern);

            for (int i = 0; i < Charges; i++)
            {
                platforms[i] = Instantiate(ResourceUtils.Instance.refPrefabPlatform.prefabPlatformistDefault);
                platforms[i].transform.position = transform.position - 0.4f * Vector3.up
                    + transform.forward * pattern.summonDistance * (pattern.distanceStep * (i + 1)) 
                    + Vector3.up * (pattern.summonHeight * (pattern.heightStep * (i + 1)) + isGroundedOffset);
                platforms[i].transform.rotation = transform.rotation;
                platforms[i].transform.GetComponent<AutoDestroyPlatform>().Init(platformLifetime);

                TrappedPlatform trappedComponent = platforms[i].GetComponent<TrappedPlatform>();
                if (trappedComponent)
                    trappedComponent.owner = GetComponent<Player>();
            }
            Charges = 0;

            if (GameManager.Instance.CurrentGameMode is Runner3DGameMode && GameManager.Instance.CurrentGameMode.minigameVersion == 1)
                feedbackCooldownImg.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }
    
    void ShowPattern()
    {
        ClearShowPattern();

        showPattern = new List<GameObject>();

        if (Charges > 0)
        {
            float isGroundedOffset = 0.0f;

            PlatformistPattern pattern = PlatformistPatternFactory.GetPatternFromIndex(IndexPattern);

            if (!playerCharacter.IsGrounded)
            {
                GameObject platform = Instantiate(ResourceUtils.Instance.refPrefabPlatform.prefabPlatformistShowPattern);
                platform.transform.position = transform.position - 0.4f * Vector3.up;
                platform.transform.rotation = transform.rotation;
                showPattern.Add(platform);
            }
            else
            {
                isGroundedOffset = 1.0f;
            }

            for (int i = 0; i < Charges; i++)
            {
                GameObject platform = Instantiate(ResourceUtils.Instance.refPrefabPlatform.prefabPlatformistShowPattern);
                platform.transform.position = transform.position - 0.4f * Vector3.up
                    + transform.forward * pattern.summonDistance * (pattern.distanceStep * (i + 1))
                    + Vector3.up * (pattern.summonHeight * (pattern.heightStep * (i + 1)) + isGroundedOffset);
                platform.transform.rotation = transform.rotation;
                showPattern.Add(platform);
            }

        }

        if (!hasPlayedSecondTuto && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            hasPlayedSecondTuto = true;
            if ((int)GetComponent<PlayerController>().playerIndex == Controls.keyboardIndex)
                Utils.PopTutoText("Scroll", KeyboardControlType.ChangePatternPl, "to change pattern", GetComponent<Player>());
            else

                Utils.PopTutoText("Press", ControlType.RightShoulder, "to change pattern", GetComponent<Player>());
        }
    }

    void ClearShowPattern()
    {
        if (showPattern != null && showPattern.Count > 0)
        {
            foreach (GameObject go in showPattern) Destroy(go);
        }
    }

    public void IndexSelection(GamePadState receivedPrevState, GamePadState receivedState, int playerIndex)
    {
        if (Controls.ChangePlatformsPattern(receivedPrevState, receivedState, playerIndex))
        {
            IndexPattern = (IndexPattern + 1) % moduloIndexPattern;
        }

        ShowPattern();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ClearShowPattern();
    }
}

static class PlatformistPatternFactory
{
    public static PlatformistPattern GetPatternFromIndex(int _selectedIndex)
    {
        if (_selectedIndex == 0) return GetBridgePattern();
        if (_selectedIndex == 1) return GetStairPattern();
        else
        {
            Debug.Log("ERROR: pattern index incorrect: " + _selectedIndex + ", returning bridge pattern by default");
            return GetBridgePattern();
        }
    }

    static PlatformistPattern GetStairPattern()
    {
        return new PlatformistPattern(8.0f, 4.0f, 1, 1);
    }

    static PlatformistPattern GetBridgePattern()
    {
        return new PlatformistPattern(8.0f, 0.0f, 2, 0);
    }
}

class PlatformistPattern
{
    public float summonDistance;
    public float summonHeight;
    public int distanceStep;
    public int heightStep;

    public PlatformistPattern(float _summonDistance, float _summonHeight, int _distanceStep, int _heightStep)
    {
        summonDistance = _summonDistance;
        summonHeight = _summonHeight;
        distanceStep = _distanceStep;
        heightStep = _heightStep;
    }
}
