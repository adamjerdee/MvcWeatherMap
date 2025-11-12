# MvcWeatherMap

MvcWeatherMap is an ASP.NET Core MVC web application that displays an interactive Google Map.  
When you click on any point, it retrieves and displays a 7-day forecast from the **U.S. National Weather Service (NWS)**.  
The application also includes an **Admin page** where you can securely store and manage your **Google Maps API key**, which is saved encrypted in SQL Server.

<p align="center">
  <img src="./MVC%20Google%Weathermap.png" alt="Screenshot of MVC Google Weathermap" width="700">
</p>
---

## 🌐 Features

- Interactive **Google Map** using the Maps JavaScript API  
- 7-Day Forecast from the **National Weather Service API** (https://api.weather.gov)  
- API key management page (`/Admin/ApiKey`) that stores keys encrypted in a SQL Server table (`dbo.AppSecrets`)  
- Clean, Bootstrap-based UI  
- Data access using **Dapper** (no Entity Framework dependency)  
- Compatible with .NET 8.0 or newer

---

## ⚙️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)
- Google Cloud Platform account
- Visual Studio 2022 or VS Code

---

## 🗝️ Get a Google Maps JavaScript API Key

1. Go to the [Google Cloud Console](https://console.cloud.google.com/).
2. Create or select a project.
3. Enable the **Maps JavaScript API**.
4. Go to **APIs & Services → Credentials → Create Credentials → API Key**.
5. Copy your key.
6. (Optional but recommended) Add HTTP referrer restrictions such as:
   - `https://localhost:*/*`
   - `http://localhost:*/*`
7. Ensure **billing is enabled** on the project (Google requires this for Maps API use).

---

## 🧩 Database Setup

1. Create a new SQL Server database (for example, `MvcWeatherMap`).
2. Run the T-SQL script included in the repository:
   - File: `SQLDBScript.txt` (or `SQLDBScript.sql`)
3. The script will create:
   - `dbo.AppSecrets` table  
   - Stored procedures:  
     - `dbo.AppSecrets_Set`  
     - `dbo.AppSecrets_Get`

This database is used to securely store your encrypted API key.

---

## 🔧 Configuration

Edit the **`appsettings.json`** file and set your connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MvcWeatherMap;Trusted_Connection=True;TrustServerCertificate=True"
}

Running the Application

Open the solution in Visual Studio or run it from the command line using:
dotnet run

Once the application starts, navigate to the URL shown in the output window, for example:
https://localhost:5001/

The Home page will display a Google Map once your API key has been configured.

Setting the Google Maps API Key

Open your web browser and go to the Admin page by navigating to:
/Admin/ApiKey
(for example: https://localhost:5001/Admin/ApiKey
)

Enter your Google Maps JavaScript API key into the provided text box and click the "Save Key" button.

The key will be encrypted and stored securely in the SQL Server database in the table named dbo.AppSecrets.

After saving, return to the Home page. The map should now load successfully using your stored key.

Using the Application

On the Home page, you will see a Google Map centered on Des Moines, Iowa by default.

Click anywhere on the map to fetch a 7-day weather forecast from the National Weather Service (NWS).

The forecast will appear in a grid showing 7 days horizontally, with daytime and nighttime rows.

Each forecast cell includes the temperature, short forecast text, and weather icon for that period.

The title above the forecast will display the city and state corresponding to the point you clicked on the map.

Technical Notes

Encryption uses ASP.NET Core Data Protection to ensure that only your machine can decrypt the stored API key.

The application uses Dapper for database access, with stored procedures named AppSecrets_Set and AppSecrets_Get.

The weather data is retrieved from the National Weather Service API endpoint:
https://api.weather.gov/points/{latitude},{longitude}

This endpoint provides a forecast URL specific to the selected coordinates.

MVC project structure includes:
Controllers: HomeController.cs, AdminController.cs, WeatherController.cs
Views: Home/Index.cshtml, Admin/ApiKey.cshtml, Shared/_Layout.cshtml
wwwroot/js: maps.js
Data: SqlConnectionFactory.cs, SecretRepository.cs
Services: SecretProtector.cs, WeatherService.cs

The Google Maps API key is automatically loaded from the database and decrypted inside the _Layout.cshtml file, so the correct script URL is generated at runtime.

The application does not use Entity Framework; all data access is handled using Dapper and stored procedures for improved performance and simplicity.

The database schema includes one primary table (dbo.AppSecrets) and two stored procedures (AppSecrets_Set and AppSecrets_Get) for saving and retrieving the encrypted API key.

The forecast view and map logic are handled client-side with the JavaScript file maps.js located in the wwwroot/js folder.


License and Credits
Weather data: National Weather Service (public domain)
Map data: Google Maps Platform
Framework: ASP.NET Core MVC (.NET 8)
This project is open-source for educational and demonstration purposes.
