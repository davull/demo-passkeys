using DemoPasskeys.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DemoPasskeys.Controllers;

// https://github.com/herrjemand/awesome-webauthn

[ApiController]
[Route("api/passkeys/[action]")]
public class PasskeysApiController(IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet]
    public IActionResult PublicKeyCredentialCreationOptions()
    {
        var userId = httpContextAccessor.HttpContext!.GetAuthCookie()!;
        var user = UsersRepository.Read(userId)!;

        var options = Passkeys.CreatePublicKeyCredentialCreationOptions(user);
        return Ok(options);
    }

    [HttpGet]
    public IActionResult PublicKeyCredentialRequestOptions()
    {
        var options = Passkeys.CreatePublicKeyCredentialRequestOptions();
        return Ok(options);
    }

    [HttpPost]
    public IActionResult UserPasskey(StorePasskeyModel data)
    {
        PasskeysRepository.Write(data);
        return Ok();
    }

    [HttpPost]
    public IActionResult Verify(VerifyPasskeyModel data)
    {
        var stored = PasskeysRepository.Read(data.Id);
        if (stored is null)
            return Unauthorized();

        var valid = Passkeys.Verify(data, stored);
        return valid ? Ok() : Unauthorized();
    }
}