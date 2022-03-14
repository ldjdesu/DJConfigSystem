using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
/// <summary>
/// 配置文件中心
/// </summary>
class ConfigDataCenter
{
    private static ConfigDataCenter instance;
    public static ConfigDataCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConfigDataCenter();
            }
            return instance;
        }
    }
    private Dictionary<Type, IDataConfig> configDicNew;
    public Dictionary<Type, IDataConfig> ConfigDicNew { get => configDicNew; }


    public void LoadAllConfigNew(Action callback)
    {
        configDicNew = new Dictionary<Type, IDataConfig>();
        AssetLabelReference assetLabel = new AssetLabelReference();
        assetLabel.labelString = "Config";
        Addressables.LoadResourceLocationsAsync(assetLabel).Completed += (AsyncOperationHandle<IList<IResourceLocation>> handle) =>
        {
            IList<IResourceLocation> resourceLocation = handle.Result;
            int num = 0;
            for (int i = 0; i < resourceLocation.Count; i++)
            {
                string[] temp = resourceLocation[i].InternalId.Split('/');

                string configName = temp[temp.Length - 1].Split('.')[0];
                Addressables.LoadAssetAsync<TextAsset>(resourceLocation[i].InternalId).Completed += (AsyncOperationHandle<TextAsset> obj) =>//加载配置文件
                {
                    Type t = Type.GetType(configName+"_ConfigSet");
                    if (t == null)
                    {
                        Debug.LogError("类型名为:" + configName + " 配置文件不存在或文件名和类型名不匹配！");
                    }
                    MethodInfo mi = this.GetType().GetMethod("AddConfigToDicNew").MakeGenericMethod(t);
                    mi.Invoke(this, new object[] { obj.Result.text });
                    num++;
                    if (num == resourceLocation.Count)
                    {
                        callback();
                    }
                };
            }
        };
    }
    /// <summary>
    /// 内部做反射用，外部不要去调用
    /// </summary>
    /// <typeparam name="配置类型"></typeparam>
    /// <param name="配置文件数据"></param>
    public void AddConfigToDicNew<T>(string data) where T:IDataConfig,new()
    {
        T value = CSVFileConfig.CSVConfigResolveNew<T>(data);
        ConfigDicNew.Add(typeof(T), value);
    }
    public T GetConfigSet<T>() 
    {
        IDataConfig dataConfig;
        ConfigDicNew.TryGetValue(typeof(T), out dataConfig);
        return (T)dataConfig;
    }
    /*public List<IDataConfigLine> GetConfigsByKey<T>(string keyName, string value) where T : IDataConfig
    {
        IDataConfig dataConfig;
        ConfigDicNew.TryGetValue(typeof(T), out dataConfig);
        return dataConfig.GetConfigsByKey(keyName, value);
    }*/
}

