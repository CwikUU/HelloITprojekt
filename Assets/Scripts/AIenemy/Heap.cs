using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount; // Ustaw indeks kopca dla elementu
        items[currentItemCount] = item; // Dodaj element do kopca
        SortUp(item); // Uporz¹dkuj kopiec w górê
        currentItemCount++; // Zwiêksz liczbê elementów w kopcu
    }

    public T RemoveFirst()
    {
        T firstItem = items[0]; // Pobierz pierwszy element z kopca
        currentItemCount--; // Zmniejsz liczbê elementów w kopcu
        items[0] = items[currentItemCount]; // Zamieñ pierwszy element z ostatnim elementem
        items[0].HeapIndex = 0; // Ustaw indeks kopca dla nowego pierwszego elementu
        SortDown(items[0]); // Uporz¹dkuj kopiec w dó³
        return firstItem; // Zwróæ usuniêty pierwszy element
    }

    public void UpdateItem(T item)
    {
        SortUp(item); // Uporz¹dkuj kopiec w górê
    }

    public int Count
    {
        get { return currentItemCount; } // Zwróæ liczbê elementów w kopcu
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item); // SprawdŸ, czy kopiec zawiera dany element
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1; // Oblicz indeks lewego dziecka
            int childIndexRight = item.HeapIndex * 2 + 2; // Oblicz indeks prawego dziecka
            int swapIndex = 0; // Indeks do zamiany

            if (childIndexLeft < currentItemCount) // SprawdŸ, czy istnieje lewe dziecko
            {
                swapIndex = childIndexLeft; // Ustaw indeks do zamiany na lewe dziecko
                if (childIndexRight < currentItemCount) // SprawdŸ, czy istnieje prawe dziecko
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) // Porównaj lewe i prawe dziecko
                    {
                        swapIndex = childIndexRight; // Ustaw indeks do zamiany na prawe dziecko, jeœli jest wiêksze
                    }
                }
                if (item.CompareTo(items[swapIndex]) < 0) // Porównaj element z dzieæmi
                {
                    Swap(item, items[swapIndex]); // Jeœli element jest mniejszy ni¿ dziecko, zamieñ je miejscami
                }
                else
                {
                    return; // Jeœli element jest wiêkszy lub równy dziecku, zakoñcz sortowanie
                }
            }
            else
            {
                return; // Jeœli nie ma dzieci, zakoñcz sortowanie
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2; // Oblicz indeks rodzica
        while (true)
        {
            T parentItem = items[parentIndex]; // Pobierz element rodzica
            if (item.CompareTo(parentItem) > 0) // Porównaj element z rodzicem
            {
                Swap(item, parentItem); // Jeœli element jest wiêkszy ni¿ rodzic, zamieñ je miejscami
            }
            else
            {
                break; // Jeœli element jest mniejszy lub równy rodzicowi, zakoñcz sortowanie
            }
            parentIndex = (item.HeapIndex - 1) / 2; // Oblicz nowy indeks rodzica
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB; // Zamieñ miejscami elementy w kopcu
        items[itemB.HeapIndex] = itemA; // Ustaw elementy na odpowiednich miejscach
        int itemAIndex = itemA.HeapIndex; // Tymczasowo przechowaj indeks
        itemA.HeapIndex = itemB.HeapIndex; // Zaktualizuj indeks kopca dla elementu A
        itemB.HeapIndex = itemAIndex; // Zaktualizuj indeks kopca dla elementu B
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    } // Indeks w kopcu, który jest u¿ywany do szybkiego dostêpu do elementu w kopcu
}
