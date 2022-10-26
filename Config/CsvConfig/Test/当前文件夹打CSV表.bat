copy ".\*.csv" "..\..\..\Assets\Addressable\Config\"
copy ".\*.csv" "..\..\Data\Csv\"

start /wait "" "..\..\Tool\DJConfigTool\ConfigTool.exe" ""..\.""
copy "..\..\Data\CSharp\*.cs" "..\..\..\Assets\Scripts\DataConfig\"
pause