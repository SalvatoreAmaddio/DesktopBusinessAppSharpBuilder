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

