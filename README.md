# SQL Installer.NET

__SQL Installer.NET__ is a toolset which assists in the _development, deployment, and maintenance_ of applications which interface 
with a relational database management system (RDBMS). It supports a wide range of RDBMS products including: 

* Microsoft SQL Server (i.e. Azure)
* Oracle 
* IBM DB2 
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

First, you should break down your database into its component pieces (tables, views, etc.) and place them into your source code repository. 
The preferred breakdown is a file-per-object. You wouldn't create one large C# source file with all of your classes would you? Most development 
teams understand this process. In my view that’s not good enough. You should also have a method of building out your databases using the 
_source_ SQL scripts (DDL, DML) that now reside in your source control repository – just like you would build your regular source code into an 
assembly or executable on a build server. The goal is to make the creation of your databases consistent, deterministic, and repeatable. 
The _SQL Installer.NET_ toolset employs a fairly straightforward ‘file structure’ based processing algorithm to ensure that you meet these goals. 

There’s nothing special about the structure. It simply establishes the convention by which SQL Installer.NET will operate. 

## Script Folder Layout

The Upgrade folder contains scripts necessary to upgrade a database from version to version. Subsequent runs of the command line utility would 
recognize that there is an existing database and execute the scripts in each ‘Upgrade’ folder starting after the current release and ending with the 
last folder which would become the current release (03.00 in the example here):

* __Upgrade__
   * 01.00
   * 01.05
   * 02.10
   * 03.00

> _SQL Installer.NET_ uses a default naming convention to differentiate between the different types of SQL objects (which can be overridden)

## Script Extensions

1. PreInstall.sql  - Any prerequisites (e.g. types, defaults, users)
2. Table.sql
3. UserDefinedFunction.sql
4. View.sql
5. StoredProcedure.sql
6. Trigger.sql
7. PostInstall.sql - Anything needed post installation (e.g. bootstrap data)
8. ForeignKey.sql

> Once the database is broken down into its separate pieces each in their own source file and into the hierarchy as illustrated, you would then 
> check these into source control.
 
## Script Formatting

Aside from the aforementioned file naming conventions imposed by _SQL Installer.NET_, the database developer should be free to author SQL scripts 
as they do normally (e.g. comments, spacing/tabs, etc). However, for the _Oracle_ and _IBM DB2_ providers, there is a special exception. 
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

> __Note:__ For the Oracle and DB2 providers only, the forward slash (/) must be at the end of the line or by itself on a separate line

## Database vs User/Schema

Every attempt was made in the design of SQL Installer.NET to provide a consistent implementation across all data providers. However, 
there is an issue which could not be avoided - how the provider itself defines a 'database'. _Oracle_, for example, has the concept of 
an _instance_ but does not further segregate that instance into _databases_. The _IBM DB2_ provider has both an _instance_ as well as 
a _database_ however it is not possible to create a _database_ from a true client connection. Both the Oracle instance and/or a DB2 database 
must be setup prior to SQL Installer.NET execution. For these two providers, what you would configure in SQL Installer.NET as a _database_ is 
really a _schema_ (for Oracle it is both user and schema). All other providers which are included with SQL Installer.NET have the concept 
of a database which is analogous to a SQL Installer.NET database.

# Chapter 2 – Running

How you would use the SQLInstaller.NET tools depends on both whether you are in development or production/deployment mode and whether you 
would be creating a new database (Install) or migrating an existing database (Upgrade).  

Once the script directory tree is located on the target machine, you will logon locally and open a command prompt. The command line syntax is as follows:

> SQLInstaller.exe _optional_path_to_xml_file_ _optional command-line parameters_

The command line utility is driven either through a special XML file or through command-line parameters or a combination of both XML file 
and command-line parameters. By default it will look for a file called _SQLInstaller.xml_ in the current working directory. You can specify 
all parameters on the command line and forego the need for the XML file completely. If the XML file is present and you specify command-line 
parameters, then the command-line parameters will override any values present in the XML file. 

