Remove-Item -Path ./net6.0/basic/* -Recurse
Remove-Item -Path ./net6.0/advanced/* -Recurse
Remove-Item -Path ./net7.0/basic/* -Recurse
Remove-Item -Path ./net7.0/advanced/* -Recurse
Remove-Item -Path ./net8.0/basic/* -Recurse
Remove-Item -Path ./net8.0/advanced/* -Recurse

dotnet new BuildProject -pn test -go testowner -tf net6.0 -ipv false -o ./net6.0/basic/
dotnet new BuildProject -pn test -go testowner -tf net6.0 -ipv true -o ./net6.0/advanced/
dotnet new BuildProject -pn test -go testowner -tf net7.0 -ipv false -o ./net7.0/basic/
dotnet new BuildProject -pn test -go testowner -tf net7.0 -ipv true -o ./net7.0/advanced/
dotnet new BuildProject -pn test -go testowner -tf net8.0 -ipv false -o ./net8.0/basic/
dotnet new BuildProject -pn test -go testowner -tf net8.0 -ipv true -o ./net8.0/advanced/
