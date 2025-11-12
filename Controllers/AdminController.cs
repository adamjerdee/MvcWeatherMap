using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    private readonly ISecretRepository _repo;
    private readonly ISecretProtector _protector;
    private const string KeyName = "GoogleMapsApiKey";

    public AdminController(ISecretRepository repo, ISecretProtector protector)
    {
        _repo = repo;
        _protector = protector;
    }

    [HttpGet]
    public async Task<IActionResult> ApiKey()
    {
        var vm = new ApiKeyViewModel();
        var enc = await _repo.GetAsync(KeyName);
        if (enc is not null) vm.CurrentMasked = Mask(_protector.Unprotect(enc));
        return View(vm); // Views/Admin/ApiKey.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApiKey(ApiKeyViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.NewKey))
        {
            vm.Error = "Please enter a key.";
            return View(vm);
        }

        var enc = _protector.Protect(vm.NewKey.Trim());
        await _repo.SetAsync(KeyName, enc);

        vm.Status = "API key saved.";
        vm.CurrentMasked = Mask(vm.NewKey);
        vm.NewKey = null;
        return View(vm);
    }

    private static string Mask(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        if (key.Length <= 8) return new string('*', key.Length);
        return key[..5] + "..." + key[^4..];
    }
}
