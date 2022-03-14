# XmlSanitizer
An attempt to port tiny PHP xml sanitizing code to net core 3.1 and measure performance. It utilizes a line by line xml parsing and have some heavy assumption 
about the schema of the input xml and the existing skus "csv". These would, hopefully, get generalized.

## Build
To build the tool you'll need [dot net core 3.1 sdk](https://dotnet.microsoft.com/download/dotnet-core/3.1) installed on your machine.
run the following command at the root repo folder
```
dotnet build
```
## Develop
You could use [Visual Studio](https://visualstudio.microsoft.com/downloads/)(VS Community, VS Code or VS for Mac) to build and test. 

## Publish# XmlSanitizer
An attempt to port tiny PHP xml sanitizing code to net core 3.1 and measure performance. It utilizes a line by line xml parsing and have some heavy assumption 
about the schema of the input xml and the existing skus "csv". These would, hopefully, get generalized.

## Build
To build the tool you'll need [dot net core 3.1 sdk](https://dotnet.microsoft.com/download/dotnet-core/3.1) installed on your machine.
run the following command at the root repo folder
```
dotnet build
```
## Develop
You could use [Visual Studio](https://visualstudio.microsoft.com/downloads/)(VS Community, VS Code or VS for Mac) to build and test. 

## Publish
You would need to use the [dotnet publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) command to create a bundle.

Here's an example publishing for ubuntu 18.04, in release configuration with self-contained mode enabled
```
dotnet publish -c release -r ubuntu.16.04-x64 --self-contained
```
from the <b>src</b> folder.
When it compiles successfully you could grab the contents of src\XmlSanitizer.CLI\bin\Release\netcoreapp3.1\ubuntu.16.04-x64\publish and this is your 
self-contained ubuntu18.04 bundle.

---
**NOTE**

After deploying the publish folder to your ubuntu machine you'll need to  execute:
```
chmod 777 ./XmlSanitizer
```
so the tool gets execution permissions.

---

## Test
To execute the tests you need to have dot net core 3.1 installed on the machine. Then go to the src folder and execute:
```
dotnet test
```


You would need to use the [dotnet publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) command to create a bundle.

Here's an example publishing for ubuntu 18.04, in release configuration with self-contained mode eneabled
```
dotnet publish -c release -r ubuntu.16.04-x64 --self-contained
```
from the <b>src</b> folder.
When it comples succesfully you could grab the contents of src\XmlSanitizer.CLI\bin\Release\netcoreapp3.1\ubuntu.16.04-x64\publish and this is your 
self-contained ubuntu18.04 bundle.

---
**NOTE**

After deloying the publish folder to your ubuntu machine you'll need to  execute:
```
chmod 777 ./XmlSanitizer
```
so the tool gets execution permitions.

---

## Test
To execute the tests you need to have dot net core 3.1 installed on the machine. Then go to the src folder and execute:
```
dotnet test
```
