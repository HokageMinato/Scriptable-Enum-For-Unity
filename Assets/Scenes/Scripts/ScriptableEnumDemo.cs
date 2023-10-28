using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableEnumSystem;
using System;

public class ScriptableEnumDemo : MonoBehaviour
{
    //Declare like normal enum, The values will be drawn by the values set in scriptable object.
    
    public ScriptableEnum playerType; //Apply menu filter by setting inspector to 'Debug Mode' 
                                      //That will allow for filtered navigation.

    public ScriptableEnum playerType2;

    
    public ScriptableEnum enemyType;

                                                          //ScriptableEnumsContainer for iterating all values.
    public BaseScriptableEnumValueContainer playerTypes; //Any asset can be a container type and
                                                        //values can be fed from them.

    public string tankTypeStringValue;
    public ScriptableEnum tankTypeFromStringValue;


    public List<ScriptableEnum> instantiatedEnemyInstances = new List<ScriptableEnum>();

    private void Start()
    {

        //xyzScEnum.String to utilize the string representation.
        Debug.Log($"{nameof(playerType)} : {playerType.StringId}"); 
        Debug.Log($"{nameof(enemyType)} : {enemyType.StringId}");
        
        //xyzScEnum.FastId to utilize the faster int counterpart.
        Debug.Log($"{nameof(playerType)} : {playerType.FastId}"); 
        Debug.Log($"{nameof(enemyType)} : {enemyType.FastId}");

        //Utilises fastId for HashCode instead of actual object.
        //caches hash for optimum performance during HashSet and DictionaryLookups.
        HashSet<ScriptableEnum> playerEnums = new();
        playerEnums.Add(playerType);
        playerEnums.Add(playerType);
        Debug.Log($"{nameof(playerEnums)} : Total count{playerEnums.Count}");


        //Also implements EquailtyComparers based on values instead of reference
        //for easy comparision accross instances.
        //For comparision it also utilises FastID ensuring only an int comparision.
        Debug.Log($"{nameof(playerType)} == {nameof(playerType2)} : Are Same ? {playerType.Equals(playerType2)}");
        Debug.Log($"{nameof(playerType)} == {nameof(playerType2)} : Are Same ? {playerType == playerType2}");
        Debug.Log($"{nameof(playerType)} == {nameof(playerType2)} : Are Same ? {playerType != playerType2}");


        //Generate ScriptableEnum from pureStrings,
        //Will ensure same hash is assigned to this instance.
        tankTypeFromStringValue = new ScriptableEnum(tankTypeStringValue);

        //Also provides ability to Generate 'RuntimeId' with equal performance
        //These values should be unique, and will reflect at 'RuntimeGenerated' scriptable container.
        //NOTE: Never save FastID as prefs or any mode of persistant storage.
        //      Any reordering/addition/removal in container will lead to variance in fastId.
        instantiatedEnemyInstances.Add(ScriptableEnum.InstantiateRuntimeInstance(enemyType.StringId + "_1"));
        instantiatedEnemyInstances.Add(ScriptableEnum.InstantiateRuntimeInstance(enemyType.StringId + "_2"));
        instantiatedEnemyInstances.Add(ScriptableEnum.InstantiateRuntimeInstance(enemyType.StringId + "_3"));

        //Can iterate enum container to access all values, for usecases like typeof().GetEnumNames().
        // PS looking for a better workflow where enum instance can directly acces those values.
        string values ="Player Types:";
        playerTypes.Ids.ForEach(x => values += $"{x} \n");
        Debug.Log(values);
        

    }

    private void OnDisable()
    {
        instantiatedEnemyInstances.Clear();
    }


}
