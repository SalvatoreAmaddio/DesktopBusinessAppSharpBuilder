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

- Create a new Window File in View. Call this file as MainWindow.
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

| Fields      | Description    |
|-------------|----------------|
| EmployeeID  | PK NN AI       |
| FirstName   | Text           |
| LastName    | Text           |
| DOB         | Text           |
| GenderID    | FK INT         |
| DepartmentID| FK INT         |
| JobTitleID  | FK INT         |
| Email       | Text           |

An AbstractModel can represent the Table through a set of attributes.
Let's start by defining the backup variables first:

 ```csharp
  long _employeeid;
  string _firstName=string.Empty;
  string _lastName = string.Empty;
  DateTime? _dob;
  string _email = string.Empty;
 ```

**What about the Foreign Keys?**
Since Foreign Keys are fields representing a relationship between one Table and another, you will need to create Model classes to represent each of those. Therefore:
```csharp
  JobTitle? _jobTitle; //JobTitle is a class extending AbstractModel
  Gender? _gender; //Gender is a class extending AbstractModel
  Department? _department; //Department is a class extending AbstractModel
```

Update your constructor to fetch the data from the Database.

```csharp
    public Employee(DbDataReader reader)
    {
        _employeeid = reader.GetInt64(0);
        _firstName = reader.GetString(1);
        _lastName = reader.GetString(2);
        _dob = reader.GetDateTime(3);
        _gender = new(reader.GetInt64(4)); //notice a ForeignKey Model was defined with a constructor taking a long as argument.
        _department = new(reader.GetInt64(5)); //notice a ForeignKey Model was defined with a constructor taking a long as argument.
        _jobTitle = new(reader.GetInt64(6)); //notice a ForeignKey Model was defined with a constructor taking a long as argument.
        _email = reader.GetString(7);
    }
```

Now we can define the properties for each backup variable.

```csharp
    [PK] //This attribute tells this property represents the Primary Key of the Employee Table.
    public long EmployeeID { get => _employeeid; set => UpdateProperty(ref value, ref _employeeid); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [Field] //This attribute tells this property represents a Field of the Employee Table.
    public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [Field] //This attribute tells this property represents a Field of the Employee Table.
    public string LastName { get => _lastName; set => UpdateProperty(ref value, ref _lastName); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [Field] //This attribute tells this property represents a Field of the Employee Table.
    public DateTime? DOB { get => _dob; set => UpdateProperty(ref value, ref _dob); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [FK] //This attribute tells this property represents a Foreign Key of the Employee Table.
    public Gender? Gender { get => _gender; set => UpdateProperty(ref value, ref _gender); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [FK] //This attribute tells this property represents a Foreign Key of the Employee Table.
    public Department? Department { get => _department; set => UpdateProperty(ref value, ref _department); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [FK] //This attribute tells this property represents a Foreign Key of the Employee Table.
    public JobTitle? JobTitle { get => _jobTitle; set => UpdateProperty(ref value, ref _jobTitle); }

    [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
    [Field] //This attribute tells this property represents a Field of the Employee Table.
    public string Email { get => _email; set => UpdateProperty(ref value, ref _email); }
```

The ```UpdateProperty(ref value, ref _var); ``` method raise the INotifyPropertyChange event.

Also, you remember to add the ```[Table]``` attribute at the beginning of your class. This attribute specify the name of the table.
```csharp
     [Table(nameof(Employee))]
     public class Employee : AbstractModel
     {
        ...
     }
```

At this point, we are ready to get the data from the Database. To do so, open your **App.xaml.cs** file and add the following constructor:

```csharp
    public App() 
    {
        Sys.LoadAllEmbeddedDll(); //load some custom assemblies that could be used later on.
        DatabaseManager.Add(new SQLiteDatabase(new Employee())); //Add the database object responsible for dealing with this table.
        DatabaseManager.Add(new SQLiteDatabase(new Gender())); //Add the database object responsible for dealing with this table.
        DatabaseManager.Add(new SQLiteDatabase(new Department())); //Add the database object responsible for dealing with this table.
        DatabaseManager.Add(new SQLiteDatabase(new JobTitle())); //Add the database object responsible for dealing with this table.
        DatabaseManager.Add(new SQLiteDatabase(new User())); //Add the database object responsible for dealing with this table.
    }
```

The fetching of the data is an **Asyncronous Task** managed by the LoadingForm control. You can define your LoadingForm control by creating a new Window.xaml file in your view folder:

