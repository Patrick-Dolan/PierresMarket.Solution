# Pierre's Sweet and Savory Treats

#### By _**Patrick Dolan**_

#### _A web application for marketing Pierres sweet and savory treats._

![Demonstration Gif](https://github.com/Patrick-Dolan/PierresMarket.Solution/blob/main/DemoGifs/BasicFunctionality.gif)

## Technologies Used

* C#
* .NET 5.0
* dotnet
* MySql/Workbench

## Description

A web application for marketing Pierres sweet and savory treats.

## Planned features

* Community treats sharing.
* Public and Private treats.

## Setup/Installation Requirements

* Make sure you have MySql Workbench installed on your computer.
* Make sure to have dotnet-ef installed too.<br>
<em>This project uses <code>dotnet-ef --version 3.0.0</code> which I have globally installed but you can install it however you want. 
* Download repo to your computer using either clone or the download link.
* Open the project in VScode or your terminal/IDE of choice.
* Create a <code>appsettings.json</code> file in the root directory of the project folder. And add the following code replacing anything in square brackets with the information it represents specific to the project database:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;database=[DATABASE-NAME-HERE];uid=[USER-ID-HERE];pwd=[YOUR-PASSWORD-HERE];"
  }
}

```

Example of complete appsettings.json:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;database=to_do_list;uid=root;pwd=MySuperStrongPassword;"
  }
}

```

* Make sure to run your mysql server and open MySql workbench.
* Open MySql Workbench and login to your server.
* From your terminal navigate to the <code>PierresMarket/</code> folder and run the command <code>dotnet ef database update</code> to create the database from migrations.
* Now using your IDE navigate into the PierresMarket.Solution/PierresMarket folder and use the command dotnet run to launch the program.
* The site should be available at the server address you used in the appsettings.json folder.
* Now using your IDE in the PierresMarket.Solution/PierresMarket/ folder use the command <code>dotnet run</code> to launch the program. 
* The site should be available at the server address you used in the <code>appsettings.json</code> folder.

### Test Setup/Installation

* Open the repo on your editor of choice/terminal
* Navigate to PierresMarket.Tests directory in your terminal
* Run the following command to setup testing:  
<code>dotnet restore</code>  
* Run tests by going to the test project in the terminal (PierresMarket.Solution/PierresMarket.Tests) and running the following command:  
<code>dotnet test</code>  

* Make sure to change the Identity services configuration to make passwords more secure as the current settings are in place to make development easier but are not good settings for secure passwords

## Known Bugs

* _No known bugs._

## Contact Me

Let me know if you run into any issues or have questions, ideas or concerns:  
dolanp1992@gmail.com

## License

_MIT_

Copyright (c) _2022_ _Patrick Dolan_