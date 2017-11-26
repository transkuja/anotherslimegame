using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tampax
{
    [SerializeField]
    public CollectableType evolutionType;
    [SerializeField]
    public Transform gameplayRoomStarter;
}

public enum Shapes { None, Circle, Line, Grid }
public class SpawnManager : MonoBehaviour{

    // Spawned Items Location
    public Dictionary<int, Transform> dicSpawnItemsLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnItems = 0;

    // Spawned Monsters Location
    public Dictionary<int, Transform> dicSpawnMonstersLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnMonsters = 0;

    // Spawned Iles Locations
    public Dictionary<int, Transform> dicSpawnIlesLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnIles = 0;

    // Frame control on item
    private int spawnedItemsCountAtTheSameTime = 0;
    private const int MAXSPAWNITEMSCOUNTATTHESAMETIME = 1000;

    // Frame control on monster
    private int spawnedMonsterCountAtTheSameTime = 0;
    private const int MAXSPAWNMONSTERSCOUNTATTHESAMETIME = 20;

    // Frame control on iles
    private int spawnedIlesCountAtTheSameTime = 0;
    private const int MAXSPAWNILESSCOUNTATTHESAMETIME = 4;

    private List<CollectableType> evolutionsLeftToSpawn = new List<CollectableType>();

    // Gameplay room starters
    public List<Tampax> gameplayRoomStarters = new List<Tampax>();

    private static SpawnManager instance;

    public int SpawnedMonsterCount
    {
        get
        {
            return spawnedMonsterCountAtTheSameTime;
        }

        set
        {
            spawnedMonsterCountAtTheSameTime = value;
        }
    }

    public int SpawnedItemsCount
    {
        get
        {
            return spawnedItemsCountAtTheSameTime;
        }

        set
        {
            spawnedItemsCountAtTheSameTime = value;
        }
    }

