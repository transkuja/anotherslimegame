using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner3D
{
    public enum DirLerpState
    {
        Up,
        Down,
        Moving
    }

    public class RunnerBlocs : MonoBehaviour {

        float[] baseYPos;
        Vector3[] baseScale;
        Quaternion[] baseOrientation;

        static float yInterval = 10;
        [SerializeField] Vector3 blockSize;
        bool[] hasFinished;
        public void Awake()
        {
            baseYPos = new float[transform.childCount];
            baseScale = new Vector3[transform.childCount];
            baseOrientation = new Quaternion[transform.childCount];
            hasFinished = new bool[transform.childCount];
        }
        public void SaveStartPos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                // register base position
                baseYPos[i] = transform.GetChild(i).position.y;
                baseScale[i] = transform.GetChild(i).localScale;
                baseOrientation[i] = transform.GetChild(i).rotation;
                // goDown at beginning : 
                if (transform.GetChild(i).GetComponent<PlatformGameplay>())
                    transform.GetChild(i).GetComponent<PlatformGameplay>().enabled = false;
                hasFinished[i] = false;
            }
        }
        public void TpDown()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.position = new Vector3(child.position.x, child.position.y - yInterval, child.position.z);
                child.localScale = Vector3.zero;
            }
        }

        public void OnValidate()
        {
            blockSize.x = Mathf.Round(blockSize.x);
            blockSize.y = Mathf.Round(blockSize.y);
            blockSize.z = Mathf.Round(blockSize.z);
        }
       
        public void LauchLerp(DirLerpState dir,float waitTime = 0)
        {
            float[] timer = new float[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                timer[i] = Random.Range(-0.2f, 0.2f);
                StartCoroutine(LerpItem(timer[i], i, dir, waitTime));
            }
        }
        IEnumerator LerpItem(float timer,int i, DirLerpState dir,float waitTime = 0)
        {
            yield return new WaitForSeconds(waitTime);

                    // vibrating  
            if (dir  == DirLerpState.Down)
            {
                float vibratingTimer = Random.Range(0,0.25f);
                while (vibratingTimer < 1)
                {
                    vibratingTimer += Time.deltaTime;
                    VibrateBeforeFalling(i);
                    yield return null;
                }
            }
            Transform child = transform.GetChild(i);

            // Falling  
            if (dir == DirLerpState.Down && child.GetComponent<PlatformGameplay>())
                    child.GetComponent<PlatformGameplay>().enabled = false;

            float angle = Random.Range(100, 179);
            if (angle %2>0)
                angle = Random.Range(-100, -179);
            while (timer < 1)
            {
                RemoveUnwantedPlayer();
                timer += Time.deltaTime * 0.95f;
                float factor = Ease.Evaluate(Ease.EASE_TYPE.BOUNCE_OUT, timer);
                
                Vector3 nextPosition = child.position;
                if (dir == DirLerpState.Up)
                {
                    nextPosition.y = Mathf.Lerp(baseYPos[i] - yInterval, baseYPos[i], factor);
                    child.localScale = Vector3.Lerp(Vector3.zero, baseScale[i], factor);
                    child.rotation = Quaternion.Lerp(baseOrientation[i] * Quaternion.AngleAxis(angle, Vector3.up), baseOrientation[i], factor);
                }
                else
                {
                    nextPosition.y = Mathf.Lerp(baseYPos[i], baseYPos[i] - yInterval, factor);
                    child.localScale = Vector3.Lerp(baseScale[i], Vector3.zero, factor);
                    child.rotation = Quaternion.Lerp(baseOrientation[i], baseOrientation[i] * Quaternion.AngleAxis(angle, Vector3.up), factor);
                }
                child.position = nextPosition;

                yield return null;
            }
            if (dir == DirLerpState.Up&& child.GetComponent<PlatformGameplay>())
                child.GetComponent<PlatformGameplay>().enabled = true;

            yield return null;
        }

        public void VibrateBeforeFalling(int iId)
        {
            RemoveUnwantedPlayer();
            Transform child = transform.GetChild(iId);
            float vibratingVar = Random.Range(0.0f, 1.0f) *0.75f;
            Vector3 newPos = child.position;
            newPos.y = baseYPos[iId] + vibratingVar;
            child.position = newPos;

        }


        void RemoveUnwantedPlayer()
        {
            Player player = transform.GetComponentInChildren<Player>();
            if (player != null)
                player.transform.parent = null;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, Vector3.Scale(blockSize, RunnerLevelGenerator.defaultBlockSize));
        }

    }
    
}
