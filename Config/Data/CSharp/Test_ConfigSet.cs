//该脚本为打表工具自动生成，切勿修改！
using UnityEngine;
using System;
using System.Collections.Generic;
public struct Test_ConfigSet:IDataConfig
{
	Dictionary<string, Dictionary<string,int>> configDic;
	List<string[]> data;
	TestDataConfig cache;
	Dictionary<string, int> dicCache;
	Dictionary<string, List<int>> dicListCache;
	List<int> indexListCache;
	List<TestDataConfig> configLineListCache;
	Dictionary<string, Dictionary<string, List<int>>> configListDic;
	public TestDataConfig GetConfigByKey(string keyName, string value)
	{
		if (configDic.TryGetValue(keyName, out dicCache))
		{
			int index;
			dicCache.TryGetValue(value, out index);
			return DeserializeByIndex(index);
		}
		else if(configListDic.TryGetValue(keyName, out dicListCache))
		{
			if(dicListCache.TryGetValue(value, out indexListCache))
			{
				return DeserializeByIndex(indexListCache[0]);
			}
			else
			{
				return new TestDataConfig();
			}
		}
		else
		{
			return new TestDataConfig();
		}
	}
	public List<TestDataConfig> GetConfigsByKey(string keyName, string value)
	{
		configListDic.TryGetValue(keyName, out dicListCache);
		if (dicListCache.TryGetValue(value, out indexListCache))
		{
			configLineListCache = new List<TestDataConfig>(indexListCache.Count);
			for (int i = 0; i < indexListCache.Count; i++)
			{
				configLineListCache.Add(DeserializeByIndex(i));
			}
		}
		else
		{
			configLineListCache = new List<TestDataConfig>(1);
			configLineListCache.Add(GetConfigByKey(keyName, value));
		}
		return configLineListCache;
	}
	private TestDataConfig DeserializeByIndex(int i)
	{
		cache.id = UInt32.Parse(data[i][0]);
		cache.key = (KeyCode)UInt32.Parse(data[i][1]);
		cache.testString = data[i][2];
		for (int temp2 = 4; temp2 < 4 + 2; temp2++)
		{
			temp1[temp2 - 4] = UInt32.Parse(data[i][temp2]);
		}
		cache.testArray1 = temp1;
		cache.testArray2 = Array.ConvertAll(data[i][6].Split(';'), s => UInt32.Parse(s));
		cache.testStruct1 = new TestDataConfig.S_testStruct1
		{
			aa=UInt32.Parse(data[i][8]),
			bb=data[i][9],
		};
		for (int temp4 = 12; temp4 < 12 + 2 * 2; temp4 += 2)
		{
			temp3[(temp4 - 12) / 2] = new TestDataConfig.S_testArrayStruct1
			{
				cc=UInt32.Parse(data[i][temp4+0]),
				dd=data[i][temp4+1],
			};
		}
		cache.testArrayStruct1 = temp3;
		return cache;
	}
	UInt32[]temp1;
	TestDataConfig.S_testArrayStruct1[]temp3;
	public void Init(List<string[]> data)
	{
		this.data = data;
		cache = new TestDataConfig();
		configDic = new Dictionary<string, Dictionary<string, int>>();
		configListDic = new Dictionary<string, Dictionary<string, List<int>>>();
		Dictionary<string,int> idDic = new Dictionary<string,int>();
		Dictionary<string,List<int>> keyListDic = new Dictionary<string,List<int>>();
		temp1 = new UInt32[2];
		temp3 = new TestDataConfig.S_testArrayStruct1[2];
		for (int i = 0; i < data.Count; i++)
		{
			idDic.Add(data[i][0], i);
			List<int> temp5;
			if (keyListDic.TryGetValue(data[i][1], out temp5))
			{
				temp5.Add(i);
			}
			else
			{
				temp5 = new List<int>(5);
				keyListDic.Add(data[i][1], temp5);
				temp5.Add(i);
			}
		}
		configDic.Add("id",idDic);
		configListDic.Add("key",keyListDic);
	}
	public List<TestDataConfig> GetAllConfig()
	{
		List<Test> temp = new List<TestDataConfig>(data.Count);
		for (int i = 0; i < data.Count; i++)
		{
			temp.Add(DeserializeByIndex(i));
		}
		return temp;
	}
}
