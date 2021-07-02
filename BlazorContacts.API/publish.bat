:::: publish to linux
:: release
:: dotnet publish -c release -r linux-x64 
:: debug
dotnet publish -r linux-x64 --self-contained true

:: release
:: pushd .\bin\release\netcoreapp3.1\linux-x64\publish\

:: debug
pushd .\bin\Debug\netcoreapp3.1\linux-x64\publish\

pscp -pw 490910 -r .\* centos@192.168.1.71:/home/centos/sandbox/BlazorContact/Api
popd
