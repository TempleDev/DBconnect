# DBconnect

Temple University IS&amp;T Capstone Database Connection Library
<br/>
Contact the Author @[chorizo.burrito@temple.edu](http://tumail.temple.edu)</br/>

**OVERVIEW**
This Library is just one class. It fixes the issues with the previous Connection class as well adds some more functionality. You are free to use it and modify it as needed.  

## Content
[Methods](#methods)<br/>
[User-Defined Table Parameters](#user-defined-table-parameters)<br/>
[SQL Merge Function](#sql-merge-function)<br/>
[Search Stored Procedure](#search-stored-procedure)<br/>

###Methods
####Notes:
The majority of the methods have not changed overall. The only change to all methods is that they call CloseConnection() at the end of the method now. Just to make sure that the database connection is actually close.

The new method is:
```csharp
DoUpdateWithDSCmdOjb(DataSet passeddataset, String DBTableDestination)
```
The DBTableDestination is the name of the table to which you are inserting the data into. The dataset HAS to have the SAME COLUMN NAMES as the SQL table in the database.

The downsides to this method is that it does not handle duplicates. It will just push the data to the table. So if you have
```sql
["1", "Chips", "10"]
["3", "Dip", "20"]
["8", "Cookies", "15"]
["1", "Chips", "10"]
```
the first and last record (["1", "Chips", "10"]) will be added twice.

I would suggest using this method to initially populate your data.

###User-Defined Table Parameters
####Notes: 
This of a User-Defined Table as a data type for SQL. Once you create it you can call it in a stored procedure. For systems that need to do a database call for multiple records it makes it more efficent to use a User-Defined Table instead of making multiple calls. It is best to use them in conjunction with SQL Merge Functions (next section). A User-Defined Table will need to have some but not all of the same columns and types as the table you plan on using it with in your database. If columns allow null values or something that can be set within the stored procedure, like a time stamp, you do not need to as that column within the User-Defined Table Type. 

####How to create them: 
When in your database in either Visual Studio 2012/2015 or Microsoft SQL Server Manager Studio go down to your **Programmability** folder. Within there open **Types**. Then right-click **User-Defined Table Type**. Click on the first option **Add New User-Defined Table Type...**. A script will open. As you will see the first line is **CREATE TYPE [dbo].[UserDefinedTableType] AS TABLE**. If you have ever made a table using a SQL script before this is very similar. A thing to keep in mind, as mentioned earlier, is to have the same types as the table you plan to use this user-defined-table with. Here is an example of what it will look like:
```sql
CREATE TYPE [dbo].[ExampleOfUserDefinedTable] AS TABLE
(
	id (INT) NOT NULL,
	first_name VARCHAR(50) NOT NULL,
	last_name VARCHAR(50) NOT NULL,
	departmentID (INT) NOT NULL
)
```
The **NOT NULL** isn't required but it is a good practice to put them.

When using a User-Defined Table in a stored procedure all you will need to do is create the table within C#. You can attach the C# table to a **SqlCommand** object no problem. Example of this:
```csharp
	DataTable t = new DataTable();
	t.Columns.Add("id")
	t.Columns.Add("first_name");
	t.Columns.Add("last_name"); 
	t.Columns.Add("departmentID");
```
The column names are the same as the user-defined-table that was created in the previous section. Doing so make it a lot easier following how the code works. Once the table is full of data you can add it as a parameter to a stored procedure.
```csharp
	public static void MergeExample(DataTable t)
	{
		DBConnect myDB =  new DBConnect();
		SqlCommand objEx = new SqlCommand();
		objEx.CommandType = CommandType.StoreProcedure;
		ojbEx.CommandText = "MergeExampleSP";
		objEx.Parameters.AddWithValue("@tableType", t); 
		myDB.DoUpdateWithDSCmdOjb(objEx);
		myDB.CloseConnection();
	}
```
**FOR THE LOVE OF GOD CLOSE ALL YOUR DATABASE CONNECTIONS!**

###SQL Merge Function
####Notes:
SQL Merge is a really powerful function. It can take data merge it into a table either by doing **INSERT** or doing an **UPDATE**. 

Now this is the example of the what a stored procedure will look like. Look at the SQL code first before reading the explanation. 
```sql
	ALTER PROCEDURE [dbo].[MergeExampleSP]
	(
		@tableType ExampleOfUserDefinedTable READONLY --The READONLY is required so the database knows it isn't doing anything except reading the table.
	)
	AS
	BEGIN
		MERGE INTO dbo.Faculty AS m1 --m1 an alias for Faculty
		USING @tblAccessRequests AS m2 --m2 is alias for the parameter
		ON m1.id = m2.id --id is the primary key for Faculty
		WHEN MATCHED THEN --If primary key matches between both tables UPDATE fires
		UPDATE SET m1.first_name = m2.first_name,
			m1.last_name = m2.last_name,
			m1.creation_date = GETDATE(),
			m1.last_modified = GETDATE()
		WHEN NOT MATCHED BY TARGET THEN --If primary key does not match INSERT fires
		INSERT (id, first_name, last_name, creation_date, last_modified)
		VALUES (m2.id, m2.first_name, m2.last_name, GETDATE(), GETDATE()); --The ; ends the function
	END
```
After **MERGE INTO** is the table within the database where the data is being merge into. The **dbo.** stands for **DATABASE OWNER**. It is required to make the **MERGE** function work. 