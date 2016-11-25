## Core.Shaman

There is still a lot of flaws in EntityFrameworkCore. Many of them is really irritating. That's why first name of this library was EfCore.Irritation :).

EfCore.Shaman offers simple solution for some of problems.

## Quick start - 3 steps to fix many problems

1. Install nuget package 	

    `Install-package EfCore.Shaman`

2. Enter following code in `Up` method in each `Migration` class:

    ````csharp
   migrationBuilder.FixMigrationUp<YourDbConextClass>();
   ````

3. Enter following code in `OnModelCreating` method in `DbContext` class:

   ````csharp
   this.FixOnModelCreating(modelBuilder);
   ````

## Columns order in table

Ef uses its own sorting method for columns in table. Primary key fields are first followed by other fields in alphabetical order. Neither natural properties order in 'code first' classes nor `ColumnAttibute.Order` value is used for sorting.

To change this behaviour put code below at the end of `protected override void Up(MigrationBuilder migrationBuilder)` method in each class derived from `Migration`.
````csharp
migrationBuilder.FixMigrationUp<YourDbConextClass>();
````

New column order is based on natural properties order in 'code first' classes. This can be overrided by `ColumnAttibute.Order` value.

## Indexes

EfCore doesn't support annotation for index creation. Each index definition must be in `OnModelCreating` method in `DbContext` class. It is inconsequence leading to worse code readablity. 

EfCore.Shaman offers own `IndexAttribute` and `UniqueIndexAttribute`. In order to use this attributes put following code at the end of 'OnModelCreating' method in you `DbContext` class.

````csharp
this.FixOnModelCreating(modelBuilder);
````

### Index creation options

#### Simple one column index

Just use `IndexAttribute` and `UniqueIndexAttribute` without any parameter like below:

````csharp
[Index]
public Guid InstanceUid { get; set; }
````

#### Own index name
````csharp
[Index("MyName")]
public Guid InstanceUid { get; set; }
````

#### Multi column indexes

To create index with more than one column put `IndexAttribute` or `UniqueIndexAttribute` with some name and number related to field position in in index

````csharp
[Index("Idx_myName", 1)]
public Guid InstanceUid { get; set; }

[Index("Idx_myName", 2)]
public Guid BoxUid { get; set; }
````

You can also use names starting with `@` sign. That names will be replaced by name generated automatically by EF. 

````csharp
[Index("@1", 1)]
public Guid InstanceUid { get; set; }

[Index("@1", 2)]
public Guid BoxUid { get; set; }
````

#### Descending field sorting

`IndexAttribute` and `UniqueIndexAttribute` contains `bool IsDescending` property designed for changing sorting order of index column. This is **not yet supported**. 

````csharp
[Index("@1", 1)]
public Guid InstanceUid { get; set; }

[Index("@1", 2, true)]
public Guid BoxUid { get; set; }
````


## Code signing
Assembly distributed with nuget package is signed with `key.snk` that is not included with github repository. `mksnk.bat` script file is included instead. It it running automatically during building process. 
