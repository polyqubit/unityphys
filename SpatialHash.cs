using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct Pair
{
    public int x;
    public int y;

    public override bool Equals(object obj)
    {
        return obj is Pair pair &&
               x == pair.x &&
               y == pair.y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static bool operator ==(Pair left, Pair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Pair left, Pair right)
    {
        return !(left == right);
    }
}

public struct CollisionPair
{
    public Movement m1;
    public Movement m2;
}

//public class SpatialHash : MonoBehaviour
public struct SpatialHash
{
    private Dictionary<Pair, List<Movement>> buckets;
    private int cellsize;
    // Start is called before the first frame update

    public void Initialize()
    {
        buckets = new Dictionary<Pair, List<Movement>>();
        cellsize = 1;
    }

    public void AddObject(Movement m)
    {
        foreach (Pair p in ObjectExists(m))
        {
            PrivateAdd(p, m);
        }
    }

    public void RemoveObject(Movement m)
    {
        foreach (Pair p in ObjectExists(m))
        {
            PrivateRemove(p, m);
        }
    }

    public void Clear()
    {
        buckets.Clear();
    }

    public List<CollisionPair> PotentialCollisions() // return references?
    {
        List<CollisionPair> sets = new();

        if (!buckets.Any())
        {
            return sets;
        }

        foreach (KeyValuePair<Pair, List<Movement>> e in buckets)
        {
            if (e.Value.Count > 1)
            {
                for (int i = 0; i < e.Value.Count - 1; i++)
                {
                    for (int j = i + 1; j < e.Value.Count; j++)
                    {
                        CollisionPair c;
                        c.m1 = e.Value[i];
                        c.m2 = e.Value[j];
                        sets.Add(c);
                    }
                }
            }
        }
        return sets;
    }

    // all cells where the object exists
    private List<Pair> ObjectExists(Movement m)
    {
        List<Pair> list = new();
        Pair topleft, topright, bottomleft, bottomright;
        topleft.x =     (int)Math.Floor((m.GetX() - 0.5) / cellsize); topleft.y =     (int)Math.Floor((m.GetZ() + 0.5) / cellsize); // potential confusion: ycoord of Pair is z of movement
        topright.x =    (int)Math.Floor((m.GetX() + 0.5) / cellsize); topright.y =    (int)Math.Floor((m.GetZ() + 0.5) / cellsize);
        bottomleft.x =  (int)Math.Floor((m.GetX() - 0.5) / cellsize); bottomleft.y =  (int)Math.Floor((m.GetZ() - 0.5) / cellsize);
        bottomright.x = (int)Math.Floor((m.GetX() + 0.5) / cellsize); bottomright.y = (int)Math.Floor((m.GetZ() - 0.5) / cellsize);
        list.Add(topleft);
        list.Add(topright);
        list.Add(bottomleft);
        list.Add(bottomright);
        return list;
    }

    private void PrivateAdd(Pair p, Movement m)
    {
        if (!buckets.ContainsKey(p))
        {
            buckets.Add(p, new List<Movement>());
        }
        buckets[p].Add(m);
    }

    private void PrivateRemove(Pair p, Movement m)
    {
        buckets[p].Remove(m);
        if (!buckets[p].Any())
        {
            buckets.Remove(p);
        }
    }
}
