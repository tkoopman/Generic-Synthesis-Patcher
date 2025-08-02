using System.Collections;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.CompilerServices;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

using Noggog;
using Noggog.IO;

using NSubstitute;
using NSubstitute.Core;

namespace Synthesis.Bethesda.UnitTests.Common;

public static class Utility
{
    public const string BaseFolder = "C:/BaseFolder";
    public static readonly string BuildFailureFile = "Files/BuildFailure.txt";
    public static readonly string BuildSuccessFile = "Files/BuildSuccess.txt";
    public static readonly string BuildSuccessNonEnglishFile = "Files/BuildSuccessNonEnglish.txt";
    public static readonly string LePath = "Files/Skyrim/";
    public static readonly string OblivionPath = "Files/Oblivion/";
    public static readonly string Other2FileName = "other2.esp";
    public static readonly string OtherFileName = "other.esp";
    public static readonly string OverallTempFolderPath = "SynthesisUnitTests";
    public static readonly string OverrideFileName = "override.esp";
    public static readonly ModKey OverrideModKey = new("override", ModType.Plugin);
    public static readonly string PluginFileName = "Plugins.txt";
    public static readonly ModKey RandomModKey = new("Random", ModType.Plugin);
    public static readonly ModKey SynthesisModKey = new("Synthesis", ModType.Plugin);
    public static readonly string TestFileName = "test.esp";
    public static readonly ModKey TestModKey = new("test", ModType.Plugin);

    public enum Return
    { True, False, Throw }

    public static TempFolder GetTempFolder (string folderName, [CallerMemberName] string? testName = null)
    {
        return TempFolder.FactoryByAddedPath(Path.Combine(Utility.OverallTempFolderPath, folderName, testName!), throwIfUnsuccessfulDisposal: false);
    }

    public static ConfiguredCall Returns (
        this bool value,
        Return ret)
    {
        return value.Returns(_ =>
        {
            switch (ret)
            {
                case Return.False:
                    return false;

                case Return.True:
                    return true;

                default:
                    throw new Exception();
            }
        });
    }

    public static TestEnvironment SetupEnvironment (GameRelease release)
    {
        var baseFolder = new DirectoryPath(BaseFolder);
        var dataFolder = new DirectoryPath($"{BaseFolder}/DataFolder");
        string filesPath;
        switch (release)
        {
            case GameRelease.Oblivion:
                filesPath = OblivionPath;
                break;

            case GameRelease.SkyrimLE:
            case GameRelease.SkyrimSE:
                filesPath = LePath;
                break;

            default:
                throw new NotImplementedException();
        }

        string pluginPath = Path.Combine(BaseFolder, PluginFileName);

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { Path.Combine(dataFolder.Path, TestFileName), new MockFileData(File.ReadAllBytes(Path.Combine(filesPath, TestFileName))) },
            { Path.Combine(dataFolder.Path, OverrideFileName), new MockFileData(File.ReadAllBytes(Path.Combine(filesPath, OverrideFileName))) },
            { pluginPath, new MockFileData(File.ReadAllBytes(Path.Combine(filesPath, PluginFileName))) },
        });

        return new TestEnvironment(
            fileSystem,
            Release: release,
            DataFolder: dataFolder,
            BaseFolder: baseFolder,
            PluginPath: pluginPath);
    }

    public static ModPath TypicalOutputFile (DirectoryPath tempFolder) => Path.Combine(tempFolder.Path, SynthesisModKey.FileName);
}

public class ReturnData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator ()
    {
        yield return new object[] { Utility.Return.True };
        yield return new object[] { Utility.Return.False };
        yield return new object[] { Utility.Return.Throw };
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return GetEnumerator();
    }
}