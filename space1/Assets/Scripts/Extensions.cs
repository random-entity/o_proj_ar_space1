using System;
using UnityEngine;

public static class Extensions
{
    public static void ModuloAdd(ref int num, int add, int modWhat)
    {
        num += add;

        if (0 <= num && num < modWhat) return;

        num %= modWhat;

        if (num < 0)
        {
            num += modWhat;
        }
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
}