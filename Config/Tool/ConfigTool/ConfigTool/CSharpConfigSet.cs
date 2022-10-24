using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigTool
{
    class CSharpConfigSet:ConfigSet
    {
        string newLine = System.Environment.NewLine;
        public string GetArray(string configName,string typeName, string fieldName,int index,ref int num,int flag, int size = 0)//模式1和模式2
        {
            string temp;
            if (flag==1)
            {
                index += 1;
                temp = "\t\tfor (int temp" + (num+1) + " = "+ index+"; temp" + (num+1) + " < " + index + " + " + size+"; temp" + (num+1) + "++)" + newLine +
                        "\t\t{" + newLine +
                        "\t\t\ttemp" + num + "[temp" + (num+1) + " - " + index + "] = "+ typeName+".Parse(data[i][temp" + (num+1) + "]);" + newLine +
                        "\t\t}" + newLine +
                        "\t\tcache." + fieldName+" = temp" + num + ";" + newLine;
                num += 2;
            }
            else
            {
                temp = "\t\tcache." + fieldName + " = Array.ConvertAll(data[i][" + index + "].Split(';'), s => ";
                temp += GetTypeToString(typeName, "s") + ");" + newLine;
            }
            return temp;
        }
        public string GetArrayCache( string typeName, int num)
        {
            return "\t"+typeName + "[]" + "temp" + num + ";" + newLine;
        }
        public string GetArrayCacheNew(string typeName,int num,int size)
        {
            return "\t\ttemp" + num + " = new " + typeName + "[" + size + "];" + newLine;
        }
        public string GetType(string fieldName,string typeName, int index)
        {
            string temp = "\t\tcache." + fieldName + " = ";
            temp += GetTypeToString(typeName, "data[i][" + index + "]") + ";" + newLine;
            return temp;
        }
        public string GetDicAdd(string fildName, bool isList, int index, ref int num)
        {
            string temp = "";
            if (isList)
            {
                temp += "\t\t\tList<int> temp" + num+";" + newLine +
                        "\t\t\tif (" + fildName + "ListDic.TryGetValue(data[i][" + index + "], out temp" + num + "))" + newLine +
                        "\t\t\t{" + newLine +
                        "\t\t\t\ttemp" + num + ".Add(i);" + newLine +
                        "\t\t\t}" + newLine +
                        "\t\t\telse" + newLine +
                        "\t\t\t{" + newLine +
                        "\t\t\t\ttemp" + num + " = new List<int>(5);" + newLine +
                        "\t\t\t\t" + fildName + "ListDic.Add(data[i][" + index + "], temp" + num + ");" + newLine +
                        "\t\t\t\ttemp" + num + ".Add(i);" + newLine +
                        "\t\t\t}" + newLine;
                num++;
            }
            else
            {
                temp += "\t\t\t" + fildName + "Dic.Add(data[i][" + index + "], i);" + newLine;
            }


            return temp;
        }
        public string GetStart(string className, bool hasNoUnique)
        {
            string temp = "//该脚本为打表工具自动生成，切勿修改！" + newLine +
                    "using UnityEngine;" + newLine +
                    "using System;" + newLine +
                    "using System.Collections.Generic;" + newLine +
                    "public struct " + className + "_ConfigSet:IDataConfig" + newLine +
                    "{" + newLine +
                    "\tDictionary<string, Dictionary<string,int>> configDic;" + newLine +
                    "\tList<string[]> data;" + newLine +
                    "\t" + className + "DataConfig cache;" + newLine +
                    "\tDictionary<string, int> dicCache;" + newLine +
                    "\tDictionary<string, List<int>> dicListCache;" + newLine +
                    "\tList<int> indexListCache;" + newLine +
                    "\tList<"+ className+"DataConfig> configLineListCache;" + newLine;
            if (hasNoUnique)
            {
                temp += "\tDictionary<string, Dictionary<string, List<int>>> configListDic;" + newLine;
            }
            return temp;
        }
        public string GetStruct(string configName, string fieldName)
        {
            return "\t\tcache." + fieldName + " = new " + configName + "DataConfig.S_" + fieldName + "" + newLine +
                   "\t\t{" + newLine;
        }
        public string GetStructType(string typeName, string fieldName,int index)
        {
            return "\t\t\t" + fieldName + "=" + GetTypeToString(typeName, "data[i][" + index + "]") + "," + newLine;
        }
        public string GetStructEnd()
        {
            return "\t\t};" + newLine;
        }
        public string GetStructArray(string configName, string fieldName,int index,ref int num, int arraySize, int structSize)
        {
            index += 1;
            string temp = "\t\tfor (int temp"+ (num+1)+" = "+ index+"; temp" + (num+1)+" < "+ index+" + "+ arraySize+" * "+ structSize+"; temp" + (num+1)+" += "+ structSize+")" + newLine +
                           "\t\t{" + newLine +
                           "\t\t\ttemp"+ num+"[(temp"+ (num+1)+" - "+ index+") / "+ structSize+"] = new " + configName+"DataConfig.S_"+ fieldName + newLine +
                           "\t\t\t{" + newLine;
            num += 2;
            return temp;
        }
        public string GetStructArrayType(string typeName, string fieldName, int num,int offset)
        {
            return "\t\t\t\t" + fieldName + "=" + GetTypeToString(typeName, "data[i][temp" + (num-1) + "+"+ offset+"]") + "," + newLine;
        }
        public string GetStructArrayEnd(string configName,string structName,int num)
        {
            return "\t\t\t};" + newLine +
                   "\t\t}" + newLine +
                   "\t\tcache." + structName+" = temp"+ (num-2)+";" + newLine;
        }
        public string GetKeyStrFunction(string configName,bool isUnique, bool hasNoUnique)
        {
            if (isUnique)
            {
                string temp1 = "";
                string temp2 = "";
                if (hasNoUnique)
                {
                    temp2 = "\t\telse if(configListDic.TryGetValue(keyName, out dicListCache))" + newLine +
                        "\t\t{" + newLine +
                        "\t\t\tif(dicListCache.TryGetValue(value, out indexListCache))" + newLine +
                        "\t\t\t{" + newLine +
                        "\t\t\t\treturn DeserializeByIndex(indexListCache[0]);" + newLine +
                        "\t\t\t}" + newLine+
                        "\t\t\telse" + newLine +
                        "\t\t\t{" + newLine +
                        "\t\t\t\treturn new "+configName+"DataConfig();" + newLine +
                        "\t\t\t}" + newLine +
                        "\t\t}" + newLine;
                }
                return "\tpublic " + configName + "DataConfig GetConfigByKey(string keyName, string value)" + newLine +
                        "\t{" + newLine +
                        "\t\tif (configDic.TryGetValue(keyName, out dicCache))" + newLine +
                        "\t\t{" + newLine +
                        "\t\t\tint index;" + newLine +
                        "\t\t\tdicCache.TryGetValue(value, out index);" + newLine +
                        "\t\t\treturn DeserializeByIndex(index);" + newLine +
                        "\t\t}" + newLine +
                        temp2 +
                        "\t\telse" + newLine +
                        "\t\t{" + newLine +
                        "\t\t\treturn new " + configName + "DataConfig();" + newLine +
                        "\t\t}" + newLine +
                        "\t}" + newLine;
            }
            else
            {
                if (hasNoUnique)
                {
                    return "\tpublic List<"+ configName+"DataConfig> GetConfigsByKey(string keyName, string value)" + newLine +
                            "\t{" + newLine +
                            "\t\tconfigListDic.TryGetValue(keyName, out dicListCache);" + newLine +
                            "\t\tif (dicListCache.TryGetValue(value, out indexListCache))" + newLine +
                            "\t\t{" + newLine +
                            "\t\t\tconfigLineListCache = new List<" + configName + "DataConfig>(indexListCache.Count);" + newLine +
                            "\t\t\tfor (int i = 0; i < indexListCache.Count; i++)" + newLine +
                            "\t\t\t{" + newLine +
                            "\t\t\t\tconfigLineListCache.Add(DeserializeByIndex(i));" + newLine +
                            "\t\t\t}" + newLine +
                            "\t\t}" + newLine +
                            "\t\telse" + newLine +
                            "\t\t{" + newLine +
                            "\t\t\tconfigLineListCache = new List<" + configName + "DataConfig>(1);" + newLine +
                            "\t\t\tconfigLineListCache.Add(GetConfigByKey(keyName, value));" + newLine +
                            "\t\t}" + newLine +
                            "\t\treturn configLineListCache;" + newLine +
                            "\t}" + newLine;
                }
                else
                {
                    return "\tpublic List<IDataConfigLine> GetConfigsByKey(string keyName, string value)" + newLine +
                        "\t{" + newLine +
                        "\t\tList<IDataConfigLine> configLineList; " + newLine +
                        "\t\tconfigLineList = new List<IDataConfigLine>(1);" + newLine +
                        "\t\tconfigLineList.Add(GetConfigByKey(keyName, value));" + newLine +
                        "\t\treturn configLineList;" + newLine +
                        "\t}" + newLine;
                }
            }
        }
        public string GetDic(string fildName, bool isList)
        {
            if (isList)
            {
                return "\t\tDictionary<string,List<int>> " + fildName + "ListDic = new Dictionary<string,List<int>>();" + newLine;
            }
            else
            {
                return "\t\tDictionary<string,int> " + fildName + "Dic = new Dictionary<string,int>();" + newLine;
            }
        }
        public string GetDicEnd(string fildName, bool isList)
        {
            string temp = isList ? "List" : "";
            return "\t\tconfig"+temp+"Dic.Add(\""+fildName+"\","+ fildName+ temp+"Dic);" +newLine;
        }
        public string GetInit(string configName,bool hasNoUnique,string strDic)
        {
            string temp = "\tpublic void Init(List<string[]> data)" + newLine +
                          "\t{" + newLine +
                          "\t\tthis.data = data;" + newLine +
                           "\t\tcache = new " + configName + "DataConfig();" + newLine +
                          "\t\tconfigDic = new Dictionary<string, Dictionary<string, int>>();" + newLine;
            if (hasNoUnique)
            {
                temp += "\t\tconfigListDic = new Dictionary<string, Dictionary<string, List<int>>>();" + newLine;
            }
            temp += strDic;
            temp += "\t\tfor (int i = 0; i < data.Count; i++)" + newLine +
                    "\t\t{" + newLine;
            return temp;
        }
        public string GetDeserialize(string configName)
        {
            return "\tprivate "+ configName+"DataConfig DeserializeByIndex(int i)" + newLine +
                   "\t{" + newLine;
        }
        public string GetDeserializeEnd()
        {
            return "\t\treturn cache;" + newLine +
                   "\t}" + newLine;
        }
        public string GetEnd()
        {
            return "}"+newLine;
        }

        private string GetTypeToString(string typeName,string value)
        {
            string temp;
            if (typeName == "string"|| typeName == "String")
            {
                temp = value ;
            }
            else if (typeName == "UInt32" || typeName == "Int32" || typeName == "UInt64" || typeName == "Int64"
                || typeName == "Boolean" || typeName == "float" || typeName == "double" )
            {
                temp = typeName + ".Parse(" + value + ")";
            }
            else
            {
                
                temp = "(" + typeName + ")UInt32.Parse(" + value + ")";
            }
            return temp;
        }
        public string GetStrAllConfig(string configName)
        {
            string temp="";
            temp += "\tpublic List<" + configName + "DataConfig> GetAllConfig()" + newLine +
                "\t{" + newLine +
                "\t\tList<" + configName + "DataConfig> temp = new List<" + configName + "DataConfig>(data.Count);" + newLine +
                "\t\tfor (int i = 0; i < data.Count; i++)" + newLine +
                "\t\t{" + newLine +
                "\t\t\ttemp.Add(DeserializeByIndex(i));" + newLine +
                "\t\t}" + newLine +
                "\t\treturn temp;" + newLine +
                "\t}" + newLine;
            return temp;
        }
    }
}
