XamarinPclToggle
================

Toggle Xamarin PCL Support

This tool works around an issue introduced with the latest Xamarin.iOS and Android packages.

The short version is that NuGet (as of 2.7.1) does not properly handle mapping portable
class libraries if you have an additional equivalent frameworks installed. Xamarin uses 
this functionality to map MonoTouch and MonoAndroid to the correct PCL profiles.

A symptom of this bug is the following error when you try to install or update a NuGet 
package to your PCL project:

```
Could not install package '<packagename>'. You are trying to install this package into a project that targets 'portable-net45+win+MonoAndroid10+MonoTouch10', but the package does not contain any assembly references or content files that are compatible with that framework. For more information, contact the package author.
```

If you see this error, then this tool can help you workaround it. You can get the binary from http://sdrv.ms/1isK6EQ
or build the source yourself. Click the Disable button to disable the Xamarin PCL profiles and Enable to restore them.

### You will have to restart Visual Studio for NuGet to see the changes!

Hopefully this issue will be addressed in a future NuGet release. You can track
the status of the issue here: https://nuget.codeplex.com/workitem/2926.