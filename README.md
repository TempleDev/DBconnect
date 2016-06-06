# DBconnect

Temple University IS&amp;T Capstone Database Connection Library
<br/>
Contact the Author @[you.do.good.work@temple.edu](http://tumail.temple.edu)</br/>

**OVERVIEW**
This Library is just one class. It fixes the issues with the previous Connection class as well adds some more functionality. You are free to use it and modify it as needed.  

## Content
[Methods](#methods)<br/>
[User-Defined Table Parameters](#user-defined-table-parameters)
[SQL Merge Function](#sql-merge-function)

###Methods
####Notes:
The majority of the methods have not changed overall. The only change to all methods is that they call CloseConnection() at the end of the method now. Just to make sure that the database connection is actually close.

The new method is:
```C#
DoUpdateWithDSCmdOjb(DataSet passeddataset, String DBTableDestination)
```
The DBTableDestination is the name of the table to which you are inserting the data into. The dataset HAS to have the SAME COLUMN NAMES as the SQL table in the database.

The downsides to this method is that it does not handle duplicates. It will just push the data to the table. So if you have
```
["1", "Chips", "10"]
["3", "Dip", "20"]
["8", "Cookies", "15"]
["1", "Chips", "10"]
```
the first and last record (["1", "Chips", "10"]) will be added twice.

I would suggest using this method to initially populate your data. 
