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
    public BaseScriptableEnumValueContainer playerTypes;


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


        //Can iterate enum container to access all values, for usecases like typeof().GetEnumNames().
        // PS looking for a better workflow where enum instance can directly acces those values.
        string values ="Player Types:";
        playerTypes.Ids.ForEach(x => values += $"{x} \n");
        Debug.Log(values);
        

    }


}
