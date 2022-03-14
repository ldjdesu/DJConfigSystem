namespace ConfigTool
{
    class CSharpModel:Model
    {
        string newLine = System.Environment.NewLine;
        public string GetArray(string type, string fieldName)
        {
            return "\tpublic " + type + "[] " + fieldName + ";" + newLine;
        }
        public string GetType(string type, string fieldName)
        {
            return "\tpublic " + type + " " + fieldName + ";" + newLine;
        }
        public string GetStart(string className)
        {
            return "//该脚本为打表工具自动生成，切勿修改！"+ newLine+
                    "using UnityEngine;" + newLine +
                    "using System;" + newLine +
                    "public struct " + className + "DataConfig:IDataConfigLine" + newLine +
                    "{" + newLine;
        }
        public string GetStruct(string fieldName)
        {
            return "\tpublic struct S_" + fieldName + newLine + "\t{" + newLine;
        }
        public string GetStructType(string type, string fieldName)
        {
            return "\t\tpublic " + type + " " + fieldName + ";" + newLine;
        }
        public string GetStructEnd(string fieldName,bool isArray)
        {
            string temp = isArray ? "[] " : "";
            return "\t}" + newLine + "\tpublic S_" + fieldName + temp + " " + fieldName + ";" + newLine;
        }
        public string GetEnd()
        {
            return "}";
        }


    }
}
