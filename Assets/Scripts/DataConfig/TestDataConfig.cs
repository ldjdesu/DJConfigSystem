//该脚本为打表工具自动生成，切勿修改！
using UnityEngine;
using System;
public struct TestDataConfig:IDataConfigLine
{
	public UInt32 id;
	public KeyCode key;
	public string testString;
	public UInt32[] testArray1;
	public UInt32[] testArray2;
	public struct S_testStruct1
	{
		public UInt32 aa;
		public String bb;
	}
	public S_testStruct1 testStruct1;
	public struct S_testArrayStruct1
	{
		public UInt32 cc;
		public String dd;
	}
	public S_testArrayStruct1[]  testArrayStruct1;
}