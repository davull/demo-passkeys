using DemoPasskeys.Repositories;
using FluentAssertions;

namespace DemoPasskeys.Tests.Repositories;

public class UsersRepositoryTests
{
    [Test]
    public void Write_Test_Users()
    {
        var users = new[]
        {
            Dummies.UserModel("test-001", "peter.pan@test.de", "peter"),
            Dummies.UserModel("test-002", "niko.laus@test.de", "niko")
        };

        foreach (var user in users)
            UsersRepository.Write(user);
    }

    [Test]
    public void Write_Should_Not_Throw()
    {
        var user = Dummies.UserModel();

        Assert.DoesNotThrow(() => UsersRepository.Write(user));

        UsersRepository.Delete(user.Id);
    }

    [Test]
    public void Read_Should_Read_User()
    {
        var id = Guid.NewGuid().ToString();

        UsersRepository.Write(Dummies.UserModel(id));

        var user = UsersRepository.Read(id);

        user.Should().NotBeNull();

        UsersRepository.Delete(id);
    }

    [Test]
    public void Delete_Should_Delete_User()
    {
        var id = Guid.NewGuid().ToString();

        UsersRepository.Write(Dummies.UserModel(id));

        UsersRepository.Read(id).Should().NotBeNull();

        UsersRepository.Delete(id);

        UsersRepository.Read(id).Should().BeNull();
    }

    [Test]
    public void FindByEmail_WithNonExistingEmail_Should_Return_Null()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var user = UsersRepository.FindByEmail(email);

        user.Should().BeNull();
    }

    [Test]
    public void FindByEmail_WithExistingEmail_Should_Return_User()
    {
        var email1 = $"{Guid.NewGuid()}@example.com";
        var email2 = $"{Guid.NewGuid()}@example.com";

        UsersRepository.Write(Dummies.UserModel(email: email1));
        UsersRepository.Write(Dummies.UserModel(email: email2));

        var user1 = UsersRepository.FindByEmail(email1);
        user1.Should().NotBeNull();

        var user2 = UsersRepository.FindByEmail(email2);
        user2.Should().NotBeNull();

        UsersRepository.Delete(user1!.Id);
        UsersRepository.Delete(user2!.Id);
    }
}