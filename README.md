# tooth-soup

## Overview

A system supporting the work of a dentist.

## Features

- Patient
  - can set main dentist in settings
  - can create appointments for main dentist
  - can create appointments for other dentists if dentist allowed for this
  - can edit or cancel appointment

## Roles & privileges

Action | Guest | Patient | Dentist | Admin
--- | :---: | :---: | :---: | :---:
Login | ✔ | ❌ | ❌ | ❌
Register | ✔ | ❌ | ❌ | ❌
Home page | ✔ | ✔ | ✔ | ✔
--- | --- | --- | --- | ---
My account page | ❌ | ✔ | ✔ | ❌
Edit my email | ❌ | ✔ | ✔ | ❌
Edit my password | ❌ | ✔ | ✔ | ❌
Edit linked dentist | ❌ | ✔ | ❌ | ❌
Edit linking agreement | ❌ | ❌ | ✔ | ❌
Edit unlinked appointments | ❌ | ❌ | ✔ | ❌
--- | --- | --- | --- | ---
Patients page | ❌ | ❌ | ✔ | ❌
List my patients | - | - | ✔ | -
Create patient | - | - | ✔ | -
Link patient | - | - | ✔ | -
Unlink patient | - | - | ✔ | -
Edit linked patient | - | - | ✔ | -
Remove patient | - | - | ❌ | -
--- | --- | --- | --- | ---
Appointments page | ❌ | ✔ | ✔ | ❌
List my appointments | - | ✔ | ✔ | -
Create appointment | - | ✔ | ✔ | -
Cancel appointment | - | ✔ | ✔ | -
Uncancel appointment | - | ❌ | ✔ | -
Edit active appointment | - | ✔ | ✔ | -
Edit canceled appointment | - | ❌ | ✔ | -
Edit past appointment | - | ❌ | ✔ | -
Remove appointment | - | ❌ | ✔ | -
--- | --- | --- | --- | ---
Users page | ❌ | ❌ | ❌ | ✔
Create any user | - | - | - | ✔
Edit any user | - | - | - | ✔
Remove any user | - | - | - | ✔
--- | --- | --- | --- | ---
Services page | ❌ | ❌ | ✔ | ✔
Create service | - | - | ✔ | ✔
Link service | - | - | ✔ | ❌
Unlink service | - | - | ✔ | ❌
Edit only my service | - | - | ✔ | ❌
Edit any service | - | - | ❌ | ✔
Remove only my service | - | - | ✔ | ❌
Remove any service | - | - | ❌ | ✔