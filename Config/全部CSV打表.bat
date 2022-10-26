set DIR=".\CsvConfig"
for /R %DIR% %%f in (*.csv) do ( 
copy %%f ".\Data\Csv\"
)

copy ".\Data\Csv\*.csv" "..\Assets\Addressable\Config\"

start /wait "" ".\Tool\DJConfigTool\ConfigTool.exe" ""
copy ".\Data\CSharp\*.cs" "..\Assets\Scripts\DataConfig\"
pause