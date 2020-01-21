using System.Diagnostics;
using Xunit;

namespace CMPlus.Tests
{
  // return otherTask?.SSMNumber == jobPlanMetrics.SSMNumber
  //        && otherTask?.LastActionEndTimestamp
  //        <jobPlanMetrics.LastActionEndTimestamp;

  // return
  //         $@"TurnAroundTimeMilliseconds,{
  //                 TurnAroundTimeMilliseconds?.TotalMilliseconds
  //             },OnInstrumentTimeMilliseconds,{
  //                 OnInstrumentTimeMilliseconds?.TotalMilliseconds
  //             },TaskProcessingTimeMilliseconds,{
  //                 TaskProcessingTimeMilliseconds?.TotalMilliseconds
  //             },AverageSinceSSMCleanDuration,{
  //                 AverageSinceSSMCleanDuration?.TotalMilliseconds
  //             },AverageSinceSSMEmptyDuration,{
  //                 AverageSinceSSMEmptyDuration?.TotalMilliseconds
  //             },MaximumSinceSSMCleanDuration,{
  //                 MaximumSinceSSMCleanDuration?.TotalMilliseconds
  //             },MaximumSinceSSMEmptyDuration,{MaximumSinceSSMEmptyDuration?.TotalMilliseconds}";

  /*
   return ResourceLinks.Where(c => c.Value.Name == instance.ItemType.Name)
                       .Select(link => GetResourceType(link.Key)
                                           .NumberedInstance(link.Value.GetCorrectTarget(instance.Index)))
                       .ToList();
   *
   throw new InvalidOperationException(
              $@"Some scenarios have duplicate names: {
                      string.Join(", ",
                          allScenarios.GroupBy(a => a.Name)
                              .Where(a => a.Count() > 1)
                              .Select(a => a.Key))
                  }");

   */

