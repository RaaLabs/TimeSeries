#!/bin/bash
export PACKAGEDIR=$PWD/Packages
export VERSION=$(git tag --sort=-version:refname | head -1)
dotnet pack -p:PackageVersion=$VERSION --include-symbols --include-source -o $PACKAGEDIR
for f in $PACKAGEDIR/*.symbols.nupkg; do
 nuget push $f -Source https://api.nuget.org/v3/index.json 
done