using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement, TPriority>
{
    public int Count => values.Count;
    public List<(TElement Element, TPriority Priority)> values = new();

    private readonly IComparer<TPriority> _comparer = Comparer<TPriority>.Default;
    public void Enqueue(TElement element, TPriority priority)
    {
        values.Add((element, priority));

        HeapifyUp(values.Count - 1);
    }

    public TElement Dequeue()
    {
        // root 반환
        TElement result = values[0].Element;


        values[0] = values[values.Count - 1];
        values.RemoveAt(values.Count - 1);

        if (values.Count > 0)
        {
            HeapifyDown(0);
        }

        return result;
    }

    private void HeapifyUp(int index)
    {
        // 가장 끝에서 부터 올라가기
        while (index > 0)
        {
            // 부모위치 정해져있음
            int parent = (index - 1) / 2;
            if (_comparer.Compare(values[index].Priority, values[parent].Priority) < 0)
            {
                Swap(index, parent);
                index = parent;
            }
            else break;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {

            int parent = index;
            int left = 2 * index + 1;
            int right = 2 * index + 2;

            if (left < values.Count && _comparer.Compare(values[left].Priority, values[parent].Priority) < 0)
                parent = left;

            if (right < values.Count && _comparer.Compare(values[right].Priority, values[parent].Priority) < 0)
                parent = right;

            if (parent != index)
            {
                Swap(index, parent);
                index = parent;
            }
            else break;
        }
    }

    private void Swap(int i, int j)
    {
        // Value 교환
        var temp = values[i];
        values[i] = values[j];
        values[j] = temp;
    }

    public TElement Peek()
    {
        if (values.Count == 0)
        {
            throw new InvalidOperationException("비었음");
        }
        return values[0].Element;

    }
    public void Clear()
    {
        values.Clear();
    }


}