    public void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
        ResetInstance();
    }

    public void ResetInstance()
    {
        instance.spawnedItemsCountAtTheSameTime = 0;
        instance.spawnedMonsterCountAtTheSameTime = 0;
        instance.spawnedIlesCountAtTheSameTime = 0;
        instance.evolutionsLeftToSpawn.Add(CollectableType.AgileEvolution1);
        instance.evolutionsLeftToSpawn.Add(CollectableType.PlatformistEvolution1);
        instance.evolutionsLeftToSpawn.Add(CollectableType.StrengthEvolution1);
        instance.evolutionsLeftToSpawn.Add(CollectableType.GhostEvolution1);
        Utils.Shuffle(instance.evolutionsLeftToSpawn);
    }

    public static SpawnManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    public int SpawnedIlesCountAtTheSameTime
    {
        get
        {
            return spawnedIlesCountAtTheSameTime;
        }

        set
        {
            spawnedIlesCountAtTheSameTime = value;
        }
    }

    private void SpawnItem(int idLocation, CollectableType myItemType, bool forceSpawn = false)
    {

        if (instance.dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }


        SpawnedItemsCount++;
        ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
            instance.dicSpawnItemsLocations[idLocation].transform.position,
            instance.dicSpawnItemsLocations[idLocation].transform.rotation,
            instance.dicSpawnItemsLocations[idLocation].transform,
            myItemType
        ).GetComponent<Collectable>().Init(0);
    }

    private void SpawnCircleShapedItems(int idLocation, int nbItems, CollectableType myItemType, bool forceSpawn = false, float circleRadius = 1.0f)
    {
        if (instance.dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }
        for (int i = 0; i < nbItems; i++)
        {
            SpawnedItemsCount++;
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                GetVector3ArrayOnADividedCircle(instance.dicSpawnItemsLocations[idLocation].transform.position, circleRadius, nbItems, Axis.XZ)[i],
                instance.dicSpawnItemsLocations[idLocation].transform.rotation,
                instance.dicSpawnItemsLocations[idLocation].transform,
                myItemType
            ).GetComponent<Collectable>().Init(0);
        }
    }

    private void SpawnLineShapedItems(int idLocation, int nbItems, CollectableType myItemType, bool forceSpawn = false)
    {
        if (instance.dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }
        for (int i = 0; i < nbItems; i++)
        {
            SpawnedItemsCount++;
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                GetVector3ArrayOnLine(instance.dicSpawnItemsLocations[idLocation].transform.position, instance.dicSpawnItemsLocations[idLocation].transform.forward, nbItems)[i],
                instance.dicSpawnItemsLocations[idLocation].transform.rotation,
                instance.dicSpawnItemsLocations[idLocation].transform,
                myItemType
            ).GetComponent<Collectable>().Init(0);
        }
    }

    private void SpawnGridShapedItems(int idLocation, int nbItems, CollectableType myItemType, bool forceSpawn = false)
    {
        if (instance.dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }

        // TMP heuristic
        int ligne = Mathf.RoundToInt(Mathf.Sqrt(nbItems));
        int colonne = Mathf.FloorToInt(nbItems/ ligne);
        for (int i = 0; i < colonne; i++)
        {
            for (int j = 0; j < ligne; j++)
            {
                SpawnedItemsCount++;
                ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                    GetVector3ArrayOnAGrid(instance.dicSpawnItemsLocations[idLocation].transform.position, instance.dicSpawnItemsLocations[idLocation].transform.forward, ligne, colonne)[i,j],
                    instance.dicSpawnItemsLocations[idLocation].transform.rotation,
                    instance.dicSpawnItemsLocations[idLocation].transform,
                    myItemType
                ).GetComponent<Collectable>().Init(0);

            }
        }
    }



    private void SpawnMonster(int idLocation, MonsterType myMonsterType, bool forceSpawn = false)
    {
        if (instance.dicSpawnMonstersLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedMonsterCount == MAXSPAWNMONSTERSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max monster reach");
            return;
        }

        SpawnedMonsterCount++;
        ResourceUtils.Instance.refPrefabMonster.SpawnMonsterInstance(
            instance.dicSpawnMonstersLocations[idLocation].transform.position,
            instance.dicSpawnMonstersLocations[idLocation].transform.rotation,
            instance.dicSpawnMonstersLocations[idLocation].transform,
            myMonsterType
        );
    }

    // add a transformation to spawn manager, retrieve the id where the spawn location was inserted
    // call before everything
    public int RegisterSpawnItemLocation(Transform mySpawnLocation, CollectableType myItemType, bool needSpawn = false, bool forceSpawn = false, Shapes shapes = Shapes.None, int nbItems = 1, float circleRadius = 1.0f)
    {
        instance.dicSpawnItemsLocations.Add(lastInsertedKeySpawnItems, mySpawnLocation);

        if (needSpawn)
        {

            switch (shapes)
            {
                case Shapes.None:
                    SpawnItem(lastInsertedKeySpawnItems, myItemType, forceSpawn);
                    break;
                case Shapes.Circle:
                    SpawnCircleShapedItems(lastInsertedKeySpawnItems, nbItems, myItemType, forceSpawn, circleRadius);
                    break;
                case Shapes.Line:
                    SpawnLineShapedItems(lastInsertedKeySpawnItems, nbItems, myItemType, forceSpawn);
                    break;
                case Shapes.Grid:
                    SpawnGridShapedItems(lastInsertedKeySpawnItems, nbItems, myItemType, forceSpawn);
                    break;
            }
 
        }

        return lastInsertedKeySpawnItems++;
    }
    public int RegisterSpawnIleLocation(Transform mySpawnLocation, GameObject associatedShelter, bool needSpawn = false, bool forceSpawn = false)
    {
        instance.dicSpawnIlesLocations.Add(lastInsertedKeySpawnIles, mySpawnLocation);

        if (needSpawn)
        {
            SpawnIle(lastInsertedKeySpawnIles, associatedShelter, forceSpawn);
        }

        return lastInsertedKeySpawnIles++;
    }

    private void SpawnIle(int idLocation, GameObject associatedShelter, bool forceSpawn)
    {
        if (instance.dicSpawnIlesLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedIlesCountAtTheSameTime == MAXSPAWNILESSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max monster reach");
            return;
        }

        SpawnedIlesCountAtTheSameTime++;
        GameObject spawnedIsland = ResourceUtils.Instance.refPrefabIle.SpawnIleInstance(
            instance.dicSpawnIlesLocations[idLocation].transform.position,
            instance.dicSpawnIlesLocations[idLocation].transform.rotation,
            instance.dicSpawnIlesLocations[idLocation].transform
        );

        Transform evolutionSpawn = spawnedIsland.transform.GetChild(0);
        if (!evolutionSpawn.name.Contains("Evolution"))
        {
            Debug.LogWarning("WARNING: Spawn evolution on island has been removed or renamed or moved or Seb lied.");
            return;
        }

        CollectableType evolutionType = GetNextEvolutionType();
        if (evolutionType == CollectableType.Size)
            return;

        ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
            evolutionSpawn.position,
            evolutionSpawn.rotation,
            evolutionSpawn,
            evolutionType
        ).GetComponent<Collectable>().Init(0);

        InitTeleporter initTeleporterComponent = associatedShelter.GetComponentInChildren<InitTeleporter>();
        initTeleporterComponent.evolutionType = evolutionType;

        Utils.Shuffle(gameplayRoomStarters);
        foreach (Tampax tmp in gameplayRoomStarters)
        {
            if (tmp.evolutionType == evolutionType)
                associatedShelter.GetComponentInChildren<PlatformGameplay>().teleporterTarget = tmp.gameplayRoomStarter;
        }
    }

    CollectableType GetNextEvolutionType()
    {
        if (instance.evolutionsLeftToSpawn == null || instance.evolutionsLeftToSpawn.Count == 0)
        {
            Debug.LogWarning("The four evolutions have already been spawned.");
            return CollectableType.Size;
        }

        CollectableType evolutionType = instance.evolutionsLeftToSpawn[0];
        instance.evolutionsLeftToSpawn.RemoveAt(0);
        return evolutionType;
    }

    // call on destroy on a spawn item
    public void UnregisterSpawnItemLocation(int idToUnregister)
    {
        instance.dicSpawnItemsLocations.Remove(idToUnregister);
    }


    // add a transformation to spawn manager, retrieve the id where the spawn location was inserted
    // call before everything
    public int RegisterSpawnMonsterLocation(Transform mySpawnLocation, MonsterType myMonsterType, bool needSpawn = false, bool forceSpawn = false)
    {
        instance.dicSpawnMonstersLocations.Add(lastInsertedKeySpawnMonsters, mySpawnLocation);

        if (needSpawn)
        {
            SpawnMonster(lastInsertedKeySpawnMonsters, myMonsterType, forceSpawn);
        }

        return lastInsertedKeySpawnMonsters++;
    }

    // call on destroy on a spawn monster
    public void UnregisterSpawnMonsterLocation(int idToUnregister)
    {
        instance.dicSpawnMonstersLocations.Remove(idToUnregister);
    }

    // call on destroy on a spawn ile
    public void UnregisterSpawnIleLocation(int idToUnregister)
    {
        instance.dicSpawnIlesLocations.Remove(idToUnregister);
    }

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

        for (int i=0; i <nbPoint; i++)
        {
            toReturn[i] = (10 * i * (direction / nbPoint)) + origin;
        }
        return toReturn;
    }

    public static Vector3[,] GetVector3ArrayOnAGrid(Vector3 origin, Vector3 direction, int nbLine = 1, int nbColonne =1 )
    {
        Vector3[,] toReturn = new Vector3[nbColonne, nbLine];
        Vector3[] line = new Vector3[nbLine]; 
        for (int i = 0; i < nbLine; i++)
        {
            line = SpawnManager.GetVector3ArrayOnLine(origin, direction, nbLine);
        }

        for (int i = 0; i < nbColonne; i++)
        {
            for (int j = 0;j < nbLine; j++)
            {
                toReturn[i, j] = line[j] + (10 * i *(new Vector3(1, 0, 0) / nbColonne));
            }
        }
        return toReturn;
    }

    public static float GetAngleForIndexedDividedCircle(int index, int divider)
    {
        return (360.0f / divider) * index;
    }

}
