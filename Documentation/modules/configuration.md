---
title: Configuration
description: How to keep configuration
keywords: TimeSeries, Modules, Configuration
author: einari
weight: 3
---
Modules that need configuration that is unique per deployment of the module
can define a strongly typed configuration object and mark it as a configuration
object. The underlying configuration system will then try to provide the module
with an instance of the configuration object by asking providers if they can
provide it.

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

The name recognized by the system and exposed to the providers will the name of the type,
so in the example above this will be `MyConfiguration`. This means that for instance if
this is a file, the system will look for a file called this and the supported extensions.

If you want it to have a different name, you can add the a `[Name("Connector")]` attribute
and specify the name. Which would lead to the following:

```csharp
using Dolittle.Configuration;

[Name("Connector")]
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

## Microsoft IoT Edge

With Microsoft IoT Edge, it is possible to configure something called [module twin](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-module-twins).
Within a module twin there is the concept of desired properties.
Out of the box with the [IoT Edge support](https://www.nuget.org/packages/Dolittle.TimeSeries.Modules.IoTEdge/)
there is a provider for working with the desired properties of a module twin by convention.
The provider for desired properties has a convention that is looking for a property with the name of the configuration object
in **camelCase**. The serializer also expects values inside the JSON object to be **camelCase**:

```json
{
    "properties": {
        "desired": {
            "connector": {
                "myProperty": "Some value",
                "myOtherProperty": 42
            }
        }
    }
}
```