```xml
    ﻿<Window x:Class="MyApplication.View.LoadingForm"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:local="clr-namespace:MyApplication.View"
            xmlns:fr="clr-namespace:FrontEnd.Forms;assembly=FrontEnd"
            mc:Ignorable="d"
            ResizeMode="NoResize"
            WindowStartupLocation="CenterScreen"
            Title="Welcome" Height="450" Width="450">

        <fr:LoadingMask MainWindow="MainWindow">
            <Image Stretch="Fill" Source="your image path"/>
        </fr:LoadingMask>
    </Window>
```

In your App.xaml file, change the StartupUri property as follow:
```xml
    StartupUri="View/LoadingForm.xaml"
```

Now, when you click on Run, a Window will open loading the data in the background. Once this Task has completed, the window will close and the MainWindow will open.

## Define your first Controller:
At this point, the application interrogates the SQLite Database in the Data folder and retrieves the data. However, to display the data on a Form, we must first define a Controller object.

- Create a C# file in the Controller folder.
- Extends **AbstractFormController<M>**.

```csharp
namespace MyApplication.Controller
{
    public class EmployeeController : AbstractFormController<Employee>
    {
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!); //Fetch the data to be displayed in a ComboBox control.
        public RecordSource Departments { get; private set; } = new(DatabaseManager.Find<Department>()!); //Fetch the data to be displayed in a ComboBox control.
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!); //Fetch the data to be displayed in a ComboBox control.
        public override int DatabaseIndex => 0; // This tells the controller to use the first Database that was defined in the DatabaseManager class.
    }
}
```

Now, you must associate this controller to your View File. Open the MainWindow.xaml.cs. Change the constructor as follow:
```csharp
public MainWindow() 
{
    InitializeComponent();
    DataContext = new EmployeeController();
    ((EmployeeController)DataContext).Window = this;
}
```

Now, we are ready to structure our GUI by adding and binding controls.

```xml
﻿<Window x:Class="MyApplication.View.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:MyApplication.View"
        mc:Ignorable="d"
        xmlns:fr="clr-namespace:FrontEnd.Forms;assembly=FrontEnd"
        xmlns:controller="clr-namespace:MyApplication.Controller"
        Title="Employee Form"
        WindowStartupLocation="CenterScreen"
        SizeToContent="WidthAndHeight"
        d:DataContext="{d:DesignInstance Type=controller:EmployeeControllerList, IsDesignTimeCreatable=False}" Height="490" Width="475"
        >
    <fr:Form>
        <Grid Margin="10" fr:Definition.RowDefinitions="30,30,30,30,30,30,30,200" fr:Definition.ColumnDefinitions="80,150,30,150">
            <Label Content="First Name"/>
            <fr:Text Text="{Binding CurrentRecord.FirstName}" Grid.Row="0" Grid.Column="1"/>
            <Label Grid.Row="1" Content="Last Name"/>
            <fr:Text Text="{Binding CurrentRecord.LastName}" Grid.Row="1" Grid.Column="1"/>
            <Label Grid.Row="2" Content="DOB"/>
            <DatePicker SelectedDate="{Binding CurrentRecord.DOB}" Grid.Row="2" Grid.Column="1"/>
            <Label Content="Gender" Grid.Row="3"/>
            <fr:Combo ItemsSource="{Binding Genders}" SelectedItem="{Binding CurrentRecord.Gender}" Grid.Row="3" Grid.Column="1"/>
            <Label Content="Department" Grid.Row="4"/>
            <fr:Combo ItemsSource="{Binding Departments}" SelectedItem="{Binding CurrentRecord.Department, Mode=TwoWay}" Grid.Row="4" Grid.Column="1"/>
            <Label Content="Job Title" Grid.Row="5"/>
            <fr:Combo ItemsSource="{Binding Titles}" SelectedItem="{Binding CurrentRecord.JobTitle, Mode=TwoWay}" Grid.Row="5" Grid.Column="1"/>
            <Label Content="Email" Grid.Row="6"/>
            <fr:Text Text="{Binding CurrentRecord.Email}" Grid.Row="6" Grid.Column="1"/>
            <fr:CommandPanel Grid.Row="0" Grid.RowSpan="4" Grid.Column="3" CommandParameter="{Binding CurrentRecord}" UpdateCMD="{Binding UpdateCMD}" DeleteCMD="{Binding DeleteCMD}"/>
    </fr:Form>
</Window>
```

When you click on Run, once the Loading is completed, you will have a Window displaying a Form.
Here you can:
- Navigate from one record to anoter.
- Add new records.
- Update a record.
- Delete a record.

All of this operations are handled by the Framework through the EmployeeController class. Each change to the records are reflected onto the Database.