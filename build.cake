#tool "nuget:?package=JetBrains.dotCover.CommandLineTools&version=2018.3.1"

var target = Argument("target", "Build");
var verbosity = Argument("verbosity", Verbosity.Minimal);

Task("Build")
    .IsDependentOn("Build-Only");
Task("Build-Only").Does(() =>
{
    MSBuild("./SimplyAOP.sln", new MSBuildSettings()
        .SetVerbosity(verbosity)
    );
});

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Test-Only");
Task("Test-Only").Does(() =>
{
    DotCoverCover(
        tool => tool. DotNetCoreTest("./SimplyAOP.Tests"),
        new FilePath("./reports/test_coverage.dcvr"),
        new DotCoverCoverSettings()
            .WithFilter("+:SimplyAOP")
    );
    DotCoverReport(new FilePath("./reports/test_coverage.dcvr"),
        new FilePath("./reports/Test.html"),
        new DotCoverReportSettings {
            ReportType = DotCoverReportType.HTML
        }
    );
});

RunTarget(target);