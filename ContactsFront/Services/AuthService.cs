using ContactsFront.Api;
using ContactsFront.Models;
using Microsoft.JSInterop;

namespace ContactsFront.Services;

public class AuthService
{
    private readonly IJSRuntime _js;
    private string? _cachedToken;
    private const string TokenKey = "auth_token";
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient http, IJSRuntime js, CustomAuthStateProvider authStateProvider)
    {
        _http = http;
        _js = js;
        _authStateProvider = authStateProvider;
    }

    // POST /Auth/login
    // Sends a login request to the backend and stores the authentication token.
    public async Task<bool> LoginAsync(LoginModel login)
    {
        var resp = await _http.PostAsJsonAsync("/auth/login", login);
        if (!resp.IsSuccessStatusCode) return false;

        var result = await resp.Content.ReadFromJsonAsync<AuthResult>();
        if (result == null || string.IsNullOrEmpty(result.Token)) return false;

        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
        _cachedToken = result.Token;

        _authStateProvider.NotifyUserAuthentication(result.Token);
        return true;
    }

    // Logs out the user by removing the authentication token and updating the authentication state.
    public async Task LogoutAsync()
    {
        _cachedToken = null;
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        _authStateProvider.NotifyUserLogout();
       
    }

    // Retrieves the authentication token from local storage or cache.
    public async Task<string?> GetTokenSafeAsync()
    {
        if (_cachedToken != null)
            return _cachedToken;

        try
        {
            _cachedToken = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
        catch
        {
            return null;
        }

        return _cachedToken;
    }

    // Checks if the user is currently logged in by verifying the token.
    public async Task<bool> IsLoggedInAsync()
    {
        var t = await GetTokenSafeAsync();
        return !string.IsNullOrEmpty(t);
    }

    // POST /Auth/register
    // Sends a registration request to the backend with the provided user information.
    public async Task<HttpResponseMessage> RegisterAsync(RegisterModel register)
    {
        if (string.IsNullOrEmpty(register.Password) || !IsValidPassword(register.Password))
        {
            throw new MissingFieldException("Registration failed. Password must be at least 8 characters, contain a number, an upper character and a special character.");
        }

        return await _http.PostAsJsonAsync("/Auth/register", register);
    }

    // Validates the password according to defined security rules.
    private bool IsValidPassword(string password)
    {
        if (password.Length < 8)
            return false;

        if (!password.Any(char.IsDigit))
            return false;

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;

        if (!password.Any(char.IsUpper))
            return false;

        return true;
    }
}
