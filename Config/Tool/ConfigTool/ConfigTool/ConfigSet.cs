using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigTool
{
    interface ConfigSet
    {
        public string GetArray(string configName, string typeName, string fieldName, int index, ref int num, int flag, int size=0);
        public string GetArrayCache( string typeName, int num);
        public string GetArrayCacheNew( string typeName, int num, int size);
        public string GetType(string fieldName, string typeName, int index);
        public string GetDicAdd(string fildName, bool isList, int index, ref int num);
        public string GetStruct(string configName, string fieldName);

        public string GetStructType(string typeName, string fieldName, int index);

        public string GetStructEnd();
        public string GetStructArray(string configName, string fieldName, int index, ref int num, int arraySize, int structSize);
        public string GetStructArrayType(string typeName, string fieldName, int num,int offset);
        public string GetStructArrayEnd(string configName, string structName, int num);
        public string GetKeyStrFunction(string configName, bool isUnique, bool hasNoUnique);
        public string GetDic(string fildName, bool isList);
        public string GetInit(string configName, bool hasNoUnique,string strDic);
        public string GetStart(string className,bool hasNoUnique);
        public string GetDicEnd(string fildName, bool isList);

        public string GetDeserialize(string configName);
        public string GetDeserializeEnd();
        public string GetEnd();
    }
}
