using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heap implementation taken off https://weblogs.asp.net/cumpsd/371719 with some modification
/// </summary>
/// <typeparam name="T"></typeparam>
public class Heap<T>
{
    private HeapPair<T>[] binaryHeap;
    private int numberOfItems;

    public Heap()
    {
        this.binaryHeap = new HeapPair<T>[15];
        numberOfItems = 0;
    }

    public bool HasValues()
    {
        return numberOfItems > 0;
    }

    private void AddToHeap(T item, float val, int ind)
    {
        if (binaryHeap.Length <= ind)
        {
            HeapPair<T>[] temp = new HeapPair<T>[binaryHeap.Length * 2];
            binaryHeap.CopyTo(temp, 0);
            binaryHeap = temp;
        }
        HeapPair<T> pair = new HeapPair<T>();
        pair.Set(val, item);
        binaryHeap[ind] = pair;
    }

    public void Print()
    {
        Debug.Log(string.Join(", ", new List<HeapPair<T>>(binaryHeap)));
    }

    public void Add(T item, float fCost)
    {
        AddToHeap(item, fCost, this.numberOfItems + 1);
        int bubbleIndex = this.numberOfItems + 1;
        while (bubbleIndex != 1)
        {
            int parentIndex = bubbleIndex / 2;
            if (this.binaryHeap[bubbleIndex].GetValue() <= this.binaryHeap[parentIndex].GetValue())
            {
                HeapPair<T> tmpValue = this.binaryHeap[parentIndex];
                this.binaryHeap[parentIndex] = this.binaryHeap[bubbleIndex];
                this.binaryHeap[bubbleIndex] = tmpValue;
                bubbleIndex = parentIndex;
            }
            else
            {
                break;
            }
        }
        this.numberOfItems++;
        //Print();
    } /* Add */

    public HeapPair<T> Remove()
    {
        HeapPair<T> returnItem = this.binaryHeap[1];
        this.binaryHeap[1] = this.binaryHeap[this.numberOfItems];

        int swapItem = 1, parent = 1;
        do
        {
            parent = swapItem;
            if ((2 * parent + 1) <= this.numberOfItems)
            {
                // Both children exist
                if (this.binaryHeap[parent].GetValue() >= this.binaryHeap[2 * parent].GetValue())
                {
                    swapItem = 2 * parent;
                }
                if (this.binaryHeap[swapItem].GetValue() >= this.binaryHeap[2 * parent + 1].GetValue())
                {
                    swapItem = 2 * parent + 1;
                }
            }
            else if ((2 * parent) <= this.numberOfItems)
            {
                // Only one child exists
                if (this.binaryHeap[parent].GetValue() >= this.binaryHeap[2 * parent].GetValue())
                {
                    swapItem = 2 * parent;
                }
            }
            // One if the parent's children are smaller or equal, swap them
            if (parent != swapItem)
            {
                HeapPair<T> tmpValue = this.binaryHeap[parent];
                this.binaryHeap[parent] = this.binaryHeap[swapItem];
                this.binaryHeap[swapItem] = tmpValue;
            }
        } while (parent != swapItem);
        this.numberOfItems--;
        return returnItem;
    } /* Remove */

    public class HeapPair<T>
    {
        private float f;
        private T item;

        public void Set(float f, T item)
        {
            this.f = f;
            this.item = item;
        }

        public float GetValue()
        {
            return f;
        }

        public T GetItem()
        {
            return item;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", item.ToString(), f);
        }
    }
}
