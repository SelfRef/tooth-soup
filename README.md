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
  - export appointments of invoice
- Dentist can
  - link/unlink patients
  - view own appointments
  - create appointments for linked patients
  - edit or cancel/uncancel appointments
  - create and edit patients
  - create/link/remove services (conditionally)
  - export appointments of invoice
- Admin can
  - fully manage users
  - fully manager services

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