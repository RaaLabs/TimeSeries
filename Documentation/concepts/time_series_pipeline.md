---
title: Time Series Pipeline
description: What is a time series pipeline
keywords: TimeSeries
author: einari
weight: 2
---
With discreet events like time series data points are, there are a set
of tasks you want to perform with the events before they can be stored.
The purpose of the pipeline is to enable these different tasks to work
independently from one another and enable a way to compose together
the desired pipeline for specific outcomes. Rather than doing this in
a very closed way within code and run the risk of violating the
[open/closed principle](https://dolittle.io/contributing/guidelines/development_principles/),
the pipeline enables a completely decoupled approach to it and provides
a way to compose things together.

The composition part is what the [Microsoft IoT Edge](https://azure.microsoft.com/en-us/services/iot-edge/)
platform is bringing to the table and we're leveraging this as is.
This means you can take any module written as a Dolittle TimeSeries
and deploy it directly through the [Microsoft IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/).
