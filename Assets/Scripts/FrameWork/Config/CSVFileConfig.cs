using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// csv文件输入类
/// </summary>
static class CSVFileConfig
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text"></param>
    /// <returns></returns>
    public static T[] CSVConfigResolve<T>(string text)
    {
        if (text == "")
        {
            Debug.LogError("传入文本为空，请检查！");
        }
        string[] str = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        Dictionary<int, Dictionary<string, string>> result = new Dictionary<int, Dictionary<string, string>>();
        string[] typeStr = str[0].Split(',');

        for (int i = 1; i < str.Length; i++)
        {
            string[] temp = str[i].Split(',');
            if (temp[0] == "")
            {
                break;
            }
            result.Add(i - 1, new Dictionary<string, string>());
            for (int j = 0; j < typeStr.Length; j++)
            {
                result[i - 1].Add(typeStr[j], temp[j]);
            }
        }

        FieldInfo[] fields = typeof(T).GetFields();
        List<T> tList = new List<T>();
        for (int i = 0; i < result.Count; i++)
        {
            tList.Add(Activator.CreateInstance<T>());
            for (int j = 0; j < fields.Length; j++)
            {
                if (result[i][fields[j].FieldType.Name]=="")
                {
                    continue;
                }
                if (IsNullableType(typeof(T)))
                {
                    fields[j].SetValue(tList[i], m_ChangeType(result[i][fields[j].FieldType.Name], fields[j].FieldType));
                }
                else//如果不为可空类型(值类型)则需要手动装箱拆箱
                {
                    object tempObj = tList[i];
                    fields[j].SetValue(tempObj, m_ChangeType(result[i][fields[j].FieldType.Name], fields[j].FieldType));
                    tList[i] = (T)tempObj;
                }
            }
        }
        return tList.ToArray();
    }
    /// <summary>
    /// 可以支持字符串转枚举的ChangeType
    /// </summary>
    /// <param name="value"></param>
    /// <param name="conversionType"></param>
    /// <returns></returns>
    private static object m_ChangeType(object value,Type conversionType)
    {

        Type nullableType = Nullable.GetUnderlyingType(conversionType);
        if (nullableType != null)
        {
            if (value == null)
            {
                return null;
            }
            return Convert.ChangeType(value, nullableType);
        }
        if (typeof(System.Enum).IsAssignableFrom(conversionType))
        {
            return Enum.Parse(conversionType, value.ToString());
        }
        return Convert.ChangeType(value, conversionType);
    }

    /// <summary>
    /// 判断一个类型是否为可空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsNullableType(Type type)
    {
        return (type.IsGenericType && type.
          GetGenericTypeDefinition().Equals
          (typeof(Nullable<>)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text"></param>
    /// <returns></returns>
    public static T CSVConfigResolveNew<T>(string text) where T: IDataConfig, new()
    {
        if (text == "")
        {
            Debug.LogError("传入文本为空，请检查！");
        }
        string[] str = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        //第一行是注释不管
        T dataConfig = new T();
        List<string[]> data = new List<string[]>();
        //处理类型
        for (int i = 4; i < str.Length; i++)
        {
            if (str[i].Length!=0)
            {
                data.Add(str[i].Split(','));
            }
        }
        dataConfig.Init(data);
        return dataConfig;
    }
}
