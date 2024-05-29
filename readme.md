# DesktopBusinessAppSharpBuilder
<div align="center">
  <img src="https://salvatoreamaddio.co.uk/img/csharp.png" width="200" height="200"/>
  <img src="https://salvatoreamaddio.co.uk/img/sql.png" width="190" height="190" />
  <img src="https://salvatoreamaddio.co.uk/img/wpf.png" width="200" height="200" />
</div>

## Description
DesktopBusinessAppSharpBuilder is a C# framework built on .NET, utilizing Windows Presentation Foundation (WPF). 
It is designed to expedite the creation of business applications by offering a comprehensive set of tools and classes 
designed for efficiency and scalability. This framework is built on .NET 8.0 specifically for Windows. 
This means the application will use the .NET 8.0 runtime and is intended to run on the Windows operating system. 

## Problem Statement:
Over the years, I recognized a recurring challenge in the development of business-oriented applications: the need for a streamlined, 
efficient approach that reduces development time while maintaining high standards of functionality and performance. 
This realization led to the creation of DesktopBusinessAppSharpBuilder.

## Purpose:
The primary objective of DesktopBusinessAppSharpBuilder is to accelerate the development of business applications by providing a robust set 
of custom controls, classes, and utilities. This allows developers to focus more on business logic and less on the repetitive tasks often 
associated with application development. Developers can focus on designing the user interface in XAML, defining classes that represent 
their database tables, and implementing any other business logic their application might require. 
By handling routine tasks and providing essential components, the framework simplifies development, 
allowing programmers to work more efficiently and effectively on their projects while the Framework takes care of 
various aspects of Windows Presentation Foundation (WPF).

## Key Features:
- **Custom ORM:**
  - Includes a custom Object-Relational Mapping (ORM) system. Automatically builds queries based on the defined models, simplifying database operations.

- **Database Management:**
  - Supports MySQL and SQLite databases.
  - Provides a set of classes to manage database operations seamlessly.

- **MVC Pattern:**
  - Implements the Model-View-Controller (MVC) pattern.
  - Abstract classes and interfaces designed to manage the Model and Controller components effectively.

- **Data Binding:**
  - Despite following the MVC pattern, data binding is extensively used to simplify UI updates and interactions.

- **Custom Controls:**
  - Includes custom WPF controls such as Form, FormList, and SubForms, ReportViewer which are not available in standard WPF.

- **Win32 API Integration:**
  - Uses the Win32 API to achieve advanced functionalities that enhance the capabilities of WPF applications.

- **Security:**
  - Utilizes the Windows Credential Manager System to securely store and manage sensitive information such as database connection strings and passwords.

- **Email Integration:**
  - Allows integration of your own email service to send emails directly from the application.

- **Excel Integration:**
  - Provides classes to easily create and read Excel files through COM (Component Object Model). Supports generating reports and data export/import functionalities.

- **Single File publishing:**
  - The Application can be distributed as a Single File, getting around the SQLite assembly issue.

## NuGet Packages used:
This framework was build by using the following NuGet Packages:
- **Microsoft.Xaml.Behaviors.Wpf** (Version 1.1.77); Author: Microsoft.
- **Refractored.MvvmHelpers** (Version 1.6.2); Author: James Montemagno.
- **System.Data.SqlClient** (Version 4.8.6); Author: Microsoft.
- **System.Data.SQLite** (Version 1.0.118); Authors: SQLite Development Team.
- **System.Drawing.Common** (Version 8.0.5); Author: Microsoft.
- **System.Linq.Async** (Version 6.0.1); Authors: Microsoft and other contributors.
- **MailKit** (Version 4.6.0); Author: Jeffrey Stedfast.
- **MimeKit** (Version 4.6.0); Author: Jeffrey Stedfast.
- **MySqlConnector** (Version 2.2.7); Author: Bradley Grainger.
- **System.Management** (Version 8.0.0); Author: Microsoft.

## Architecture:
Picture 1 below shows the MCV pattern handled by the Framework.

![Architecture](https://raw.githubusercontent.com/SalvatoreAmaddio/DesktopBusinessAppSharpBuilder/master/architecture.png)

**AbstractModel** represents the structure of a table within a database. It not only defines the schema but can also be extended to include methods and additional properties, just like any other class. This class utilizes a set of custom attributes to specify which properties are primary keys, foreign keys, or normal fields. This metadata is then sent to the QueryBuilder class (the ORM), which generates default SQL statements for SELECT and all CRUD (Create, Read, Update, Delete) operations.

**AbstractController** is a class that accepts a generic parameter of type M, where M must be an instance of AbstractModel. This class serves as the intermediary between the GUI and the AbstractModel. It manages the communication, ensuring that user inputs from the GUI are properly handled and that the AbstractModel is updated accordingly. Additionally, it performs database queries and updates, facilitating seamless interaction between the user interface and the underlying data.

**Windows Presentation Foundation (WPF)** is a GUI framework used for building rich desktop applications on Windows. This Framework further enrich WPF by bringing a set of Custom controls such as Form, FormList, SubForm, and ReportViewer are designed to communicate with the AbstractController through their DataContext property. This design ensures that the controls can seamlessly interact with the controller, facilitating data binding and user interface updates.

## Demo Project:
You can download a Demo of this Framework in action [here](https://github.com/SalvatoreAmaddio/MyApplicationDemo)

## Demo exe:
You can download a executable of the Demo [here](https://drive.google.com/file/d/1uMlZIEcip69ItPvh13N4pCltF4sF0QTr/view?usp=drive_link)

## Getting Started:
- Download the NuGet package **DesktopBusinessAppSharpBuilder**
- Open your App.xaml file
- Merge the ResourceDictionary as shown below:
```xml
 <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/FrontEnd;component/Themes/FrontEndDictionary.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
```

- Structure your project as follow:

| Folders/Files | Description                                                |
|---------------|------------------------------------------------------------|
| View          | Folder for xaml files.                                     |
| Controller    | Folder for classes extending AbstractController<M>         |
| Model         | Folder for classes extending AbstractModel                 |
| Themes        | Folder for styling xaml files (optional)                   |
| Data          | Folder for the Database (in this example a SQLite database)|
| Images        | Folder for images to be used (optional)                    |
| App.xaml      | The file which is the entry point of every WPF Application |

Create your first Form Window:

- Create a new Window File in View
- Add the following namespace:
```xml
  xmlns:fr="clr-namespace:FrontEnd.Forms;assembly=FrontEnd"
```
- Add a Form Control:
```xml
  <fr:Form>
        <!--Put your content here, like a Grid or a StackPanel-->
  </fr:Form>
```

## Define your first Model:
Assuming you have a SQLite database in the Data folder, you must create a Model class for each Table in your database.

- Create a C# file in the Model folder.
- Extends **AbstractModel**.
- Each Model has to have a Parameterless constructors.
- Create a constructor that takes **DbDataReader** reader as argument.

 ```csharp
    namespace MyApplication.Model
    {
         public class Employee : AbstractModel
         {
            public Employee() { }
            public Employee(DbDataReader reader)
            {
                ....
            }

            public override ISQLModel Read(DbDataReader reader) => new Employee(reader);

         }
    }
```

Assuming your database has a table called Employee structure as follow:

| Field       | Description    |
|-------------|----------------|
| EmployeeID  | PK NN AI       |
| FirstName   | Text           |
| LastName    | Text           |
| DOB         | Text           |
| GenderID    | FK INT         |
| DepartmentID| FK INT         |
| JobTitleID  | FK INT         |
| Email       | Text           |
