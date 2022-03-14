# ConfigDataCenter
## 描述：
- 配置中心，存放所有配置的映射关系


```
/// <summary>
/// 如何使用
/// </summary>
public class DemoStart : MonoBehaviour
{
    void Start()
    {
        TestDataConfig config;
        ConfigDataCenter.Instance.LoadAllConfigNew(() => {//只需初始化一次
        config= ConfigDataCenter.Instance.GetConfigSet<Test_ConfigSet>().GetConfigByKey("id", "1");//根据字段的值获取单行配置数据      
        config= ConfigDataCenter.Instance.GetConfigsSet<Test_ConfigSet>().GetConfigByKey("key", "2");//根据字段的值获取多行配置数据 
        });
        
    }

}
```

---
### 公开属性
属性名|功能
--- | ---
**Instance** | 单例

---

### 公开函数
函数名|功能
--- | ---
**LoadAllConfigNew** |读取所有配置并初始化系统
**GetConfigSet<T>** | 获取IDataConfig
---

