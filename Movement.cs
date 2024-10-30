using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector3d v = new Vector3d(0, 0, 0);
    public Vector3d pos = new Vector3d(0, 0, 0);
    private double previousTime;
    private double delta;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 q = transform.position;
        pos = new Vector3d(q.x,q.y,q.z);
    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.timeAsDouble - previousTime;
        // global time in initializer?
        Boundary(ref v);
        pos.x += v.x * delta;
        pos.z += v.z * delta;
        transform.position = new Vector3((float)(pos.x), (float)(pos.y), (float)(pos.z));

        previousTime = Time.timeAsDouble;

        //Debug.Log(transform.position.x - pos.x);
    }

    bool Boundary(ref Vector3d current)
    {
        bool active = false;
        if (pos.x > 9 || pos.x < -9)
        {
            current.x = -current.x;
            pos.x = (pos.x > 9) ? 9 : -9;
            active = true;
        }
        if (pos.z > 9 || pos.z < -9)
        {
            current.z = -current.z;
            pos.z = (pos.z > 9) ? 9 : -9;
            active = true;
        }
        return active;
    }

    public void SetPosition(double x, double y, double z)
    {
        pos.x = x;
        pos.y = y;
        pos.z = z;
    }

    public void SetVelocity(double x, double y, double z)
    {
        v.x = x;
        v.y = y;
        v.z = z;
        //Debug.Log("velocity: " + v.x + " " + v.y + " " + v.z);
    }

    public void SetVelocityVec(Vector3d a)
    {
        v = a;
    }

    public Vector3d GetVelocityVec()
    {
        return v;
    }

    public Vector3d GetPositionVec()
    {
        return pos;
    }

    public double GetX()
    {
        return pos.x;
    }

    public double GetY()
    {
        return pos.y;
    }

    public double GetZ()
    {
        return pos.z;
    }
}