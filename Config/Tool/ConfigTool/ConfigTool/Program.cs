using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace ConfigTool
{
    class Program
    {
        enum indexType
        {
            UnIndex,//不索引
            NoUnique,//多值
            Unique,//唯一值
        }
        static void Main(string[] args)
        {
            Model model=new CSharpModel();
            ConfigSet configSet = new CSharpConfigSet();
            Console.WriteLine("Hello World!");
            string csvPath = args[0]+ @".\Data\Csv";
            string cSharpPath = args[0]+ @".\Data\CSharp";
            string md5Path = args[0]+ @".\Data\MD5";
            try
            {
                var files = Directory.EnumerateFiles(csvPath, "*.csv");//得到所有csv文件
                List<string> csvMD5;
                Dictionary<string, string> configName_MD5 = new Dictionary<string, string>();
                try
                {
                    csvMD5 = new List<string>(File.ReadAllLines(md5Path + "\\CsvMD5.txt"));
                    foreach (var item in csvMD5)
                    {
                        string[] str = item.Split(",");
                        configName_MD5.Add(str[0], str[1]);
                    }
                }
                catch (Exception)
                {
                    csvMD5 = new List<string>();
                    Console.WriteLine("无MD5文件，将重新创建");
                }
                foreach (string filePath in files)
                {



                    Console.WriteLine(filePath);
                    string[] tempFilePath = filePath.Split('\\');
                    string configName = tempFilePath[tempFilePath.Length - 1].Replace(".csv","");
                    string str = File.ReadAllText(filePath);

                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    Byte[] newMD5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                    var sBuilder = new StringBuilder();
                    for (int i = 0; i < newMD5Bytes.Length; i++)
                    {
                        sBuilder.Append(newMD5Bytes[i].ToString("x2"));
                    }
                    string newMD5Str = sBuilder.ToString();
                    //string newMD5Str = Encoding.UTF8.GetString(newMD5Bytes);
                    string oldMD5;
                    if (configName_MD5.TryGetValue(configName,out oldMD5))
                    {
                        if (newMD5Str== oldMD5)//对比md5
                        {
                            continue;
                        }
                        else
                        {
                            configName_MD5[configName] = newMD5Str;
                        }
                    }
                    else//添加md5
                    {
                        configName_MD5.Add(configName, newMD5Str);
                    }
                    string[] text = File.ReadAllLines(filePath);
                    //======================构建model类 Start

                    string outText= GenerateModel(model, configName, text, filePath);
                    string outPutPath = cSharpPath+"\\" + configName + "DataConfig.cs";
                    File.WriteAllText(outPutPath, outText, Encoding.UTF8);

                    //======================构建model类 End
                    //======================构建set类 Start
                    outText = GenerateSet(configSet, configName, text, filePath);
                    outPutPath = cSharpPath + "\\" + configName + "_ConfigSet.cs";
                    File.WriteAllText(outPutPath, outText, Encoding.UTF8);
                    //======================构建set类 End
                }
                string md5Str = "";
                foreach (var item in configName_MD5)
                {
                    md5Str+=item.Key+","+item.Value + Environment.NewLine;
                }
                File.WriteAllText(md5Path + "\\CsvMD5.txt", md5Str, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("有错误，点击任意键退出");
            }

            Console.WriteLine("构建成功");
            Console.ReadKey();
        }

        static private string GenerateModel(Model model, string configName, string[] text, string filePath)
        {
            //构建开头
            string outPut = model.GetStart(configName);


            string[] typeStr = text[1].Split(',');//0行是注释，跳过
            string[] fildStr = text[2].Split(',');
            int flag = 0;//标志位(1为结构体数组模式)
            int offset = 0;//长度位
            //构建字段
            for (int i = 0; i < typeStr.Length; i++)
            {
                if (typeStr[i].Contains("_"))
                {
                    string[] typeDivision = typeStr[i].Split('_');
                    if (typeDivision[0] == "Array")
                    {
                        string suffix = typeDivision[1];
                        if (suffix == "Struct")//结构体数组模式
                        {
                            flag = 1;
                            offset = int.Parse(fildStr[i]);
                            //structName = fildStr[i];
                        }
                        else if (Regex.IsMatch(suffix, @"^\d+$"))//数组模式1
                        {
                            int size = int.Parse(suffix);
                            outPut += model.GetArray(typeStr[i + 1], fildStr[i + 1]);
                            i += size;
                        }
                        else//数组模式2
                        {
                            outPut += model.GetArray(suffix, fildStr[i]);
                        }
                    }
                    else if (typeDivision[0] == "Struct")
                    {
                        int size = int.Parse(typeDivision[1]);
                        outPut += model.GetStruct(fildStr[i]);
                        string structName = fildStr[i];
                        for (int j = 0; j < size; j++, i++)
                        {
                            outPut += model.GetStructType(typeStr[i + 1], fildStr[i + 1]);
                        }
                        bool isArray = false;
                        if (flag == 1)
                        {
                            isArray = true;
                            i += offset * size - size;
                        }
                        outPut += model.GetStructEnd(structName, isArray);
                    }
                    else
                    {
                        Console.WriteLine("未能识别的类型前缀,请检查!  路径为：" + filePath + "  第" + i + "列");
                    }
                }
                else
                {
                    outPut += model.GetType(typeStr[i], fildStr[i]);
                }
            }
            outPut += model.GetEnd();
            return outPut;
        }

        static private string GenerateSet(ConfigSet configSet, string configName, string[] text, string filePath)
        {



            string[] typeStr = text[1].Split(',');//0行是注释，跳过
            string[] fildStr = text[2].Split(',');
            string[] indexStr = text[3].Split(',');
            bool hasNoUnique = false;
            for (int i = 0; i < indexStr.Length; i++)
            {
                if (indexStr[i].Contains("NoUnique"))
                {
                    hasNoUnique = true;
                    break;
                }
            }
            //构建开头
            string outPut = configSet.GetStart(configName, hasNoUnique);
            //构建字段
            string strDic = "";//索引的开始
            string strDicEnd = "";//索引的结尾
            indexType[] indexTypes = new indexType[typeStr.Length];//索引类型标记（0为不索引，1为Unique，2为NoUnique）
            for (int i = 0; i < indexStr.Length; i++)
            {
                if (indexStr[i].Length != 0)
                {
                    string[] indexDivision = indexStr[i].Split('_');
                    if (indexDivision[0] == "Index")
                    {
                        if (indexDivision[1] == "Unique")
                        {
                            strDic += configSet.GetDic(fildStr[i], false);
                            strDicEnd += configSet.GetDicEnd(fildStr[i], false);
                            indexTypes[i] = indexType.Unique;
                        }
                        else if (indexDivision[1] == "NoUnique")
                        {
                            strDic += configSet.GetDic(fildStr[i], true);
                            strDicEnd += configSet.GetDicEnd(fildStr[i], true);
                            indexTypes[i] = indexType.NoUnique;
                        }
                        else
                        {
                            Console.WriteLine("未能识别的索引后缀,请检查!  路径为：" + filePath + "  第" + i + "列");
                        }
                    }
                    else
                    {
                        Console.WriteLine("未能识别的索引前缀,请检查!  路径为：" + filePath + "  第" + i + "列");
                    }
                }
                else
                {
                    indexTypes[i] = indexType.UnIndex;
                    continue;
                }
            }
            outPut += configSet.GetKeyStrFunction(configName, true, hasNoUnique);
            outPut += configSet.GetKeyStrFunction(configName, false, hasNoUnique);

            int flag = 0;//标志位(1为结构体数组模式)
            int offset = 0;//长度位
            string structName = "";//缓存结构体字段名
            int num = 1;//临时变量名序号
            string arrayCache = "";
            string arrayCacheNew = "";
            outPut += configSet.GetDeserialize(configName);
            for (int i = 0; i < typeStr.Length; i++)
            {
                if (typeStr[i].Contains("_"))
                {
                    string[] typeDivision = typeStr[i].Split('_');
                    if (typeDivision[0] == "Array")
                    {
                        string suffix = typeDivision[1];
                        if (suffix == "Struct")//结构体数组模式
                        {
                            flag = 1;
                            offset = int.Parse(fildStr[i]);
                            //structName = fildStr[i];
                        }
                        else if (Regex.IsMatch(suffix, @"^\d+$"))//数组模式1
                        {
                            int size = int.Parse(suffix);
                            arrayCache += configSet.GetArrayCache( typeStr[i + 1], num);
                            arrayCacheNew += configSet.GetArrayCacheNew(typeStr[i + 1], num, size);
                            outPut += configSet.GetArray(configName, typeStr[i + 1], fildStr[i + 1], i, ref num, 1, size);
                            i += size;
                        }
                        else//数组模式2
                        {
                            outPut += configSet.GetArray(configName, suffix, fildStr[i], i, ref num, 2);
                        }
                    }
                    else if (typeDivision[0] == "Struct")
                    {
                        bool isArray = flag == 1;
                        int size = int.Parse(typeDivision[1]);
                        structName = fildStr[i];
                        if (isArray)
                        {
                            arrayCache += configSet.GetArrayCache(configName + "DataConfig.S_" + structName, num);
                            arrayCacheNew += configSet.GetArrayCacheNew(configName + "DataConfig.S_" + structName, num, size);
                            outPut += configSet.GetStructArray(configName, fildStr[i], i, ref num, offset, size);
                            for (int j = 0; j < size; j++, i++)
                            {
                                outPut += configSet.GetStructArrayType(typeStr[i + 1], fildStr[i + 1], num, j);
                            }
                            i += offset * size - size;
                            outPut += configSet.GetStructArrayEnd(configName, structName, num);
                        }
                        else
                        {
                            outPut += configSet.GetStruct(configName, fildStr[i]);
                            for (int j = 0; j < size; j++, i++)
                            {
                                outPut += configSet.GetStructType(typeStr[i + 1], fildStr[i + 1], i + 1);
                            }
                            outPut += configSet.GetStructEnd();
                        }
                    }
                    else
                    {
                        Console.WriteLine("未能识别的类型前缀,请检查!  路径为：" + filePath + "  第" + i + "列");
                    }
                }
                else
                {
                    outPut += configSet.GetType( fildStr[i], typeStr[i], i);
                }
            }
            outPut += configSet.GetDeserializeEnd();
            outPut += arrayCache;

            outPut += configSet.GetInit(configName, hasNoUnique, strDic+ arrayCacheNew);

            for (int i = 0; i < typeStr.Length; i++)
            {
                if (!typeStr[i].Contains("_"))
                {
                    switch (indexTypes[i])
                    {
                        case indexType.UnIndex:
                            break;
                        case indexType.NoUnique:
                            outPut += configSet.GetDicAdd(fildStr[i], true, i, ref num);
                            break;
                        case indexType.Unique:
                            outPut += configSet.GetDicAdd(fildStr[i], false, i, ref num);
                            break;
                        default:
                            break;
                    }
                }
            }
            outPut += "\t\t" + configSet.GetEnd();
            outPut += strDicEnd;
            outPut += "\t" + configSet.GetEnd();
            outPut += configSet.GetEnd();
            return outPut;
        }
    }
}