  public class IndentAligner : TestBase
  {
    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Align_BracesBock(string singleIndent)
    {
      var code = @"
    var map = new Dictionary<int, int>
    {
         { 1, 2 },
          { 3, 4 }
     }".GetSyntaxRoot();
      var processedCode = code.AlignIndents(singleIndent)
                              .ToString()
                              .GetLines();

      // todo - try to change to configured
      //var dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE2;
      //var singleIndent1 = dte.Properties["TextEditor", "CSharp"];

      Assert.Equal(singleIndent + "{", processedCode[1]);
      Assert.Equal(singleIndent + singleIndent + "{ 1, 2 },", processedCode[2]);
      Assert.Equal(singleIndent + singleIndent + "{ 3, 4 }", processedCode[3]);
      Assert.Equal(singleIndent + "}", processedCode[4]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Align_Braces(string singleIndent)
    {
      var code =
@"var map = new Dictionary<int, int>
{
   { 1, 2 },
    { 3, 4 }
}".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent,(i, x) => Debug.WriteLine(x))
                              .ToString()
                              .GetLines();

      Assert.Equal("{", processedCode[1]);
      Assert.Equal(singleIndent + "{ 1, 2 },", processedCode[2]);
      Assert.Equal(singleIndent + "{ 3, 4 }", processedCode[3]);
      Assert.Equal("}", processedCode[4]);
    }


    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Should_Compensate_ForDots(string singleIndent)
    {
      var code =
@"
dirItem.AddElement(
        new XElement(""Component"",
            new XAttribute(""Id?"", compId),
            new XAttribute(""Guid"", WixGuid.NewGuid(compId))));
"

.GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent, (i, x) => Debug.WriteLine(x))
                              .ToString()
                              .GetLines();

      Assert.Equal("dirItem.AddElement(", processedCode[0]);
      Assert.Equal(singleIndent + singleIndent + "new XElement(\"Component\",", processedCode[1]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Should_Allow_Args_PartialAlignment(string singleIndent)
    {
      var code =
@"
class Test
{
    void Test()
    {
        var project = new Project(""Test"",
                          new File(""file.txt""));
    }
}".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent, (i, x) => Debug.WriteLine(x))
                              .ToString()
                              .GetLines();

      Assert.Equal("        var project = new Project(\"Test\",", processedCode[4]);
      Assert.Equal("                          new File(\"file.txt\"));", processedCode[5]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Should_Favour_Fluent(string singleIndent)
    {
      // return new ActionBuilder()
      //        .WithId(id)
      //         .WithDuration(duration.test
      //                               .Length)
      //         .Build()
      //     .WithStartTime(startTime);

      var code =
@"
class Test
{
    private OptimiserAction CreateAction(uint id, TimeSpan startTime, TimeSpan duration)
    {
        return new ActionBuilder()
               .WithId(id)
                .WithDuration(duration)
                .Build()
            .WithStartTime(startTime);
    }
}".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent, (i, x) => Debug.WriteLine(x))
                              .ToString()
                              .GetLines();

      Assert.Equal(singleIndent + singleIndent + singleIndent + "   .WithId(id)", processedCode[5]);
      Assert.Equal(singleIndent + singleIndent + singleIndent + "   .WithDuration(duration)", processedCode[6]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Ignore_Intepolation(string singleIndent)
    {
      var code =
@"
class Test
{
    if (allScenarios.Select(a => a.Name).Distinct().Count() != allScenarios.Length)
    {
        throw new InvalidOperationException(
            $@""Some scenarios have duplicate names: {
                    string.Join("", "",
                        allScenarios.GroupBy(a => a.Name)
                            .Where(a => a.Count() > 1)
                            .Select(a => a.Key))
                }"");
    }
}".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent, (i, x) => Debug.WriteLine(x))
                              .ToString()
                              .GetLines();

      // line 5 is the same as before formatting
      Assert.Equal(singleIndent + singleIndent + singleIndent + singleIndent + singleIndent + "string.Join(\", \",", processedCode[6]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Align_Attributes(string singleIndent)
    {
      var code = @"
[assembly: AssemblyInformationalVersion(""0.0.0.0"")]
[assembly: InternalsVisibleTo(""Infrastructure.Tests""),
         InternalsVisibleTo(""TATSimulator"")]".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent)
                              .ToString()
                              .GetLines();

      Assert.Equal("[assembly: AssemblyInformationalVersion(\"0.0.0.0\")]", processedCode[0]);
      Assert.Equal("[assembly: InternalsVisibleTo(\"Infrastructure.Tests\"),", processedCode[1]);
      Assert.Equal(singleIndent + singleIndent + "   InternalsVisibleTo(\"TATSimulator\")]", processedCode[2]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Should_Ignore_IndentPoints_InStrings(string singleIndent)
    {
      var code =
          @"
void Test()
{
         new Exception($@""Some scenarios .have duplicate names: {
                                  string.Join("", "",
                                      allScenarios.GroupBy(a => a.Name)
                                          .Where(a => a.Count() > 1)
                                          .Select(a => a.Key))
                    }"");
}
".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent)
                              .ToString()
                              .GetLines();

      Assert.Equal(singleIndent + singleIndent + singleIndent + singleIndent + singleIndent + singleIndent + singleIndent + singleIndent + "  string.Join(\", \",", processedCode[3]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Should_Keep_ConditionalExprtesions_Aligned(string singleIndent)
    {
      var code =
          @"
class Test
{
    void Test()
    {
        var resourceConfig = schedulerConfiguration.ResourceConfiguration
                              ?? new TaipanResourceModelConfiguration();
    }
}
".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent)
                              .ToString()
                              .GetLines();

      Assert.Equal("        var resourceConfig = schedulerConfiguration.ResourceConfiguration", processedCode[4]);
      Assert.Equal("                             ?? new TaipanResourceModelConfiguration();", processedCode[5]);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("    ")]
    public void Should_Respect_AnonymousTypeBrackets(string singleIndent)
    {
      // however VS native formatting will break anonymous type indent
      var code =
          @"
class Test
{
    void Test(string text)
    {
        var test = text.Select(x => new
                                    {
                                        Hash = x.GetHashCode(),
                                        View = x.ToString()
                                    });
    }
}
".GetSyntaxRoot();

      var processedCode = code.AlignIndents(singleIndent)
                              .ToString()
                              .GetLines();

      Assert.Equal("        var test = text.Select(x => new", processedCode[4]);
      Assert.Equal("                                    {", processedCode[5]);
    }
  }
}