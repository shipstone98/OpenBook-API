using System;

namespace Shipstone.Extensions.Identity;

public interface IPasswordService
{
    String Hash(String password);
    void Validate(String password);
    bool Verify(String passwordHash, String password);
}
