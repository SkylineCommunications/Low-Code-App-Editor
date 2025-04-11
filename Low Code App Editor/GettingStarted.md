# Getting Started with Skyline DataMiner DevOps

Welcome to the Skyline DataMiner DevOps environment!
This quick-start guide will help you get up and running. It was auto-generated based on the initial project setup during creation.
For more details and comprehensive instructions, please visit [DataMiner Docs](https://docs.dataminer.services/).

## Enabling the creation of a DataMiner application package

This project was not configured to generate a .dmapp file.
To enable .dmapp generation, consider migrating to a multi-artifact DataMiner Application Package as described below.

## Migrating to a multi-artifact DataMiner application package

If you need to combine additional components in your .dmapp file, you should:

1. Open the `Low Code App Editor.csproj` file and ensure the `<GenerateDataminerPackage>` property is set to `False`.

1. Right-click your solution and select *Add* > *New Project*.

1. Select the *Skyline DataMiner Package Project* template.

Follow the provided **Getting Started** guide in the new project for further instructions.

## Enabling publishing to the Catalog

This project was created without support for publishing to the DataMiner Catalog.
To enable this, add a *Skyline DataMiner Package Project* to your solution and follow the **Getting Started** guide provided in that project.

