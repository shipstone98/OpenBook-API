using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthenticationTest.Mocks;

internal sealed class MockSecurityTokenHandler : SecurityTokenHandler
{
    internal Func<SecurityTokenDescriptor, SecurityToken> _createTokenFunc;
    internal Func<SecurityToken, String> _writeTokenFunc;

    public sealed override bool CanValidateToken =>
        throw new NotImplementedException();

    public sealed override bool CanWriteToken =>
        throw new NotImplementedException();

    public sealed override int MaximumTokenSizeInBytes
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public sealed override Type TokenType =>
        throw new NotImplementedException();

    public MockSecurityTokenHandler()
    {
        this._createTokenFunc = _ => throw new NotImplementedException();
        this._writeTokenFunc = _ => throw new NotImplementedException();
    }

    public sealed override bool CanReadToken(XmlReader reader) =>
        throw new NotImplementedException();

    public sealed override bool CanReadToken(String tokenString) =>
        throw new NotImplementedException();

    public sealed override SecurityKeyIdentifierClause CreateSecurityTokenReference(
        SecurityToken token,
        bool attached
    ) =>
        throw new NotImplementedException();

    public sealed override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor) =>
        this._createTokenFunc(tokenDescriptor);

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    public sealed override SecurityToken ReadToken(XmlReader reader) =>
        throw new NotImplementedException();

    public sealed override SecurityToken ReadToken(
        XmlReader reader,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override SecurityToken ReadToken(String token) =>
        throw new NotImplementedException();

    public sealed override String? ToString() =>
        throw new NotImplementedException();

    public sealed override ClaimsPrincipal ValidateToken(
        XmlReader reader,
        TokenValidationParameters validationParameters,
        out SecurityToken validatedToken
    ) =>
        throw new NotImplementedException();

    public sealed override ClaimsPrincipal ValidateToken(
        String securityToken,
        TokenValidationParameters validationParameters,
        out SecurityToken validatedToken
    ) =>
        throw new NotImplementedException();

    public sealed override Task<TokenValidationResult> ValidateTokenAsync(
        SecurityToken token,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override Task<TokenValidationResult> ValidateTokenAsync(
        String token,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override String WriteToken(SecurityToken token) =>
        this._writeTokenFunc(token);

    public sealed override void WriteToken(
        XmlWriter writer,
        SecurityToken token
    ) =>
        throw new NotImplementedException();
}
