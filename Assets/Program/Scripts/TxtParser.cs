using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Global;
public static class TxtParser{
    private static string path = Application.dataPath;
    //private static string path = "Assets/Resources/Data/";
    public static void LoadData(string fileName)
    {
        FileStream fs = new FileStream(path + fileName + ".txt", FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string source = sr.ReadLine();
        string[] value;
        
        while(source!=null)
        {
            value = source.Split(',');
            int index = int.Parse(value[0]);
            int layer1_1Percent = int.Parse(value[1]);
            int layer1_2Percent = int.Parse(value[2]);
            int layer2_1Percent = int.Parse(value[3]);
            int layer2_2Percent = int.Parse(value[4]);
            int layer3_1Percent = int.Parse(value[5]);
            int layer3_2Percent = int.Parse(value[6]);
            int layer4_1Percent = int.Parse(value[7]);
            int layer4_2Percent = int.Parse(value[8]);
            int layer1_Count = int.Parse(value[9]);
            int layer2_Count = int.Parse(value[10]);
            int layer3_Count = int.Parse(value[11]);
            int layer4_Count = int.Parse(value[12]);
            float speed = float.Parse(value[13]);
            GLOBAL_VAR.Phase temp = GLOBAL_VAR.phaseInfo[index];
            temp.layersInfo[0].oneLayerPercent = layer1_1Percent;
            temp.layersInfo[0].twoLayerPercent = layer1_2Percent;
            temp.layersInfo[1].oneLayerPercent = layer2_1Percent;
            temp.layersInfo[1].twoLayerPercent = layer2_2Percent;
            temp.layersInfo[2].oneLayerPercent = layer3_1Percent;
            temp.layersInfo[2].twoLayerPercent = layer3_2Percent;
            temp.layersInfo[3].oneLayerPercent = layer4_1Percent;
            temp.layersInfo[3].twoLayerPercent = layer4_2Percent;
            temp.layerCount[0] = layer1_Count;
            temp.layerCount[1] = layer2_Count;
            temp.layerCount[2] = layer3_Count;
            temp.layerCount[3] = layer4_Count;
            temp.speed = speed;
            if (value.Length==0)
            {
                sr.Close();
                return;
            }
            source = sr.ReadLine();
        }
        
    }
}
