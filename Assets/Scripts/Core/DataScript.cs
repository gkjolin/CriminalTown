﻿using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.ObjectModel;


[Serializable]
public class SData
{
    public int[] itemsCount;
    public int money;
}

[Serializable]
public class ChData
{
    private List<Character> campCharacters;
    private List<Character> panelCharacters;

    public ReadOnlyCollection<Character> PanelCharacters { get { return panelCharacters.AsReadOnly(); } }
    public List<Character> CampCharacters { get { return campCharacters; } }

    public delegate void ChDataEvent(Character character);

    public event ChDataEvent OnRemoveEvent;
    public event ChDataEvent OnAddEvent;

    //Constructor
    public ChData()
    {
        campCharacters = new List<Character>();
        panelCharacters = new List<Character>();
        OnAddEvent = delegate { };
        OnRemoveEvent = delegate { };
    }

    public void RemoveCharacter(Character character)
    {
        panelCharacters.Remove(character);
        OnRemoveEvent(character);
    }

    public void AddCharacter(Character character)
    {
        panelCharacters.Add(character);
        OnAddEvent(character);
    }

}

[Serializable]
public class PData
{
    public bool[] isItemAvailable;
    public bool isBlackMarketAvailable;
    public bool isBanditCampAvailable;
    public bool isHospitalAvailable;
    public bool isPoliceStationAvailable;
    public bool isRobberyAvailable; //В дальнейшем добавить виды ограблений

    public int authority;
    public int day;
}

[Serializable]
public class EData
{
    public int policeKnowledge;

    public Dictionary<RobberyType, Dictionary<int, Robbery>> robberiesData;
}

public class DataScript : MonoBehaviour
{
    public static SData sData = new SData();
    public static ChData chData = new ChData();
    public static PData pData = new PData();
    public static EData eData = new EData();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ItemsOptions.GetItemsCollectionData();
        CharactersOptions.GetCharactersCollectionData();
        RobberiesOptions.GetRobberiesCollectionData();
        NightEventsOptions.GetNightEventsCollectionData();
        TraitsOptions.GetTraitsCollectionData();
    }

    public static bool CheckDataFiles()
    {
        return
            (File.Exists(Application.persistentDataPath + "/sourcesDataFile.dat") &&
            File.Exists(Application.persistentDataPath + "/charactersDataFile.dat") &&
            File.Exists(Application.persistentDataPath + "/progressDataFile.dat")) &&
            File.Exists(Application.persistentDataPath + "/eventsDataFile.dat");
    }

    public static void AssignDefaultData()
    {
        //sData
        sData.itemsCount = new int[ItemsOptions.totalAmount];
        for (int i = 0; i < ItemsOptions.totalAmount; i++) sData.itemsCount[i] = 5;
        sData.money = 1000000;

        //chData
        CommonCharacter arrestedChar = CharactersOptions.GetRandomCommonCharacter(5);
        arrestedChar.AddToPolice();
        chData.AddCharacter(arrestedChar);

        CommonCharacter hospitalChar = CharactersOptions.GetRandomCommonCharacter(6);
        hospitalChar.AddToHospital();
        chData.AddCharacter(hospitalChar);


        //chData.panelCharacters.Add(CharactersOptions.GetRandomCommonCharacter(8));
        //chData.panelCharacters.Add(CharactersOptions.GetRandomCommonCharacter(9));
        //chData.panelCharacters.Add(CharactersOptions.GetSpecialCharacter(9, 0));
        chData.AddCharacter(CharactersOptions.GetSpecialCharacter(9, 1));

        //eData
        eData.policeKnowledge = 0;

        RobberiesOptions.GetNewRobberies();

        //pData
        pData.isItemAvailable = new bool[ItemsOptions.totalAmount];
        for (int i = 0; i < ItemsOptions.totalAmount; i++) pData.isItemAvailable[i] = true;
        pData.authority = 9;

        CharactersOptions.FillCampCells();
        SaveAll();
    }

    public static void LoadData()
    {
        sData = (SData)LoadData("/sourcesDataFile.dat");
        chData = (ChData)LoadData("/charactersDataFile.dat");
        pData = (PData)LoadData("/progressDataFile.dat");
        eData = (EData)LoadData("/eventsDataFile.dat");
    }

    public static void SaveSourcesData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/sourcesDataFile.dat");
        bf.Serialize(dataFile, sData);
        dataFile.Close();
    }

    public static void SaveCharactersData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/charactersDataFile.dat");
        bf.Serialize(dataFile, chData);
        dataFile.Close();
    }

    public static void SaveProgressData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/progressDataFile.dat");
        bf.Serialize(dataFile, pData);
        dataFile.Close();
    }

    public static void SaveEventsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/eventsDataFile.dat");
        bf.Serialize(dataFile, eData);
        dataFile.Close();
    }

    public static void SaveAll()
    {
        SaveCharactersData();
        SaveEventsData();
        SaveProgressData();
        SaveSourcesData();
    }

    private static object LoadData(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
        object data = bf.Deserialize(dataFile);
        dataFile.Close();
        return data;
    }
}
