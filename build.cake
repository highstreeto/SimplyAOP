#tool "nuget:?package=ReportGenerator&version=4.0.9"

var target = Argument("target", "Build");
var version = Argument("version", "0.1.0");
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
        ArgumentCustomization = args => args
            .Append("/p:CollectCoverage=true")
            .Append("/p:CoverletOutputFormat=opencover")
    });
    ReportGenerator(
        "SimplyAOP.Tests/coverage.opencover.xml",
        "TestCoverage"
    );
});

Task("Package")
    .IsDependentOn("Package-Only");;
Task("Package-Only").Does(() =>
{
    DotNetCorePack("SimplyAOP", new DotNetCorePackSettings() {
        Configuration = "Release",
        ArgumentCustomization = args => args
            .Append($"/p:Version={version}")
            .Append($"/p:NuspecProperties=\\\"version={version}\\\"")
            .Append("/p:NuspecFile=SimplyAOP.nuspec")
    });
});

RunTarget(target);