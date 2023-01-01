using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Randomness
{
    /// <summary>
    /// Shuffles the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Shuffles the array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    /// <summary>
    /// Returns a random item from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Returns a random item from the list that meets the given criteria. Returns default if no item is found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> list, System.Func<T,bool> criteria)
    {
        List<T> tempList = new();
        tempList.AddRange(list);
        tempList.Shuffle();
        foreach (T item in tempList)
        {
            if (criteria(item)) return item;
        }

        return default;
    }

    /// <summary>
    /// Returns a random item from the array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    /// <summary>
    /// Returns a random key value pair from the dictionary
    /// </summary>
    /// <typeparam name="T">Key</typeparam>
    /// <typeparam name="U">Value</typeparam>
    /// <param name="dict">Dictionary</param>
    /// <returns></returns>
    public static KeyValuePair<T,U> GetRandom<T,U>(this Dictionary<T,U> dict)
    {
        int index = dict.Count;
        foreach (KeyValuePair<T,U> item in dict)
        {
            if (Random.Range(1, index) == index)
            {
                return item;
            }
            index--;
        }

        return new KeyValuePair<T,U>(default,default);
    }

    /// <summary>
    /// Returns a random item from the list given specific weightings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public static T GetWeightedRandom<T>(this List<T> list, List<float> weights)
    {
        float total = 0;
        T value = list[0];

        for (int i = 0; i < list.Count; i++)
        {
            //If we go outside the bounds of the weights array, assume the weightings for all the final elements are 0.
            float weight = (i >= weights.Count) ? 0 : weights[i];
            total += weight;
            if (total > 0 && Random.Range(0, 1) <= weight / total) value = list[i];
        }

        return value;
    }

}