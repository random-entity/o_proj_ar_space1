using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static void SafeModuloAdd(ref int num, int add, int modWhat)
    {
        num += add;

        if (0 <= num && num < modWhat) return;

        num %= modWhat;

        if (num < 0)
        {
            num += modWhat;
        }
    }
    public static T SafeGet<T>(T[] array, int index)
    {
        int size = array.Length;
        if (index >= size)
        {
            index %= size;
        }
        else if (index < 0)
        {
            Debug.LogWarning("SafeGet() : index < 0");
            index %= size;
            index += size;
        }
        return array[index];
    }
    public static void InitializeArray<T>(out T[] array, T prefab, int size, Transform parent) where T : MonoBehaviour
    {
        array = new T[size];
        for (int i = 0; i < size; i++)
        {
            T ith = MonoBehaviour.Instantiate(prefab);
            array[i] = ith;
            ith.transform.SetParent(parent);
        }
    }
    public static void InitializeArray<T>(out T[] array, T prefab, int size, Transform parent, Action<T> initFunction) where T : MonoBehaviour
    {
        InitializeArray<T>(out array, prefab, size, parent);
        Array.ForEach<T>(array, initFunction);

    }
    public static int[] Shuffle(ref int[] permutation)
    {
        System.Random rng = new System.Random();
        for (int i = permutation.Length - 1; i > -1; i--)
        {
            int j = rng.Next(i);
            int tmp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = tmp;
        }
        return permutation;
    }
}