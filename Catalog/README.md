# Low Code App Editor

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkylineCommunications_Low-Code-App-Extensions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkylineCommunications_Low-Code-App-Extensions)

The **Low Code App Editor** simplifies managing low-code applications within the DataMiner System. It allows you to import, export, edit, and delete apps while offering customization options for metadata, resources, and deployment settings. With features like tailored export configurations and cross-Agent synchronization, the tool streamlines app life cycle tasks, making it a practical solution for maintaining and deploying low-code apps efficiently.

![Overview](Images/LCAEditor_1_0_0_13.gif)

## Getting Started

#### Step 1: Deploy the Low Code App Editor package

1. Click the **Deploy** button to deploy the package directly to your DataMiner System.
1. Optionally, go to [admin.dataminer.services](https://admin.dataminer.services/) and verify whether the deployment was successfull.

#### Step 2: Launch the script

1. Open DataMiner Cube and navigate to the Automation Module.
1. Search for the **Low Code App Editor** automation script
1. Press the execute button

## Use Cases

### Exporting Apps

You can export apps using this feature, which gathers all the necessary files for the selected apps and creates a .dmapp package. This package can then be installed on another system. The exported package is saved to the following directory: C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\Low Code Apps Exports.

For more details on the exporting functionality, refer to the [Low Code App Editor readme](https://github.com/SkylineCommunications/Low-Code-App-Editor/blob/main/README.md).

> [!NOTE]
> To avoid compatibility issues, when exporting an app from one DMA to another, ensure that the versions match (for example GQI versions).

![Editor](Images/ExportDialog.png)

## Editing Apps

You can edit various aspects of your low-code app's general information in this section. Additionally, you have the option to import pages and panels from other apps within the system to enhance functionality. The features you can manage here include the app's name, description, and sections. You can also define who can edit the app by selecting editors and specify access permissions by assigning viewers. Furthermore, you can expand the app's capabilities by integrating pages and panels from other apps.

> [!CAUTION]
> When you add users to the editors/viewers list, there is no check if the users that you add actually exist.

![Editor](Images/EditorDialog_1_0_0_13.gif)

## Support

For additional help, reach out to [arne.maes@skyline.be](mailto:arne.maes@skyline.be)
