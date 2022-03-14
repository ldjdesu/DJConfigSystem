# xxx_ConfigSet:IDataConfig
## 描述：
- 由打表工具自动生成，操作配置表和存放原始数据的地方


```
/// <summary>
/// 读取数据
/// </summary>
public class DemoStart : MonoBehaviour
{
    void Start()
    {
        TestDataConfig config;
        config= ConfigDataCenter.Instance.GetConfigSet<Test_ConfigSet>().GetConfigByKey("id", "1");//根据字段的值获取单行配置数据      
        config= ConfigDataCenter.Instance.GetConfigsSet<Test_ConfigSet>().GetConfigByKey("key", "2");//根据字段的值获取多行配置数据 
    }
}
```

### 公开函数
函数名|功能
--- | ---
**GetConfigByKey(string keyName, string value)** |根据字段值获取单行数据
**GetConfigsByKey(string keyName, string value)** | 根据字段值获取多行数据


