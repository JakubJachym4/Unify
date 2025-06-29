# Unify - University Teaching Support System.

Project made for my Computer Science Engineering Thesis.

System desinged to represent real-life university structure and it's members.

# Key Features.

- 3 User roles - Admin, Lecturer, Student.
- Ability to recreate unversity strucutre via - Faculties, Courses, Classes, Locations etc.
- Planning lectures and classes.
- Creating and grading assignments
- Schedule.
- Course grading.
- Managing course / classes resources.
- Sending messages.


>![Lecturer Dashboard View](https://github.com/user-attachments/assets/c7af6712-c3c0-434a-a5e4-9802531b7c91)
>*Lecturer Dashboard View*

# Technical design.

System is separated into two parts, backend application and frontend web-applicaiton.

## Backend.

Created with .NET 8, design follows Clean Archtecture and Domain Driven Desing.
Whole backend applicaiton is contenerized in Docker containers.

Data is stored in PostgreSQL database and system uses Entity Framework as ORM.
Applicaiton implements CQRS pattern but without using faster ORM or second database (Dapper was planned to be used for quering but idea was abandoned due to limited time).
System uses REST API to communicate with web-app.

## Frontend

Svelte-Kit with TypeScript was chosen as framework for this web-app, mostly for learing purposes and as test of a new technology.
Bootstrap was used as styling.

Thanks to Svelte-Kit other frameworks/tools weren't needed for this simple project.
Most Views/Forms etc were done with copliot for quicker development.

Not contenerized yet, uses server-side rendering, but Svelte-Kit allows easy switch to browser-rendering if needed.
