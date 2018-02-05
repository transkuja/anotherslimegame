using UnityEngine;

// Use to have shaped spawn
[ExecuteInEditMode]
public class SpawnEditor : MonoBehaviour {

    public enum SpawnType { Points, Money };
    public SpawnType spawnType;


    public Shapes shape;
    public int nbItems = 1;
    public float circleRadius = 1.0f;

    private CollectableType myItemType;

    // Use this for initialization
    void Start () {
        if (spawnType == SpawnType.Points)
            myItemType = CollectableType.Points;
        if (spawnType == SpawnType.Money)
            myItemType = CollectableType.Money;

        switch (shape)
        {
            case Shapes.None:
                SpawnItem(myItemType);
                break;
            case Shapes.Circle:
                SpawnCircleShapedItems(nbItems, myItemType, circleRadius);
                break;
            case Shapes.Line:
                SpawnLineShapedItems(nbItems, myItemType);
                break;
            case Shapes.Grid:
                SpawnGridShapedItems( nbItems, myItemType);
                break;
        }
    }

    public enum Shapes { None, Circle, Line, Grid }

    // Problably should be moved to Utils
    #region Utils
    public enum Axis { XY, XZ, YZ };
    public static Vector3[] GetVector3ArrayOnADividedCircle(Vector3 center, float radius, int divider, Axis baseAXis)
    {
        Vector3[] toReturn = new Vector3[divider];
        for (int i = 0; i < toReturn.Length; i++)
        {
            switch (baseAXis)
            {
                case Axis.XY:
                    toReturn[i] = new Vector3(
                        center.x + (float)(Mathf.Cos(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                        center.y + (float)(Mathf.Sin(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                        center.z
                    );

                    break;
                case Axis.XZ:
                    toReturn[i] = new Vector3(
                            center.x + (float)(Mathf.Cos(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                            center.y,
                            center.z + (float)(Mathf.Sin(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius)
                        );

                    break;
                case Axis.YZ:
                    toReturn[i] = new Vector3(
                center.x,
                center.y + (float)(Mathf.Sin(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                center.z + (float)(Mathf.Cos(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius)
                 );

                    break;
            }
        }
        return toReturn;
    }

    public static Vector3[] GetVector3ArrayOnLine(Vector3 origin, Vector3 direction, int nbPoint, bool isCentered = false)
    {
        Vector3[] toReturn = new Vector3[nbPoint];

        if (isCentered)
        {
            origin = origin - (direction / 2f);
        }

        for (int i = 0; i < nbPoint; i++)
        {
            toReturn[i] = (10 * i * (direction / nbPoint)) + origin;
        }
        return toReturn;
    }

    public static Vector3[,] GetVector3ArrayOnAGrid(Vector3 origin, Vector3 direction, int nbLine = 1, int nbColonne = 1)
    {
        Vector3[,] toReturn = new Vector3[nbColonne, nbLine];
        Vector3[] line = new Vector3[nbLine];
        for (int i = 0; i < nbLine; i++)
        {
            line = GetVector3ArrayOnLine(origin, direction, nbLine);
        }

        for (int i = 0; i < nbColonne; i++)
        {
            for (int j = 0; j < nbLine; j++)
            {
                toReturn[i, j] = line[j] + (10 * i * (new Vector3(1, 0, 0) / nbColonne));
            }
        }
        return toReturn;
    }

    public static float GetAngleForIndexedDividedCircle(int index, int divider)
    {
        return (360.0f / divider) * index;
    }
    #endregion

    #region Items
    private void SpawnItem(CollectableType myItemType, bool forceSpawn = false)
    {
        ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
            transform.position,
            transform.rotation,
            transform,
            myItemType
        ).GetComponent<Collectable>().Init();
    }

    private void SpawnCircleShapedItems(int nbItems, CollectableType myItemType, float circleRadius = 1.0f)
    {
        for (int i = 0; i < nbItems; i++)
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                GetVector3ArrayOnADividedCircle(transform.position, circleRadius, nbItems, Axis.XZ)[i],
                transform.rotation,
                transform,
                myItemType
            ).GetComponent<Collectable>().Init();
        }
    }

    private void SpawnLineShapedItems(int nbItems, CollectableType myItemType)
    {
        for (int i = 0; i < nbItems; i++)
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                GetVector3ArrayOnLine(transform.position, transform.forward, nbItems)[i],
                transform.rotation,
                transform,
                myItemType
            ).GetComponent<Collectable>().Init();
        }
    }

    private void SpawnGridShapedItems(int nbItems, CollectableType myItemType)
    {
        // TMP heuristic
        int ligne = Mathf.RoundToInt(Mathf.Sqrt(nbItems));
        int colonne = Mathf.FloorToInt(nbItems / ligne);
        for (int i = 0; i < colonne; i++)
        {
            for (int j = 0; j < ligne; j++)
            {
                ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                    GetVector3ArrayOnAGrid(transform.position, transform.forward, ligne, colonne)[i, j],
                    transform.rotation,
                    transform,
                    myItemType
                ).GetComponent<Collectable>().Init();

            }
        }
    }
    #endregion 
}
