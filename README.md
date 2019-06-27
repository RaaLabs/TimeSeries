# TimeSeries


[![Build Status](https://dev.azure.com/dolittle/Dolittle%20open-source%20repositories/_apis/build/status/dolittle-timeseries.Modules?branchName=master)](https://dev.azure.com/dolittle/Dolittle%20open-source%20repositories/_build/latest?definitionId=17&branchName=master)

## Cloning

This repository has sub modules, clone it with:

```shell
$ git clone --recursive <repository url>
```

If you've already cloned it, you can get the submodules by doing the following:

```shell
$ git submodule update --init --recursive
```

## Building

All the build things are from a submodule.
To build, run one of the following:

Windows:

```shell
$ Build\build.cmd
```

Linux / macOS

```shell
$ Build\build.sh
```
