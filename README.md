# tooth-soup

## Overview

A system supporting the work of a dentist.

## Features

- Patient can
  - set main dentist in settings
  - change email or password
  - view own appointments
  - create appointments for main dentist
  - create appointments for other dentists if dentist allowed for this
  - edit or cancel appointment
  - export appointments of invoice to PDF or XML
- Dentist can
  - link/unlink patients
  - view own appointments
  - create appointments for linked patients
  - edit or cancel/uncancel appointments
  - create and edit patients
  - create/link/remove services (conditionally)
  - export appointments of invoice to PDF or XML
- Admin can
  - fully manage users
  - fully manager services

## Development & running

1. Install [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Install [Node.js](https://nodejs.org)
3. Install [Yarn](https://yarnpkg.com/getting-started/install)
4. Open terminal windows in `backend` directory
5. Run backend by command `dotnet run -p ToothSoupAPI.csproj`
6. Open new terminal windows in `frontend` directory
7. Install dependencies by command `yarn`
8. After installation run command `yarn run dev`
9. Open listening URL (by default [http://localhost:3000/](http://localhost:3000/))

## Mock accounts

Role | Email | Password
--- | --- | ---
Patient | patient1@qwe.pl | qwe
Patient | patient2@qwe.pl | qwe
Patient | patient3@qwe.pl | qwe
Dentist | dentist1@qwe.pl | qwe
Dentist | dentist2@qwe.pl | qwe
Dentist | dentist3@qwe.pl | qwe
Admin | admin1@qwe.pl | qwe

## Roles & privileges

Action | Guest | Patient | Dentist | Admin
--- | :---: | :---: | :---: | :---:
Login as anyone | ✔ | ❌ | ❌ | ❌
Register as patient | ✔ | ❌ | ❌ | ❌
Home page | ✔ | ✔ | ✔ | ✔
--- | --- | --- | --- | ---
My account page | ❌ | ✔ | ✔ | ❌
Edit my email | - | ✔ | ✔ | -
Edit my password | - | ✔ | ✔ | -
Edit linked dentist | - | ✔ | ❌ | -
Edit linking agreement | - | ❌ | ✔ | -
Edit unlinked appointments | - | ❌ | ✔ | -
--- | --- | --- | --- | ---
Patient list page | ❌ | ❌ | ✔ | ❌
List my patients | - | - | ✔ | -
Create patient | - | - | ✔ | -
Link patient | - | - | ✔ | -
Unlink patient | - | - | ✔ | -
Edit linked patient | - | - | ✔ | -
Remove patient | - | - | ❌ | -
--- | --- | --- | --- | ---
Appointment list page | ❌ | ✔ | ✔ | ❌
List my appointments | - | ✔ | ✔ | -
Create appointment | - | ✔ | ✔ | -
Cancel appointment | - | ✔ | ✔ | -
Uncancel appointment | - | ❌ | ✔ | -
Edit active appointment | - | ✔ | ✔ | -
Edit canceled appointment | - | ❌ | ✔ | -
Edit past appointment | - | ❌ | ✔ | -
Remove appointment | - | ❌ | ✔ | -
Export appointments in PDF | - | ✔ | ✔ | -
Export appointments in XML | - | ✔ | ✔ | -
Export invoice in PDF | - | ✔ | ✔ | -
--- | --- | --- | --- | ---
User list page | ❌ | ❌ | ❌ | ✔
Create any user | - | - | - | ✔
Edit any user | - | - | - | ✔
Remove any user | - | - | - | ✔
--- | --- | --- | --- | ---
Service list page | ❌ | ❌ | ✔ | ✔
Create service | - | - | ✔ | ✔
Link service | - | - | ✔ | ❌
Unlink service | - | - | ✔ | ❌
Edit only my service | - | - | ✔ | ❌
Edit any service | - | - | ❌ | ✔
Remove only my service | - | - | ✔ | ❌
Remove any service | - | - | ❌ | ✔

## Database

By default project uses `InMemory` database with provided seed that populates all the tables with example values. Database configuration can be defined in `Startup.cs` file. Seed is defined in `Seed/DatabaseSeed.cs` and enabled in `Database.cs` with `SeedData.Seed()` method.

## Screenshots

- | -
--- | ---
![](assets/home_anonymous.png) | ![](assets/home_logged.png)
![](assets/login.png) | ![](assets/register.png)
![](assets/patients_all.png) | ![](assets/patients_appointment.png)
![](assets/patients_create.png) | ![](assets/patients_link.png)
![](assets/services.png) | ![](assets/services_create.png)
![](assets/account_dentist.png) | ![](assets/account_patient.png)
![](assets/users.png) | ![](assets/users_create_patient.png)
![](assets/users_create_dentist.png) | ![](assets/users_create_admin.png)
![](assets/appointments.png) | ![](assets/invoice_pdf.png)
![](assets/export_pdf.png) | ![](assets/export_xml.png)