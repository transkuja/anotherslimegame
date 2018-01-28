using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner3D
{

    public enum Direction
    {
        Up,
        Down,
    }

    public class RunnerBlocs : MonoBehaviour {

        float[] baseYPos;
        Vector3[] baseScale;

        static float yInterval = 10;
        [SerializeField] Vector3 blockSize;
        public void Awake()
        {
            baseYPos = new float[transform.childCount];
            baseScale = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount;i++)
            {
                    // register base position
                baseYPos[i] = transform.position.y;
                baseScale[i] = transform.localScale;
                // goDown at beginning : 
            }
        }
        public void Start()
        {
            Invoke("TpDown", 0.1f);
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
        public void LauchLerp(Direction dir)
        {
            StartCoroutine(LerpDown(dir));
        }
        IEnumerator LerpDown(Direction dir)
        {
            float timer = 0;
            while (timer < 1) 
            {
                timer += Time.deltaTime*0.85f;
                float factor = Ease.Evaluate(Ease.EASE_TYPE.BOUNCE_OUT, timer);
                for (int i = 0; i < transform.childCount;i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.GetComponent<PlatformGameplay>())
                    {
                        child.GetComponent<PlatformGameplay>().IsMoving = false;
                    }
                    Vector3 nextPosition = child.position;
                    if (dir == Direction.Up)
                    {
                        nextPosition.y = Mathf.Lerp(baseYPos[i] - yInterval, baseYPos[i], factor);
                        child.localScale = Vector3.Lerp(Vector3.zero, baseScale[i], factor);
                    }
                    else
                    {
                        nextPosition.y = Mathf.Lerp(baseYPos[i], baseYPos[i] - yInterval, factor);
                        child.localScale = Vector3.Lerp(baseScale[i], Vector3.zero, factor);
                    }
                    child.position = nextPosition;
                }
                yield return null;
            }
            if (dir == Direction.Up)
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.GetComponent<PlatformGameplay>())
                        child.GetComponent<PlatformGameplay>().IsMoving = true;
                }
            yield return null;
        }


    }
}
