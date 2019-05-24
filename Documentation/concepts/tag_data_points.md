---
title: Tag Data Points
description: What are Tag Data Points
keywords: TimeSeries
author: einari
weight: 3
---
A `TagDataPoint` is the type that is expected to be the type coming out of the first module
at the beginning of a [time series pipeline]({{ relref time_series_pipeline }}).
In industries, there tend to be a control system that is responsible for gathering
the different data points into one place and it is this system that is then responsible for
consolidating formats and output in one consistent format. A tag data point will therefor
have a control system reference on it. To identify the source of the sampled value, it is
commonly known as a tag. This tag identifier is specific to the control system.
This identifier is something the Dolittle time series software is in general not using,
and there is therefor an [identity mapper](/timeseries/identitymapper) that
deals with translating it into what is identified as a [time series]({{< relref time_series >}}).

Typically, if consuming this type, you can expect the following payload as JSON:

```json
{
    "controlSystem": "string",
    "tag": "tag",
    "value": number | string | any,
    "timestamp": "EPOCH in milliseconds"
}
```

{{% notice information %}}
Important to notice that the timestamp is in standard UNIX EPOCH time with
milliseconds granularity.
{{% /notice %}}
