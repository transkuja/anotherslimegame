using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour {

    public GameObject door;
    public GameObject button;
    private Vector3 posOrigin;
    private Vector3 posArrive;
    public Transform transformArrive;
    private Material mat;
    public Material newMat;

    private bool isActive = false;

    private bool active = false;
    public float timer = 2.0f;
    private float currentTimer = 0.0f;

    public bool isABackAndForthAction = false;

    private Vector3 scalePorteOrigin;
    private Vector3 scalePorteArrive;

    public void Start()
    {
        mat = transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
        posOrigin = button.transform.localPosition;
        posArrive = transformArrive.localPosition;
        scalePorteOrigin = door.transform.localScale;
        scalePorteArrive = new Vector3(scalePorteOrigin.x, 0, scalePorteOrigin.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!active && (!isActive || isABackAndForthAction))
        {
            if (collision.gameObject.GetComponent<Player>())
            {
                PlayerCharacterHub pch = collision.gameObject.GetComponent<PlayerCharacterHub>();
                if (pch.PlayerState is DashState
                    || pch.PlayerState is DashDownState)
                {
                    active = true;
                    ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
                    currentTimer = 0.0f;
                }
            }
        }
    }

    public void Update()
    {
        if (active)
        {
            if (!isActive)
            {
                ActivateButton(mat, newMat, posOrigin, posArrive, scalePorteOrigin, scalePorteArrive);
            }
            else
            {
                ActivateButton(newMat, mat, posArrive, posOrigin, scalePorteArrive, scalePorteOrigin);
            }
        }
    }

    public void ActivateButton(Material _mat, Material _newMat, Vector3 _pos1, Vector3 _pos2, Vector3 _scale1, Vector3 _scale2)
    {
        currentTimer += Time.deltaTime;

        // lerp position and mat
        button.transform.localPosition = Vector3.Lerp(_pos1, _pos2, currentTimer / timer);
        transform.GetChild(0).GetComponent<Renderer>().material.Lerp(_mat, _newMat, currentTimer / timer);
        button.transform.GetChild(0).GetComponent<Renderer>().material.Lerp(_mat, _newMat, currentTimer / timer);

        // would need to rethink this
        if (isActive)
            door.SetActive(isActive);

        door.transform.localScale = Vector3.Lerp(_scale1, _scale2, currentTimer / timer);

        if (currentTimer >= timer)
        {
            // force set mat
            transform.GetChild(0).GetComponent<Renderer>().material = _newMat;
            button.transform.GetChild(0).GetComponent<Renderer>().material = _newMat;


            // Activation
            active = false;
            isActive = !isActive;

            // would need to rethink this
            if ( isActive)
                door.SetActive(!isActive);
        }
    }
}
