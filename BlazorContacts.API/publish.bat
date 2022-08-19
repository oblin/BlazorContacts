:::: publish to linux
:: release
:: dotnet publish -c release -r linux-x64 
:: debug
dotnet publish -r linux-x64 --self-contained true

:: release
:: pushd .\bin\release\netcoreapp3.1\linux-x64\publish\

:: debug
pushd .\bin\Debug\net6.0\linux-x64\publish\

pscp -pw Ub@490910 -r .\* ubuntu@192.168.29.90:/home/ubuntu/sandbox/BlazorContact/Api
popd
