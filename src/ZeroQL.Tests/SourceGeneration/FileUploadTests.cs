using FluentAssertions;
using Xunit;
using ZeroQL.Tests.Core;
using ZeroQL.Tests.Data;

namespace ZeroQL.Tests.SourceGeneration;

public class FileUploadTests : IntegrationTest
{
    [Fact]
    public async Task UploadFileAsClassInstance()
    {
        var csharpQuery = "Mutation(new AddProfileImage(1, new Upload(\"image.png\", new MemoryStream(new byte[42]))), static (i, m) => m.AddUserProfileImage(i.UserId, i.File))";
        var project = await TestProject.Project
            .ReplacePartOfDocumentAsync("Program.cs", (TestProject.FullMeQuery, csharpQuery));

        var result = (GraphQLResult<int>)await project.Execute();
        result.Data.Should().Be(42);
    }
    
    [Fact]
    public async Task UploadFileAsAnonymousTypeWithMultipleProperties()
    {
        var csharpQuery = "Mutation(new { UserId = 1, File = new Upload(\"image.png\", new MemoryStream(new byte[42])) }, static (i, m) => m.AddUserProfileImage(i.UserId, i.File))";
        var graphqlQuery = @"mutation ($userId: Int!, $file: Upload!) { addUserProfileImage(userId: $userId, file: $file)}";

        var project = await TestProject.Project
            .ReplacePartOfDocumentAsync("Program.cs", (TestProject.FullMeQuery, csharpQuery));

        var result = (GraphQLResult<int>)await project.Validate(graphqlQuery);
        result.Data.Should().Be(42);
    }

    [Fact]
    public async Task UploadFileAsAnonymousType()
    {
        var csharpQuery = "Mutation(new { File = new Upload(\"image.png\", new MemoryStream(new byte[42])) }, static (i, m) => m.AddMyProfileImage(i.File))";
        var graphqlQuery = @"mutation ($file: Upload!) { addMyProfileImage(file: $file)}";

        var project = await TestProject.Project
            .ReplacePartOfDocumentAsync("Program.cs", (TestProject.FullMeQuery, csharpQuery));

        var result = (GraphQLResult<int>)await project.Validate(graphqlQuery);
        result.Data.Should().Be(42);
    }

    [Fact]
    public async Task UploadFileAsDeepNestedAnonymousType()
    {
        var usersVariable = @"var users = new UserInfoInput[]
        {
            new() { FirstName = ""John"", LastName = ""Smith"", Avatar = new Upload(""image.png"", new MemoryStream(new byte[42])) },
            new() { FirstName = ""Ben"", LastName = ""Smith"", Avatar = new Upload(""image.png"", new MemoryStream(new byte[42])) }
        };";
        var csharpQuery = "Mutation(new { Users = users }, static (i, m) => m.AddUsersInfo(i.Users))";

        var graphqlQuery = @"mutation ($users: [UserInfoInput!]!) { addUsersInfo(users: $users)}";

        var project = await TestProject.Project
            .ReplacePartOfDocumentAsync("Program.cs",
                (TestProject.PlaceToReplace, usersVariable), 
                (TestProject.FullMeQuery, csharpQuery));

        var result = (GraphQLResult<int>)await project.Validate(graphqlQuery);
        result.Data.Should().Be(84);
    }
    
    [Fact]
    public async Task UploadFileAsDeepNestedAnonymousTypeWithClosureSyntax()
    {
        var usersVariable = @"var users = new UserInfoInput[]
        {
            new() { FirstName = ""John"", LastName = ""Smith"", Avatar = new Upload(""image.png"", new MemoryStream(new byte[42])) },
            new() { FirstName = ""Ben"", LastName = ""Smith"", Avatar = new Upload(""image.png"", new MemoryStream(new byte[42])) }
        };";
        var csharpQuery = "Mutation(m => m.AddUsersInfo(users))";

        var project = await TestProject.Project
            .ReplacePartOfDocumentAsync("Program.cs",
                (TestProject.PlaceToReplace, usersVariable), 
                (TestProject.FullMeQuery, csharpQuery));

        var result = (GraphQLResult<int>)await project.Execute();
        result.Data.Should().Be(84);
    }
}