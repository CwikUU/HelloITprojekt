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
        SortUp(item); // Uporz�dkuj kopiec w g�r�
        currentItemCount++; // Zwi�ksz liczb� element�w w kopcu
    }

    public T RemoveFirst()
    {
        T firstItem = items[0]; // Pobierz pierwszy element z kopca
        currentItemCount--; // Zmniejsz liczb� element�w w kopcu
        items[0] = items[currentItemCount]; // Zamie� pierwszy element z ostatnim elementem
        items[0].HeapIndex = 0; // Ustaw indeks kopca dla nowego pierwszego elementu
        SortDown(items[0]); // Uporz�dkuj kopiec w d�
        return firstItem; // Zwr�� usuni�ty pierwszy element
    }

    public void UpdateItem(T item)
    {
        SortUp(item); // Uporz�dkuj kopiec w g�r�
    }

    public int Count
    {
        get { return currentItemCount; } // Zwr�� liczb� element�w w kopcu
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item); // Sprawd�, czy kopiec zawiera dany element
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1; // Oblicz indeks lewego dziecka
            int childIndexRight = item.HeapIndex * 2 + 2; // Oblicz indeks prawego dziecka
            int swapIndex = 0; // Indeks do zamiany

            if (childIndexLeft < currentItemCount) // Sprawd�, czy istnieje lewe dziecko
            {
                swapIndex = childIndexLeft; // Ustaw indeks do zamiany na lewe dziecko
                if (childIndexRight < currentItemCount) // Sprawd�, czy istnieje prawe dziecko
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) // Por�wnaj lewe i prawe dziecko
                    {
                        swapIndex = childIndexRight; // Ustaw indeks do zamiany na prawe dziecko, je�li jest wi�ksze
                    }
                }
                if (item.CompareTo(items[swapIndex]) < 0) // Por�wnaj element z dzie�mi
                {
                    Swap(item, items[swapIndex]); // Je�li element jest mniejszy ni� dziecko, zamie� je miejscami
                }
                else
                {
                    return; // Je�li element jest wi�kszy lub r�wny dziecku, zako�cz sortowanie
                }
            }
            else
            {
                return; // Je�li nie ma dzieci, zako�cz sortowanie
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2; // Oblicz indeks rodzica
        while (true)
        {
            T parentItem = items[parentIndex]; // Pobierz element rodzica
            if (item.CompareTo(parentItem) > 0) // Por�wnaj element z rodzicem
            {
                Swap(item, parentItem); // Je�li element jest wi�kszy ni� rodzic, zamie� je miejscami
            }
            else
            {
                break; // Je�li element jest mniejszy lub r�wny rodzicowi, zako�cz sortowanie
            }
            parentIndex = (item.HeapIndex - 1) / 2; // Oblicz nowy indeks rodzica
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB; // Zamie� miejscami elementy w kopcu
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
    } // Indeks w kopcu, kt�ry jest u�ywany do szybkiego dost�pu do elementu w kopcu
}
