using System;
using ZeroQL.Internal.Enums;
using ZeroQL.Schema;

namespace ZeroQL.Bootstrap;

public record GraphQlGeneratorOptions(string ClientNamespace, ClientVisibility Visibility)
{
    public string? ClientName { get; set; }

    public string AccessLevel => Visibility switch
    {
        ClientVisibility.Public => "public",
        ClientVisibility.Internal => "internal",
        _ => throw new ArgumentOutOfRangeException()
    };

    public string GetDefinitionFullTypeName(Definition definition)
        => $"global::{ClientNamespace}.{definition.Name}";
}