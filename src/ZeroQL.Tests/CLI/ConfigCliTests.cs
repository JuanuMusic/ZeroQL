using System.Text.Json;
using CliFx.Infrastructure;
using ZeroQL.CLI;
using ZeroQL.CLI.Commands;
using ZeroQL.Internal.Enums;
using ZeroQL.Json;
using ZeroQL.Tests.Core;

namespace ZeroQL.Tests.CLI;

[UsesVerify]
public class ConfigCliTests
{
    [Fact]
    public async Task CanCreateWithDefaultValues()
    {
        using var console = new FakeInMemoryConsole();
        var tempFile = Path.GetTempFileName();
        var command = new InitConfigCommand()
        {
            Output = tempFile
        };

        await command.ExecuteAsync(console);

        var config = await File.ReadAllTextAsync(tempFile);

        await Verify(config);
    }

    [Fact]
    public async Task CanCreateWithCustomValues()
    {
        using var console = new FakeInMemoryConsole();
        console.WriteInput($"""
                    ./service.graphql
                    Service.ZeroQL.Client
                    ServiceZeroQLClient
                    QL.g.cs
                    """);

        var tempFile = Path.GetTempFileName();
        var command = new InitConfigCommand()
        {
            Output = tempFile
        };

        await command.ExecuteAsync(console);

        var config = await File.ReadAllTextAsync(tempFile);

        await Verify(config);
    }

    [Fact]
    public async Task ConfigCanBeParsed()
    {
        using var console = new FakeInMemoryConsole();
        var tempFile = Path.GetTempFileName();
        var config = new ZeroQLFileConfig()
        {
            GraphQL = "./service.graphql",
            Namespace = "Service.ZeroQL.Client",
            ClientName = "ServiceZeroQLClient",
            Visibility = ClientVisibility.Internal,
            Output = "QL.g.cs"
        };

        var json = JsonSerializer.Serialize(config, ZeroQLJsonOptions.Options);

        await File.WriteAllTextAsync(tempFile, json);

        var generateCommand = new GenerateCommand
        {
            Config = tempFile,
        };

        await generateCommand.ReadConfig(console);

        await Verify(generateCommand)
            .Track(tempFile);
    }
}