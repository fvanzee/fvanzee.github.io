<!--
Description: A guide on how to integrate EmailJS with Google reCAPTCHA in a Blazor WebAssembly application to create a secure contact form
Keywords: blazor, emailjs, recaptcha, dotnet
Author: Freek van Zee
Created: 20250201
-->
In my [previous post](blog/a-blazor-webassembly-static-site-on-github-pages), I discussed setting up a Blazor WebAssembly static site on GitHub Pages. Today, I'll walk you through how I integrated [EmailJS](https://www.emailjs.com/) with [Google reCAPTCHA](https://www.google.com/recaptcha/) to create a secure contact form. This integration helps prevent spam while providing a seamless way to handle contact form submissions without a backend server.

<br>

## Why EmailJS and reCAPTCHA?

<br>

- **EmailJS**: Allows sending emails directly from client-side code without a backend server - perfect for static sites
- **Google reCAPTCHA**: Protects the contact form from spam and prevents bots from consuming EmailJS API limits

<br>

## Implementation Steps

<br>

### 1. Setting Up the Contact Form Model


<br>

You can find the full `Contact.razor` file in the [GitHub repository](https://github.com/fvanzee/fvanzee.github.io/blob/main/Pages/Contact.razor).

<br>

First, I created model classes to handle the form data and EmailJS integration:

```csharp
public class ContactMe
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
}

public class TemplateParams : ContactMe
{
    [JsonPropertyName("g-recaptcha-response")]
    public required string RecaptchaToken { get; init; }
}

public class PostEmailJs
{
    public required TemplateParams TemplateParams { get; init; }
    public required string ServiceId { get; init; }
    public required string UserId { get; init; }
    public required string TemplateId { get; init; }
}
```

### 2. Adding the MudBlazor Contact Form

<br>

I used a MudBlazor form to collect user input:

```razor
<MudForm @ref="_form" @bind-IsValid="@_formSuccess" @bind-Errors="@_errors" Class="px-4">
    <MudTextField T="string" OnlyValidateIfDirty="true" Label="Email" Required="true"
        RequiredError="Email is required!" @bind-Value="_model!.Email"
        Validation="@(new EmailAddressAttribute() {ErrorMessage = "The email address is invalid"})" />
    <MudTextField T="string" OnlyValidateIfDirty="true" Label="Phone" @bind-Value="_model!.Phone" />
    <MudTextField T="string" OnlyValidateIfDirty="true" Label="Message" Required="true"
        RequiredError="Message is required!" @bind-Value="_model!.Message" Lines="10" />
</MudForm>
```

### 3. Adding reCAPTCHA Component

<br>

I used a reCAPTCHA component in the contact form with success and expiration handlers:

```html
<ReCaptcha @ref="_reCaptchaComponent" 
    SiteKey="your-site-key"
    OnSuccess="OnSuccess" 
    OnExpired="OnExpired" />
```

```csharp
private ReCaptcha? _reCaptchaComponent;
private bool _validReCaptcha;

private void OnSuccess()
{
    _validReCaptcha = true;
}

private void OnExpired()
{
    _validReCaptcha = false;
}
```

See more below on how the ReCaptcha component is implemented.

<br>

### 4. Form Submission with EmailJS

<br>

I implemented a form submission process that includes form validation, reCAPTCHA verification, and EmailJS integration.

```razor
<MudStack Row="true" Class="d-flex flex-wrap align-center justify-space-between pa-2">
    <ReCaptcha @ref="_reCaptchaComponent" SiteKey="your-site-key"
        OnSuccess="OnSuccess" OnExpired="OnExpired" />
    <MudButton Variant="Variant.Filled" OnClick="Submit" Color="Color.Primary"
        Disabled="@(!_formSuccess || !_validReCaptcha)" Class="ml-auto">
        Submit</MudButton>
</MudStack>
```

<br>

The form submission process combines form validation, reCAPTCHA verification, and EmailJS:

1. Validate the form inputs
2. Check reCAPTCHA status
3. Get reCAPTCHA response token
4. Create EmailJS request with form data and reCAPTCHA token
5. Send the request to EmailJS API

```csharp
private async Task Submit()
{
    await _form.Validate();
    if (!_formSuccess || !_validReCaptcha)
    {
        return;
    }

    var recaptchaToken = await _reCaptchaComponent!.GetResponseAsync();
    
    var templateParams = new TemplateParams
    {
        Email = _model.Email,
        Phone = _model.Phone,
        Message = _model.Message,
        RecaptchaToken = recaptchaToken
    };

    var postEmailJs = new PostEmailJs
    {
        TemplateParams = templateParams,
        ServiceId = "your-service-id",
        UserId = "your-user-id",
        TemplateId = "your-template-id"
    };

    var response = await Http.PostAsJsonAsync(
        "https://api.emailjs.com/api/v1.0/email/send", 
        postEmailJs,
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        }
    );
}
```

## Key Features

<br>

1. **Form Validation**: Uses MudBlazor's form validation for required fields and email format
2. **reCAPTCHA Integration**: Prevents form submission until reCAPTCHA is completed
3. **User Feedback**: Shows success/error messages using a snackbar component
4. **Security**: Includes reCAPTCHA token with each EmailJS request
5. **Clean UI**: Integrates seamlessly with MudBlazor components

<br>

## Configuration Requirements

<br>

To implement this in your own project, you'll need:

1. An EmailJS account with:
  - Service ID
  - User ID
  - Email template ID
2. Google reCAPTCHA site key
3. MudBlazor package for UI components
4. A reCAPTCHA component for Blazor

<br>

Ensure you enable ReCAPTCHA in the [Google reCAPTCHA console](https://console.cloud.google.com/security/recaptcha).

<br>

Follow the [instructuions from EmailJS](https://www.emailjs.com/docs/user-guide/adding-captcha-verification/) to enable ReCaptcha on the email template.

**Note:** Only reCAPTCHA V2 is supported at time of writing.

<br>

## Understanding the ReCaptcha Component

<br>

The ReCaptcha component in this implementation is a reusable Blazor component that wraps the Google reCAPTCHA v2 functionality. Let's break down how it works:

<br>

### Component Structure

<br>

The component consists of two main parts:
1. A Blazor component ([`ReCaptcha.razor`](https://github.com/fvanzee/fvanzee.github.io/blob/main/Components/ReCaptcha.razor)) that handles the .NET side
2. A JavaScript module ([`recaptcha.js`](https://github.com/fvanzee/fvanzee.github.io/blob/main/wwwroot/scripts/recaptcha.js)) that interfaces with Google's reCAPTCHA API

<br>

### Key Features

<br>

1. **Automatic Script Loading**: The component automatically loads the reCAPTCHA script if it's not already present:
   ```javascript
   if (!scripts.some((s) => s.src.startsWith("https://www.google.com/recaptcha/api.js"))) {
     const script = document.createElement("script");
     script.src = "https://www.google.com/recaptcha/api.js?render=explicit";
     script.async = true;
     script.defer = true;
     document.head.appendChild(script);
   }
   ```

2. **Component Parameters**:
   - `SiteKey`: Your Google reCAPTCHA site key
   - `OnSuccess`: Event callback when verification succeeds
   - `OnExpired`: Event callback when the verification expires

3. **JavaScript Interop**: The component uses Blazor's JavaScript interop to communicate with the reCAPTCHA API:
   ```csharp
   protected override async Task OnInitializedAsync()
   {
       await JS.InvokeAsync<object>("reCAPTCHA.init");
       WidgetId = await JS.InvokeAsync<int>("reCAPTCHA.render", 
           DotNetObjectReference.Create(this), 
           UniqueId, 
           SiteKey);
   }
   ```

4. **Helper Methods**:
   - `GetResponseAsync()`: Retrieves the verification token
   - `ResetAsync()`: Resets the reCAPTCHA widget

<br>

The component generates a unique ID for each instance, allowing multiple reCAPTCHA widgets on the same page if needed.

<br>

## Conclusion

<br>

This integration provides a secure and efficient way to handle contact form submissions in a static Blazor WebAssembly site. The combination of EmailJS and reCAPTCHA ensures that the contact form is both functional and protected against spam, all while maintaining a serverless architecture.

Remember to keep your EmailJS and reCAPTCHA keys secure and consider implementing rate limiting on your EmailJS account to prevent abuse.