* /ConnectionString | /conn - The database connection string (e.g. /conn="Data Source=localhost;Integrated Security=SSPI")
* /Database | /d - The name of the database to create or upgrade (e.g. /d=MyDatabase)
* /InstallPath | /ins - The relative path to the directory containing the install scripts (e.g. /inst=".")
* /FileTypes| /f - A comma separated list of the name and order of file types used during execution (e.g. /f="Table,View,StoredProcedure")
* /NoPrompt | /nop - Do not prompt for upgrade
* /Options | /o - Runtime options (see configuration file reference below for values) (e.g. /o="Retry,Verbose")
* /Provider | /p - The data provider to use (SqlServer,Oracle,PostGres,DB2,FireBird,MySQL,SQLite,Teradata) (e.g. /p=Oracle)
* /ScriptExtension | /ext - The extension to use for the SQL scripts (defaults to sql) (e.g. /ext=txt)
* /UpgradePath | /upg - The relative path to the directory containing the upgrade scripts (e.g. /upg="..\Upgrade")
* /WriteConfig | /w - Writes the configuration values to the XML file

## Install

During development, you can add the Drop option to always drop the database beforehand. This will ensure that you have a complete refresh of 
the database from source. Obviously, for a production install you would not want to drop the existing database. In this case, the utility 
will first attempt to determine the database version and report that the database already exists at a given version and exit. 

## Upgrade

The upgrade process starts just like the install process. The SQLInstaller.NET command line utility will recognize that the database already exists, 
scan the Upgrade folder for releases, and prompt you to upgrade (assuming the database is not already at the current release). 

# Appendix A: Configuration File Reference
----
```xml
<Parameters Options="Create Drop Verbose ExitCode" IsProtected="false" NoPrompt="false" 
ScriptExtension="sql" InstallPath="Install" UpgradePath="Upgrade">
   <Database>DATABASE_NAME</Database>
   <Provider Name="SqlServer|Azure|Oracle|PostGres|DB2|FireBird|MySQL|SQLite|Teradata" />
   <ConnectionString>Data Source=dbsrv;Integrated Security=SSPI</ConnectionString>
   <FileTypes>
      <FileType Name="Table" Description="TABLES" IsDisabled="false" IsGlobal="false" HaltOnError="false"/>
   </FileTypes>
</Parameters>
```

__Options__ - specify which options are in effect.
* Create – create database if it does not exist
* Drop – drop database if it exists
* Retry – retry the last upgrade
* Verbose – output all status messages
* ExitCode - sets the process exit code (errorLevel) for SQL errors 

* __IsProtected__ - specify whether or not the ConnectionString property has been encrypted
* __NoPrompt__ - do not prompt user to upgrade (automatically approve)
* __ScriptExtension__ - The file extension used for your script files
* __InstallPath__ - The root path (relative to this file) where the install scripts are located
* __UpgradePath__ - The root path (relative to this file) where the upgrade scripts are located
* __Database__ - name of the database to install/upgrade
* __Provider__ – Set _Name_ attribute to one of standard or custom data providers: __SqlServer|Oracle|PostGres|DB2|FireBird|MySQL|SQLite|Teradata__
* __ConnectionString__ – ADO.NET connection string

* __FileTypes__ – A list of file types to process
* __Name__ - The root name of the file type. This is used in conjunction with the ScriptExtension attribute to construct a search filter
* __IsDisabled__ - Toggles processing of these file types on and off.
* __IsGlobal__ - Run scripts in the global database context
* __HaltOnError__ - Exit completely if any error occurs 

# Appendix B: Adding/Customizing Data Providers
----

```xml
<?xml version="1.0" encoding="utf-8"?>
<ProviderFactory xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="ProviderFactory">
   <!--
   The ProviderFactory configuration XML is used by SQL Installer.NET to control interaction
   with the underlying ADO.NET data provider. This file is an embedded resource within the
   binary. However, if you rename the ProviderFactory.xml.orig (remove orig) found within
   the SQL Installer.NET installation folder, you can customize these per installation. 
   There are five scripts that you can customize:
   
   1.   Exists: checks to see if the target database already exists. Will replace {0} 
      with the database name passed in from the configuration (SQLInstaller.xml). 
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
      
   The Provider element is the same Provider used within the SQLInstaller.xml config file.
   You can override any or all of the Provider elements/attributes within SQLInstaller.xml. These
   will be merged back with the global ProviderFactory xml at runtime.
   
   You may also define your own provider within either the ProviderFactory.xml file or within
   each of your SQLInstaller.xml configuration files respectively. This allows you to add
   support for other ADO.NET data providers as necessary.
   -->
   <Providers>
      <!--
      NOTE: only showing the PostGres provider here for brevity. You can see the full provider 
      factory configuration by opening the ProviderFactory.xml.orig file found within the installation
      folder.
      -->
      <Provider Name="PostGres" InvariantName="Npgsql">
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
