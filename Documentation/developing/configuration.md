---
title: Configuration
description: How to keep configuration
keywords: TimeSeries, Modules, Configuration
author: einari
weight: 3
---
Modules that need configuration that is unique per deployment of the module
can define a strongly typed configuration object and mark it as a configuration
object. This will by convention then automatically be handled by an
configuration object provider, for TimeSeries modules there is an implementation
of a provider that deals with this using the desired properties in
[module twins](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-module-twins) of
the Microsoft's IoT Edge system.

```csharp
using Dolittle.Configuration;

public class MyConfiguration : IConfigurationObject
{

}
```

It is important to remember that configuration objects are considered immutable.
This means you can't have public properties with setters on them. For properties
to be set, you will have to provide a constructor that sets these and let the
parameters on the constructor match by convention the names of the properties.

{{% notice information %}}
Properties are typically PascalCased, while constructor parameters must be camelCased.
{{% /notice %}}

```csharp
using Dolittle.Configuration;

public class MyConfiguration : IConfigurationObject
{
    public MyConfiguration(string myProperty, string myOtherProperty)
    {
        MyProperty = myProperty;
        MyOtherProperty = myOtherProperty;
    }

    public string MyProperty { get; }
    public int MyOtherProperty { get; }
}
```

{{% notice information %}}
If you don't have any default constructor with parameters that fulfills the public
properties but only have properties, an exception will be thrown.
{{% /notice %}}

Once you have a configuration object defined and it has been configured in the deployments
desired properties. All you need to do in code is to take a dependency to it in your
constructor and it will automatically deserialized from the properties.

```csharp
public class MyConnector
{
    public MyConnector(MyConfiguration configuration)
    {
        /* Use the configuration */
    }
}
```
