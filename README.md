# SecureEnvironmentSettings
## keep your environment settings secure, encrypted :closed_lock_with_key: and isolated

This library helps you to keep your config files secure because once you call the **Encrypt** method the config sections and connections strings will be encrypted using a specific value from the machine (or devices) running your code.

![settings](https://user-images.githubusercontent.com/74436244/180286333-538f96fd-ffbf-443a-b6c4-f0e96e3f6baf.png)

## Features
- :closed_lock_with_key: Encrypt ConfigSections and ConnectionStrings inside your config files
- :unlock: Decrypt config sections and connectionStrings
- :pencil2: Update configuration values directly on your config file and encrypt
- :open_file_folder: Get secure access to all your parameters based on your environments
- :open_file_folder: Get easy access to shared settings values

## How to install

First of all you need to download the latest version of the library, once you have downloaded you should save into a specif folder and include in your project, finally you need to import in to your dependency

#### 1.- Download and save in your local files
#### 2.- Add reference to this library
In references righ-click and then browse for your library
#
![image](https://user-images.githubusercontent.com/74436244/180444679-50a9783b-9f86-44e3-95c5-d15fa29750d9.png)
#### 3.- Use it as you need

## How to use

First, you need to config your main app.config in order to add the configSections, you should have at leas your file like thise example

### :memo: App.config

This is an example of how you can set up your main app.config or web.config

```xml
<?xml version = "1.0" encoding="utf-8" ?>
<configuration>
<configSections>
    <sectionGroup name = "EnvironmentSettings" type="System.Configuration.ApplicationSettingsGroup">
		<section name = "Production" type="System.Configuration.AppSettingsSection"/>
		<section name = "Development" type="System.Configuration.AppSettingsSection"/>
		<section name = "QA" type="System.Configuration.AppSettingsSection"/>
    </sectionGroup>
</configSections>
<connectionStrings>
    <add name = "MovieDBContextPrd"
    connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesPrd.mdf"
    providerName="System.Data.SqlClient">
    <add name = "MovieDBContextDev"
    connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf"
    providerName="System.Data.SqlClient">
   <add name = "MovieDBContextQA"
    connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesQA.mdf"
    providerName="System.Data.SqlClient">
</connectionStrings>
<EnvironmentSettings>
    <Production file = "Config\Production.config" ></ Production >
    <Development>
      <add key = "TestKey" value="DevValue"/>
      <add key = "connection" value="MovieDBContextDev"/>
    </Development>
    <QA>
      <add key = "TestKey" value="QAValue"/>
      <add key = "connection" value="MovieDBContextQA"/>
    </QA>
</EnvironmentSettings>
<appSettings>
    <add key = "CurrentEnvironment" value="Development"/>
    <add key = "CommonKey" value="CommonValue"/>
</appSettings>
</configuration>
```
There are specific key sections than you can change in order to get your specific parameters values

<details><summary>ConfigSections</summary>
  
  <p>
    
### ConfigSections
This is the main configuration that you need to add in your main config file, you can name your sections name as you wish, **but the sectionGroup name must be _EnvironmentSettings_ , :warning: so it can not be change**

```xml
<configSections>
  <sectionGroup name = "EnvironmentSettings" type="System.Configuration.ApplicationSettingsGroup">
    <section name = "Production" type="System.Configuration.AppSettingsSection"/>
    <section name = "Development" type="System.Configuration.AppSettingsSection"/>
    <section name = "QA" type="System.Configuration.AppSettingsSection"/>
  </sectionGroup>
</configSections>
```
  </p>
  </details>
  
<details><summary>EnvironmentSettings</summary>
  
<p>

### EnvironmentSettings

This sections are used to specify your custom parameters based on your environment, so when you call one of these values from your code, the environmentSettings engine
 tries to load the specific value based on the **CurrentEnvironment** parameter on your appSettings
    
#### :memo: app.config

Inside any of your environmentSections you can add your parameters directly as a KeyValue like Development or QA section.
  
```xml
<EnvironmentSettings>
  <Production file = "Config\Production.config" ></ Production >
  <Development>
    <add key = "TestKey" value="DevValue"/>
    <add key = "connection" value="MovieDBContextDev"/>
  </Development>
    <QA>
      <add key = "TestKey" value="QAValue"/>
      <add key = "connection" value="MovieDBContextQA"/>
    </QA>
</EnvironmentSettings>
```
  
#### :memo: Config\Production.config
You can also use an external file in order to keep your config files isolated like Production section [^1].
  
```xml
<Production>
  <add key="TestKey" value="ProductionValue"/>
  <add key="connection" value="MovieDBContextPrd"/>
</Production>
```

[^1]: For more information [Using External Config Files in .NET Applications] (https://www.codeproject.com/Articles/1075487/Using-External-Config-Files-in-NET-Applications).
 
Once you have setup your envirnmentSettings, **you must specify the current environment** otherwise you will give an error
```xml
<appSettings>
  <add key = "CurrentEnvironment" value="Development"/>
</appSettings>
```
  
So, you can call any of your values from your application code
  
```cs
string valueFromAppConfig = SecureEnvironmentSettings.Settings["TestKey"].Value;
Console.WriteLine($"CurrentValue: {valueFromAppConfig}");
/// CurrentValue: DevValue
```
  
</p>
  </details>
    
<details><summary>ConnectionStrings</summary>
  <p>

### ConnectionStrings
    
You can specify your all your connections strings, you need first, add your connectionString on that section, name it, and then add that name in your specific environmentSection, so you can call it from the SecureEnvironmentSettings library
    
##### 1.- Add your connection

```xml
<connectionStrings>
  <add name = "MovieDBContextPrd" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesPrd.mdf" providerName="System.Data.SqlClient">
  <add name = "MovieDBContextDev" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf" providerName="System.Data.SqlClient">
  <add name = "MovieDBContextQA" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesQA.mdf"providerName="System.Data.SqlClient">
</connectionStrings>
      
```

#### 2.- Add your connectionString name in your specific environment 
```xml
<Development>
  <add key = "connection" value="MovieDBContextDev"/>
</Development>
```
    
#### 3.- Call it from your code
```cs
string connection = SecureEnvironmentSettings.GetConnectionString("MovieDBContextDev");
Console.WriteLine($"ConnectionValue: {connection}");
  /// ConnectionValue: Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf" providerName="System.Data.SqlClient
```
    
  </p>
 </details>
