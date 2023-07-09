using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableEnumSystem;
using System;

public class ScriptableEnumDemo : MonoBehaviour
{
    //Declare like normal enum, The values will be drawn by the values set in scriptable object.
    public ScriptableEnum playerType;
    public ScriptableEnum playerType2;
    
    public ScriptableEnum enemyType;

    //ScriptableEnumsContainer for iterating all values.
    public ScriptableEnumValueContainer playerTypes;


    private void Start()
    {
        //xyzScEnum.Value to utilize the value.
        Debug.Log($"{nameof(playerType)} : {playerType.Value}"); 
        Debug.Log($"{nameof(enemyType)} : {enemyType.Value}");

        //Utilises HashCode based on values instead of actual object.
        //caches hash for optimum performance during HashSet and DictionaryLookups.
        HashSet<ScriptableEnum> playerEnums = new();
        playerEnums.Add(playerType);
        playerEnums.Add(playerType);
        Debug.Log($"{nameof(playerEnums)} : Total count{playerEnums.Count}");


        //Also implements EquailtyComparers based on values instead of reference
        //for easy comparision accross instances.
        Debug.Log($"{nameof(playerType)} == {nameof(playerType2)} : Are Same ? {playerType.Equals(playerType2)}");


        //Can iterate enum container to access all values, for usecases like typeof().GetEnumNames().
        // PS looking for a better workflow where enum instance can directly acces those values.
        string values ="Player Types:";
        playerTypes.Ids.ForEach(x => values += $"{x} \n");
        Debug.Log(values);
        

    }


}
