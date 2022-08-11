# Delay-Unity3D
Simple delay class to execute code after a certain amount of time in Unity

[![openupm](https://img.shields.io/npm/v/com.sandrofigo.delay-unity3d?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.sandrofigo.delay-unity3d/)

## Installation
There are multiple ways to install this package into your project:
- Add it to your project through [OpenUPM](https://openupm.com/packages/com.sandrofigo.delay-unity3d/) (recommended)
- Add the package to the Unity package manager using the HTTPS URL of this repository (recommended)
- Download the whole repository as a .zip and place the contents into a subfolder in your assets folder
- Fork the repository and add it as a submodule in git

## Usage
```csharp
using Timing;
using UnityEngine;

public class Foo : MonoBehaviour
{
    public void Bar()
    {
        Delay.Create(1f, () => Debug.Log("I will be executed after 1 second."));
        
        Delay.WaitUntil(() => Time.time >= 5, () => Debug.Log("I will wait until the provided condition is true."));
    }
}
```

## Collaboration
Support this project with a ⭐️, report an issue or if you feel adventurous and would like to extend the functionality open a pull request.
