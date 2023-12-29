using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Protobuf;

public class MapControl : MonoBehaviour {
    public GameObject mapFa;
    public int[][] map = new int[50][] {
        new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],
        new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],
        new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],
        new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],
        new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],new int[50],
        };
    // Start is called before the first frame update
    void Start()
    {
        FileStream fs = new FileStream("D:\\UPro\\THUAI7\\map.txt", FileMode.OpenOrCreate, FileAccess.Read);
        for (int i = 0; i < 50; i++) {
            for (int j = 0; j < 50; j++) {
                map[i][j] = fs.ReadByte() - '0';
                while(map[i][j] < 0 || map[i][j] > 9)
                    map[i][j] = fs.ReadByte() - '0';
            }
        }
        fs.Close();
        Debug.Log(map[0][0]);
        Debug.Log(map[0][1]);
        Debug.Log(map[0][2]);
        Debug.Log(map[1][0]);
        Debug.Log(map[2][0]);
        //fs = new FileStream("mapRead.txt", FileMode.OpenOrCreate, FileAccess.Write);
        //for (int i = 0; i < 50; i++) {
        //    for (int j = 0; j < 50; j++) {
        //        fs.WriteByte((byte)map[i][j]);
        //    }
        //}
        fs.Close();
        for (int i = 0; i < 50; i++) {
            for (int j = 0; j < 50; j++) {
                Instantiate(ParaDefine.GetInstance().PT((PlaceType)map[i][j]), new Vector3(j, 50 - i, 0), Quaternion.identity, mapFa.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
