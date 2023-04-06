using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class MapHeap
{
    List<Node> list;
    //int[] fscore;

    public MapHeap()
    {
        //this.fscore = fscore;
        list = new List<Node>();
    }

    void swap(int i, int j)
    {
        Node temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    int HasChild(int i)
    {
        int result = 0;
        if (list.Count > i * 2 + 1)
            result++;
        if (list.Count > i * 2 + 2)
            result++;
        return result;
    }

    public int Count()
    {
        return list.Count;
    }

    public bool isEmpty()
    {
        return list.Count == 0;
    }

    public void Push(Node node)
    {
        list.Add(node);
        int current = list.Count - 1;
        int next;
        while(current > 0)
        {
            if (current % 2 == 0)
                next = (current - 1) / 2;
            else
                next = current / 2;
            if (list[current].f < list[next].f)
            {
                swap(current, next);
                current = next;
            }
            else
                break;
        }
    }

    public Node Pop()
    {
        Node result = list[0];
        swap(0, list.Count - 1);
        list.RemoveAt(list.Count - 1);
        int current = 0;
        int left, right;
        int h = HasChild(current);
        bool side;
        while (h != 0)
        {
            left = current * 2 + 1;
            right = current * 2 + 2;
            side = h == 1 || list[left].f < list[right].f;
            if(side)
            {
                if (list[current].f > list[left].f)
                {
                    swap(current, left);
                    current = left;
                }
                else
                    break;
            }
            else
            {
                if (list[current].f > list[right].f)
                {
                    swap(current, right);
                    current = right;
                }
                else
                    break;
            }
            h = HasChild(current);
        }
        return result;
    }

    public void PrintList()
    {
        string o = "";
        for (int i = 0; i < list.Count; i++)
            o += list[i].f + " ";
        Debug.Log(o);
    }
}
