using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    public GameObject prefab;
    private SpatialHash hash = new();
    private List<Movement> scene = new();
    private int size = 2;
    //private List<double> sceneX1 = new List<double>();
    //private List<double> sceneX2 = new List<double>();

    // Start is called before the first frame update
    void Start()
    {
        hash.Initialize();
        int c = 0;
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                scene.Add(Instantiate(prefab, new Vector3(2 * x - size, 0, 2 * z - size), Quaternion.identity).GetComponent<Movement>());
                //sceneX1.Add(x - 0.5); //find some way to get size
                //sceneX2.Add(x + 0.5);
                scene[c].SetVelocity(UnityEngine.Random.Range(-9.0f, 9.0f), 0, UnityEngine.Random.Range(-9.0f, 9.0f));
                scene[c].SetPosition(2 * x - 5, 0, 2 * z - 5);
                scene[c].gameObject.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
                c++;
            }
        }

        //scene.Add(Instantiate(prefab, new Vector3(-7, 0, -7), Quaternion.identity).GetComponent<Movement>());
        //scene.Add(Instantiate(prefab, new Vector3(7, 0, 7), Quaternion.identity).GetComponent<Movement>());
        //scene[0].SetVelocity(1, 0, 1);
        //scene[1].SetVelocity(-1, 0, -1);
        //scene[1].gameObject.GetComponent<Renderer>().material.color = Color.red;    

        foreach (Movement m in scene)
        {
            hash.AddObject(m);
        }
    }

    // Update is called once per frame
    void Update()
    {
        hash.Clear();
        foreach (Movement m in scene)
        {
            hash.AddObject(m);
        }
        TestCollision();
    }

    void TestCollision()
    {
        List<CollisionPair> list = hash.PotentialCollisions();
        if (list.Count > 0)
        {
            foreach (CollisionPair p in list)
            {
                int side = CheckCollision(p);
                if (side != 0) { ActCollision(p, side); }
            }
        }
    }

    private void ActCollision(CollisionPair p, int order)
    {
        Vector3d temp1 = p.m1.GetVelocityVec();
        Vector3d temp2 = p.m2.GetVelocityVec();
        Vector3d temp3 = p.m1.GetPositionVec();
        Vector3d temp4 = p.m2.GetPositionVec();

        if (order == 1) // m1 up
        {
            p.m1.SetVelocity(temp1.x, temp1.y, temp2.z);
            p.m2.SetVelocity(temp2.x, temp2.y, temp1.z);
            p.m1.SetPosition(temp3.x, temp3.y, temp4.z + 1);
        }
        else if (order == 2) // m1 right
        {
            p.m1.SetVelocity(temp2.x, temp1.y, temp1.z);
            p.m2.SetVelocity(temp1.x, temp2.y, temp2.z);
            p.m1.SetPosition(temp4.x + 1, temp3.y, temp3.z);
        }
        else if (order == 3) // m1 down
        {

            p.m1.SetVelocity(temp1.x, temp1.y, temp2.z);
            p.m2.SetVelocity(temp2.x, temp2.y, temp1.z);
            p.m1.SetPosition(temp3.x, temp3.y, temp4.z - 1);
        }
        else if (order == 4) // m1 left
        {

            p.m1.SetVelocity(temp2.x, temp1.y, temp1.z);
            p.m2.SetVelocity(temp1.x, temp2.y, temp2.z);
            p.m1.SetPosition(temp4.x - 1, temp3.y, temp3.z);
        }
    }

    // checks which m1 corner is inside m2
    private int CheckCollision(CollisionPair p)
    {
        double m1x = p.m1.GetX();
        double m1z = p.m1.GetZ();
        double m2x = p.m2.GetX();
        double m2z = p.m2.GetZ();
        if ((m2x - 0.5 <= m1x - 0.5) && (m1x - 0.5 <= m2x + 0.5)) // top left
        {
            if ((m2z - 0.5 <= m1z + 0.5) && (m1z + 0.5 <= m2z + 0.5)) { return CheckEdge(m1x - 0.5, m1z + 0.5, m2x + 0.5, m2z - 0.5); }
        }
        if ((m2x - 0.5 <= m1x + 0.5) && (m1x + 0.5 <= m2x + 0.5)) // top right
        {
            if ((m2z - 0.5 <= m1z + 0.5) && (m1z + 0.5 <= m2z + 0.5)) { return CheckEdge(m1x + 0.5, m1z + 0.5, m2x - 0.5, m2z - 0.5); }
        }
        if ((m2x - 0.5 <= m1x - 0.5) && (m1x - 0.5 <= m2x + 0.5)) // bottom left
        {
            if ((m2z - 0.5 <= m1z - 0.5) && (m1z - 0.5 <= m2z + 0.5)) { return CheckEdge(m1x - 0.5, m1z - 0.5, m2x + 0.5, m2z + 0.5); }
        }
        if ((m2x - 0.5 <= m1x + 0.5) && (m1x + 0.5 <= m2x + 0.5)) // bottom right
        {
            if ((m2z - 0.5 <= m1z - 0.5) && (m1z - 0.5 <= m2z + 0.5)) { return CheckEdge(m1x + 0.5, m1z - 0.5, m2x - 0.5, m2z + 0.5); }
        }
        return 0;
    }

    // top,right,bottom,left -> 1,2,3,4
    private int CheckEdge(double x1, double y1, double x2, double y2)
    {
        if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1)) // z side
        {
            if (y2 - y1 > 0) { return 1; }
            return 3;
        }
        if (x2 - x1 > 0) { return 2; }
        return 4;
    }
}
