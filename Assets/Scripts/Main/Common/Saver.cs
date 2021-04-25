using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Saver
{
    private static Saver _instance;
    public static Saver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Saver();
            }
            return _instance;
        }
    }

    private string path;
    private string filePath;

    private const int dataLength = 5;
    public int[] data { get; private set; }

    private Saver()
    {
        path = Application.persistentDataPath + "/Data";
        filePath = path + "/save.json";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
            Save(new int[dataLength]);
        }
    }

    public void AddData(int score)
    {
        for (int i = 0; i < data.Length; i++) 
        {
            if (score > data[i]) 
            {
                for (int j = data.Length - 1; j > i; j--) 
                {
                    data[j] = data[j - 1];
                }
                data[i] = score;
                
                Save(data);
                break;
            }
        }
    }

    public int[] GetData()
    {
        StreamReader reader = new StreamReader(filePath);
        string str = reader.ReadToEnd();
        reader.Close();
        int[] data = JsonUtility.FromJson<SaveData>(str).data;
        data = Cut(data);
        Sort(data);
        this.data = data;
        return this.data;
    }

    private void Save(int[] data)
    {
        SaveData d = new SaveData();
        d.data = new int[dataLength];
        d.data = Cut(data);
        Sort(d.data);
        this.data = d.data;
        StreamWriter writer = new StreamWriter(filePath);
        writer.Write(JsonUtility.ToJson(d));
        writer.Close();
    }

    private void Sort(int[] array)
    {
        for (int j = 0; j < array.Length; j++)
        {
            for (int i = 0; i < array.Length - 1 - j; i++)
            {
                if (array[i] < array[i + 1])
                {
                    int t = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = t;
                }
            }
        }
    }
    private int[] Cut(int[] array)
    {
        int[] outData = new int[dataLength];

        for (int i = 0; i < outData.Length; i++)
        {
            if (i >= array.Length)
            {
                outData[i] = 0;
            }
            else
            {
                outData[i] = array[i];
            }
        }
        return outData;
    }

    [System.Serializable]
    private class SaveData{
        public int[] data;//成绩由大到小排列
    }
}

