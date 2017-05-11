# WSP-Repackage

This tool allows you to easily repackage and existing SharePoint WSP solution using several settings.
* Set a new solution ID
* Remove all DLLs from the package
* Overwrite the package or save as a new package
* Configure the output file name

## Use cases

Why would you need this tool?

### Shared assemblies issue:
Consider you have a SharePoint 2010 package, and you just built a new package for SharePoint 2013.
If you want to support existing users who are upgrading their site, you will need them to deploy the 2013 solution package as well as all the resources and features from the 2010 package.
Since in most cases the DLL evidence will be the same - once you retract the 2010 package it will remove these shared DLLs from your GAC and the 2013 package will stop working.
So, what you need is a 2010 solution package with all the features and files, except for the DLLs.

### Same solution ID issue:
Consider you have a SharePoint 2010 package, and you just built a new package for SharePoint 2013.
If you want to support existing users who are upgrading their site, you will need them to deploy the 2013 solution package as well as all the resources and features from the 2010 package.
If you used the same solution ID in your 2010 and 2013 packages, you will not be able to deploy both of them at the same time.

Using this tool you can create a simple 2010 BC (backward compatibility) package that can be safely deployed on your 2013 site for as long as you need to support 2010 sites. Once all sites are upgraded, you can safely remove this package from your farm.

---
This project is brought to you by KWizCom corporation, with full source code.


Please visit us http://www.kwizcom.com


Migrated to GitHub May 11 2017
