XamarinPclToggle
================

Toggle Xamarin PCL Support

This tool works around an issue introduced with the latest Xamarin.iOS and Android packages.

The short version is that NuGet (as of 2.7.1) does not properly handle mapping portable
class libraries if you have an additional equivalent frameworks installed. Xamarin uses 
this functionality to map MonoTouch and MonoAndroid to the correct PCL profiles.

NuGet 2.7.2 will have a fix for this once its released. Be sure to re-enable the Xamarin
PCL profiles once you upgrade NuGet.

A symptom of this bug is the following error when you try to install or update a NuGet 
package to your PCL project:

```
Could not install package '<packagename>'. You are trying to install this package into a project that targets 'portable-net45+win+MonoAndroid10+MonoTouch10', but the package does not contain any assembly references or content files that are compatible with that framework. For more information, contact the package author.
```

If you see this error, then this tool can help you workaround it. 

##Get the binary 
http://sdrv.ms/1isK6EQ

As the tool requires Admin rights, you can build the source yourself if you prefer. 

## Usage
Click the Disable button to disable the Xamarin PCL profiles and Enable to restore them.

### You will have to restart Visual Studio for NuGet to see the changes!

This issue will be addressed in NuGet 2.7.2, though there's no current ETA. I expect
it to be before the end of 2013.

You can track the status of the issue here: https://nuget.codeplex.com/workitem/2926.