using GraphQL.TestServer;

namespace ZeroQL.TestApp.Models;

public static partial class BrokenQueryFragments
{
    [GraphQLFragment]
    public static UserModel AsUserFromDifferentAssembly(this User user)
    {
        return new UserModel(user.FirstName, user.LastName, user.Role(o => o.Name));
    }

    [GraphQLFragment]
    public static UserModel? AsUserFromDifferentAssembly(this Query query, int userId)
    {
        return query.User(userId, o => o.AsUserFromDifferentAssembly());
    }
}