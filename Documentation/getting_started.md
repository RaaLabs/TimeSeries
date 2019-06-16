---
title: Getting started
description: Get started with implementing a Time Series module
keywords: TimeSeries, Modules, Getting Started
author: einari
weight: 1
---
## Introduction

This tutorial is for C# and .NET only, as that is the only environment supported right now.

## Pre-requisites

* .NET 2.2 or better - download [here](https://dotnet.microsoft.com/download)
* Microsoft IoT Edge development environment - read more [here](https://github.com/Azure/iotedgedev/wiki/manual-dev-machine-setup)

## Setting up project

Lets start by creating a folder for your project, call it "MyFirstConnectorModule".
Inside this folder we then want to create a new .NET Core project, from the CLI you can do the following:

```shell
$ dotnet new console
```

Once created, we need to change the generated `.csproj`. Add the project capability for IoT Edge:

```xml
<ItemGroup>
    <ProjectCapability Include="AzureIoTEdgeModule" />
</ItemGroup>
```

Then we're going to need to add NuGet package references to [Dolittle.TimeSeries.Modules](https://www.nuget.org/packages/Dolittle.TimeSeries.Modules/)
and [Dolittle.TimeSeries.Modules.IoTEdge](https://www.nuget.org/packages/Dolittle.TimeSeries.Modules.IoTEdge/).
You can do this using the `dotnet` CLI tool or your IDEs package manager, or editing the `.csproj` file
directly and add the following:

```xml
<ItemGroup>
    <PackageReference Include="Dolittle.TimeSeries.Modules" Version="5.*" />
    <PackageReference Include="Dolittle.TimeSeries.Modules.IoTEdge" Version="5.*" />
</ItemGroup>
```

## Booting

Part of building a module is that it needs to boot the core Dolittle fundamentals and
get the module environment set up. Open the `Program.cs` file and add the correct
using statement and start the bootloader:

```c#
using Dolittle.TimeSeries.Modules.Booting;

namespace MyFirstConnectorModule
{
    class Program
    {
        static void Main()
        {
            Bootloader.Configure(_ => {}).Start().Wait();
        }
    }
}
```

This is enough to get going and be ready to create a connector or message handlers.

If you want to run this locally without packaging it up as a Docker container first and enable debugging,
please follow the instructions [here](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-vs-code-develop-module#debug-a-module-without-a-container-c-nodejs-java).

{{% notice information %}}
Due to the certificate usage of Microsoft IoT Edge, you might run into issues running it barebone without a
container. These are known issues and you might just be better off going for the container.
{{% /notice %}}

## Connector

Let's add a simple connector that will be discovered and hooked up automatically. We're not going to do anything
in it, but its the place you'd typically go and add your logic for connecting to a source and get the data from.
We're going to do a pull based connector, read more about connectors [here]({{< relref "developing/connectors" >}}).

Add a class called `Connector` to your project and put the following code into it:

```c#
using System;
using Dolittle.TimeSeries.Modules;
using Dolittle.TimeSeries.Modules.Connectors;

namespace MyFirstConnectorModule
{
    public class Connector : IAmAPullConnector
    {
        public Source Name => "My Connector";

        public IEnumerable<TagWithData> GetAllData()
        {
            Console.WriteLine("Getting all the data");

            return new TagWithData[0];
        }

        public object GetData(Tag tag)
        {
            return 0;
        }
    }
}
```

## Docker

Running modules in a production environment requires packaging your module as a Docker container image.
Below is a Dockerfile that also supports debugging when running things locally.
Add a file called `Dockerfile` to your project and copy/paste the following into it:

```dockerfile
FROM microsoft/dotnet:2.1-runtime-stretch-slim as base

ARG CONFIGURATION=Release

RUN echo Configuration = $CONFIGURATION

RUN if [ "$CONFIGURATION" = "Debug" ] ; then apt-get update && \
    apt-get install -y --no-install-recommends unzip procps && \
    rm -rf /var/lib/apt/lists/* \
    ; fi

RUN useradd -ms /bin/bash moduleuser
USER moduleuser


RUN if [ "$CONFIGURATION" = "debug" ] ; then curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg ; fi

FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

ARG CONFIGURATION

COPY *.csproj ./Source/

WORKDIR /app/Source/

RUN dotnet restore

COPY . ./
RUN dotnet publish -c $CONFIGURATION -o out

FROM base

WORKDIR /app
COPY --from=build-env /app/Source/out ./

ENTRYPOINT ["dotnet", "[Assembly to run].dll"]
```

To build the container image, you simply do:

```shell
$ docker build -t myfirstconnectormodule .
```

If you want to enable debugging, you run it with a build arg saying to build the debug configuration:

```shell
$ docker build -t myfirstconnectormodule . --build-arg CONFIGURATION="Debug"
```

## Configuring IoT Hub

## Configuring Module

```json
{
    "pullConnectors": {
        "My Connector": {
            "interval": 1000
        }
    }
}
```

## Running
