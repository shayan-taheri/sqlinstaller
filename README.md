# SQL Installer

> __Install the official NuGet Package here:__ [SQL Installer](https://www.nuget.org/packages/JobTech.SqlInstaller/)

> __The deprecated version (.NET full only) is still available here:__ [Old SQL Installer.NET](https://www.nuget.org/packages/SQLInstaller.NET/)

__SQL Installer__ is a toolset which assists in the _development, deployment, and maintenance_ of applications which interface 
with a relational database management system (RDBMS). It supports a wide range of RDBMS products including: 

* Microsoft SQL Server (i.e. Azure)
* Oracle 
* PostGreSQL 
* Firebird SQL  
* MySQL
* SQLite
* Teradata


# Table of Contents

* __Chapter 1 – Development__
   * __Database Version Management__
   * __Script Folder Layout__
   * __Script Extensions__
   * __Script Formatting__
   * __Database vs User/Schema__
* __Chapter 2 – Running__
   * __Install__
   * __Upgrade__
* __Appendix A: Configuration File Reference__
* __Appendix B: Adding/Customizing Data Providers__


# Chapter 1 – Development

The first step is to separate your scripts that create database objects (tables, views, etc.) and place them into your source code repository. 
The preferred breakdown is a file-per-object. You wouldn't create one large C# source file with all of your classes would you?  

Next you will need a method of building out your databases using these _source_ SQL scripts (DDL, DML) that now reside in your source control 
repository – just like you would build your regular source code into an assembly or executable on a build server. The goal is to make the creation 
of your databases consistent, deterministic, and repeatable. 

This tool employs a fairly straightforward ‘file structure’ based processing algorithm to ensure that you meet these goals. 

> There’s nothing special about the structure. It simply establishes the convention by which SQL Installer will operate. 

## Script Folder Layout (example)

The Upgrade folder (default name) contains scripts necessary to upgrade a database from version to version. Subsequent runs of the command line 
utility would recognize that there is an existing database and execute the scripts in each ‘Upgrade’ folder starting after the current release 
and ending with the last folder which would become the current release (03.00 in the example here):

* __Upgrade__
   * 01.00
   * 01.05
   * 02.10
   * 03.00

> Once the database objets are seperated out such that each has its own source file and placed into the hierarchy as illustrated, you would then 
> check these into source control and add a build step to call the SQL Installer tool to build out the database in conjunction with other source.

## Script Extensions (default)

1. PreInstall.sql  - Any prerequisites (e.g. types, defaults, users)
2. Table.sql
3. UserDefinedFunction.sql
4. View.sql
5. StoredProcedure.sql
6. Trigger.sql
7. PostInstall.sql - Anything needed post installation (e.g. bootstrap data)
8. ForeignKey.sql

> _SQL Installer_ uses this default naming convention to differentiate between the different types of SQL objects (which can be overridden)
 
## Script Formatting

Aside from the aforementioned file naming conventions imposed by _SQL Installer_, the database developer should be free to author SQL scripts 
as they do normally (e.g. comments, spacing/tabs, etc). However, for the _Oracle_ provider, there is a special exception. 
When scripting stored procedures (or anonymous blocks) a special terminating character is required to differentiate between the end of the procedure 
and individual statements. You must use a forward slash (/) to indicate the end of a procedure/block. For example:

```sql
BEGIN
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (1, 'Beverages', 'Soft drinks, coffees, teas, beers, and ales');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (2, 'Condiments', 'Sweet and savory sauces, relishes, spreads, and seasonings');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (3, 'Confections', 'Desserts, candies, and sweet breads');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (4, 'Dairy Products', 'Cheeses');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (5, 'Grains/Cereals', 'Breads, crackers, pasta, and cereal');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (6, 'Meat/Poultry', 'Prepared meats');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (7, 'Produce', 'Dried fruit and bean curd');
INSERT INTO Categories (CategoryID, CategoryName, Description)
VALUES (8, 'Seafood', 'Seaweed and fish');
END;
/
```

> __Note:__ For the Oracle provider only, the forward slash (/) must be at the end of the line or by itself on a separate line

## Database vs User/Schema

Every attempt was made in the design of SQL Installer to provide a consistent implementation across all data providers. However, 
there is an issue which could not be avoided - how the provider itself defines a 'database'. _Oracle_ has the concept of 
an _instance_ but does not further segregate that instance into _databases_. Therefore, the Oracle instance 
must be setup prior to SQL Installer execution. What you would configure in SQL Installer as a _database_ is 
really a _schema_ (for Oracle it is both user and schema). All other providers which are included with SQL Installer have the concept 
of a database which is analogous to a SQL Installer database.

# Chapter 2 – Running

Once the script directory tree is located on the target machine, you will logon locally and open a command prompt or
add a pre-post build command in your IDE (e.g. Visual Studio). __Note__ that the path to .nuget may differ between environments. For example:  

> __Visual Studio / Windows (pre-post build)__  
> dotnet "$(USERPROFILE)\.nuget\packages\jobtech.sqlinstaller\2.0.1\tools\netcoreapp2.2\SqlInstaller.dll" "$(ProjectDir)\sqlinstaller.$(Configuration).xml" 

> __Command Line MacOS or Linux__  
> dotnet ~/.nuget/packages/jobtech.sqlinstaller/2.0.1/tools/netcoreapp2.2/SqlInstaller.dll /d=MyDatabase /conn="$CONNECTIONSTRING"

During execution, the tool obtains its parameters through a special XML file or through command-line options or a combination of both XML file 
and command-line options. By default it will look for a file called _sqlinstaller.xml_ in the current working directory. You can specify 
all parameters on the command line and forego the need for the XML file completely. If the XML file is present and you specify command-line 
parameters, then the command-line parameters will override any values present in the XML file. 

* /ConnectionString | /conn - The database connection string (e.g. /conn="Data Source=localhost;Integrated Security=SSPI")
* /Database | /d - The name of the database to create or upgrade (e.g. /d=MyDatabase)
* /InstallPath | /ins - The relative path to the directory containing the install scripts (e.g. /inst=".")
* /FileTypes| /f - A comma separated list of the name and order of file types used during execution (e.g. /f="Table,View,StoredProcedure")
* /NoPrompt | /nop - Do not prompt for upgrade
* /Options | /o - Runtime options (see configuration file reference below for values) (e.g. /o="Retry,Verbose")
* /Provider | /p - The data provider to use (SqlServer,Oracle,PostGres,FireBird,MySQL,SQLite,Teradata) (e.g. /p=Oracle)
* /ScriptExtension | /ext - The extension to use for the SQL scripts (defaults to sql) (e.g. /ext=txt)
* /UpgradePath | /upg - The relative path to the directory containing the upgrade scripts (e.g. /upg="..\Upgrade")
* /WriteConfig | /w - Writes the configuration values to the XML file

## Install

During development, you can add the __Drop__ option to always drop the database beforehand. This will ensure that you have a complete refresh of 
the database from source. For a production install you would remove the __Drop__ option to ensure any existing database is kept in place. If the
database does not exist the install will continue. Otherwise, the tool will perform an __Upgrade__.

## Upgrade

If SQL Installer detects an existing database, and the __Drop__ option is not set, it will scan the Upgrade folder for releases, report back the
current database version, and optionally prompt you to upgrade (if necessary and the /NoPrompt parameter is either missing or set to false).  

> __Note__ that it is recommended to have just an Upgrade folder and no seperate Install folder. In this case (no Install folder) it would simply 
> perform all the upgrades in order cycling through Upgrade folder and its children. Having a seperate Install folder (path) can be helpful in cases 
> where you need to have a 'clean' install of the latest version rather than perform each upgrade (which may have complex and long running commands).

# Appendix A: Configuration File Reference
----
```xml
<Parameters Options="Create Drop Verbose ExitCode" IsProtected="false" NoPrompt="false" ScriptExtension="sql" InstallPath="Install" UpgradePath="Upgrade">
   <Database>DATABASE_NAME</Database>
   <Provider Name="SqlServer|Azure|Oracle|PostGres|FireBird|MySQL|SQLite|Teradata">
   		<Scripts>
			<Script Type="Exists|Drop|Create|GetVersion|SetVersion">
			</Script>
		</Scripts>
   </Provider>
   <ConnectionString>Data Source=dbsrv;Integrated Security=SSPI</ConnectionString>
   <FileTypes>
      <FileType Name="Table" Description="TABLES" IsDisabled="false" IsGlobal="false" HaltOnError="false"/>
   </FileTypes>
</Parameters>
```

__Options__ - specify which options are in effect.  
* Create – create database if it does not exist.  
* Drop – drop database if it exists.  
* Retry – retry the last upgrade.  
* Verbose – output all status messages.  
* ExitCode - sets the process exit code (errorLevel) for SQL errors.  

__IsProtected__ - specify whether or not the ConnectionString property has been encrypted.  
__NoPrompt__ - do not prompt user to upgrade (automatically approve).  
__ScriptExtension__ - The file extension used for your script files.  
__InstallPath__ - The root path (relative to this file) where the install scripts are located.  
__UpgradePath__ - The root path (relative to this file) where the upgrade scripts are located.  

__Database__ - name of the database to install/upgrade.  
__Provider__ – Set _Name_ attribute to one of standard data providers.  
* Scripts - Use this section to override the scripts used to manage the overall database (e.g. Create). See Appendix B.  

__ConnectionString__ – ADO.NET connection string.  

__FileTypes__ – A list of file types to process.  
* Name - The root name of the file type. This is used in conjunction with the ScriptExtension attribute to construct a search filter.  
* IsDisabled - Toggles processing of these file types on and off.  
* IsGlobal - Run scripts in the global database context.  
* HaltOnError - Exit completely if any error occurs.  

# Appendix B: Adding/Customizing Data Providers
----

```xml
<?xml version="1.0" encoding="utf-8"?>
<ProviderFactory xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="ProviderFactory">
   <!--
   The ProviderFactory configuration XML is used by SQL Installer to control interaction
   with the underlying ADO.NET data provider. You can override any of these settings within your own sqlinstaller.xml file. 
   There are five scripts that you can customize:
   
   1.   Exists: checks to see if the target database already exists. Will replace {0} 
      with the database name passed in from the configuration (sqlnstaller.xml). 
      Simply return a numeric value greater than zero if the database exists. 
      Note: for Oracle you will check for a USER account.
   2.   Drop: drops the target database (or USER if Oracle). Will replace {0} with the
      database name.
   3.   Create: creates the target database (or USER if Oracle). Will replace {0} with
      the database name.
   4.   GetVersion: retrieves the version information. Will replace {0} with the 
      database name.
   5.   SetVersion: sets the version information. Will replace {0} with the 
      database name, {1} with the version, and {2} with the user running the install.
   -->
   <Providers>
      <!--
      NOTE: only showing the PostGres provider here for brevity. You can see the full provider 
      factory configuration by opening the ProviderFactory.xml.orig file found within the installation
      folder.
      -->
      <Provider Name="PostGres">
         <Scripts>
            <Script Type="Exists">SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname = '{0}'</Script>
            <Script Type="Drop">DROP DATABASE {0}</Script>
            <Script Type="Create">CREATE DATABASE {0}</Script>
            <Script Type="GetVersion">SELECT version_info FROM db_version</Script>
            <Script Type="SetVersion">
CREATE OR REPLACE VIEW db_version AS SELECT CAST('{1};{2}' AS VARCHAR(512)) AS version_info
            </Script>
         </Scripts>
      </Provider>
   </Providers>
</ProviderFactory>
```
