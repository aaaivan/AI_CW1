using System;
using System.Collections;
using System.Collections.Generic;

public class Heap<T> where T : IHeapItem<T>
{
	T[] heap;
	int count;

	public int Count
	{
		get { return count; }
	}

	public Heap(int maxHeapSize)
	{
		heap = new T[maxHeapSize];
	}

	public void Add(T item)
	{
		item.HeapIndex = count;
		heap[count] = item;
		++count;
		SortUp(item);
	}

	public T Pop()
	{
		T firstItem = heap[0];
		--count;
		heap[0] = heap[count];
		heap[0].HeapIndex = 0;
		SortDown(heap[0]);
		return firstItem;
	}

	public bool Contains(T item)
	{
		return Equals(heap[item.HeapIndex], item);
	}

	public void UpdateItem(T item)
	{
		SortUp(item);
	}

	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;
		while(true)
		{
			T parentItem = heap[parentIndex];
			if(item.CompareTo(parentItem) > 0)
			{
				Swap(item, parentItem);
			}
			else
			{
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void SortDown(T item)
	{
		while(true)
		{
			int indexLeft = item.HeapIndex * 2 + 1;
			int indexRight = item.HeapIndex * 2 + 2;
			if (indexLeft < count)
			{
				int swapIndex = indexLeft;
				if (indexRight < count &&
					heap[indexRight].CompareTo(heap[indexLeft]) > 0)
				{
					swapIndex = indexRight;
				}

				if (item.CompareTo(heap[swapIndex]) < 0)
				{
					Swap(item, heap[swapIndex]);
				}
				else
				{
					return;
				}
			}
			else
			{
				return;
			}
		}
	}

	void Swap(T a, T b)
	{
		heap[a.HeapIndex] = b;
		heap[b.HeapIndex] = a;
		int temp = a.HeapIndex;
		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = temp;
	}
}

public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex { get; set; }
}