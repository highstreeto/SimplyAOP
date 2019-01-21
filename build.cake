#tool "nuget:?package=JetBrains.dotCover.CommandLineTools&version=2018.3.1"

var target = Argument("target", "Build");
var verbosity = Argument("verbosity", DotNetCoreVerbosity.Minimal);

Task("Build")
    .IsDependentOn("Build-Only");
Task("Build-Only").Does(() =>
{
    DotNetCoreBuild(".", new DotNetCoreBuildSettings() {
        Verbosity = verbosity
    });
});

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Test-Only");
Task("Test-Only").Does(() =>
{
    DotNetCoreTest("SimplyAOP.Tests", new DotNetCoreTestSettings() {
        ArgumentCustomization = args => args.Append("/p:CollectCoverage=true")
    });
});

RunTarget(target);