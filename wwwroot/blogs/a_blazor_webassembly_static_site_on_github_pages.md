# A Blazor WebAssembly static site on Github Pages

<br>

For a long while I've wanted to setup a personal portfolio website.  
<br>
Being mainly a backend and DevOps developer specialized in .NET, using Blazor to do this seemed the right choice. I wanted to limit the amount of JavaScript (who doesn't?!) and CSS.

<br>

**Requirements:**

<br>

I started out defining some requirements, so I had a general idea what my approach would be.  

<br>

* Simple and easy to update. I didn't want to spend weeks on it. I also didn't want a flashy site.
* Self built, not using Wordpress-like site builders.
* A home page with some general information about me.
* An overview page of all the technology stacks I'm familiar with
* A contact form where people could reach me without exposing my email address
* Hosting the site and it's features should be dirt-cheap, preferably free
* A static website, as it keeps things cheap an simple.

<br>

After some Googling on Reddit I landed on the following:

<br>

* A [Blazor WebAssembly](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) site. It's ideal for a static website with minimal JavaScript
* [MudBlazor](https://mudblazor.com/) Components. Free components to limit development work on styling etc...
* Hosted using [Github Pages](https://pages.github.com/). Free and easy to setup.
* [EmailJS](https://www.emailjs.com/) for the contact form. Free for limited use (I don't expect many emails).
* [Google ReCaptcha](https://www.google.com/recaptcha/about/). Prevent malicious bots eating through my EmailJS limits.

<br>

Did it work? Well, you're on the site right now!

I will go through the base steps to setup a Blazor static site to host it on Github pages, and create follow up blog on integrating with EmailJS with Google Recaptcha on the contact page.

<br>

## Setting up the Blazor WASM project

<br>

I won't go through the all the steps on how to setup a Blazor project using MudBlazor, as the [MudBlazor Documentation](https://mudblazor.com/getting-started/installation) explains it pretty well. Follow the instructions. You'll of course need to have the .NET SDK installed.

<br>

Personally I chose to go for the [manual install](https://mudblazor.com/getting-started/installation#manual-install) steps, simply because it gave me a better idea on how Blazor and MudBlazor work. I assume using a template works just as well.

<br>

For the manual installation start a new `blazorwasm` project and install the MudBlazor package

```shell
$ dotnet new blazorwasm
$ dotnet add package MudBlazor
```

If you choose for a manual install then also make sure you setup `Layout/MainLayout.razor`. So decide on a layout type and follow the [Layout setup](https://mudblazor.com/getting-started/layouts). Don't forget to update [`Layout/NavMenu.razor`](https://mudblazor.com/getting-started/layouts#navigation-menu) as well.

I went for the "Appbar and Drawer" layout and ended up with:

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<MudThemeProvider />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar>
        <MudIconButton Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" Edge="Edge.Start" OnClick="@((e) => DrawerToggle())" />
        My Application
    </MudAppBar>
    <MudDrawer @bind-Open="@_drawerOpen">
        <MyNavMenu/>
    </MudDrawer>
    <MudMainContent>
        @Body
    </MudMainContent>
</MudLayout>

@code {
    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
}
```

```razor
@* NavMenu.razor *@
<MudNavMenu>
    <MudNavLink Href="/" Match="NavLinkMatch.All">About Me</MudNavLink>
    <MudNavLink Href="/technologies" Match="NavLinkMatch.Prefix">Technologies</MudNavLink>
    <MudNavLink Href="/contact"  Match="NavLinkMatch.Prefix">Contact Me</MudNavLink>
</MudNavMenu>
```

## Styling

<br>

Next you might want to setup your styling configuration to create a more personalized look and feel for your site.

I simply went for the default Dark Theme, and changed the loading bar animation.

You can find CSS for loading bars on [freefrontend.com](https://freefrontend.com/css-progress-bars/) if you don't want to bother creating your own like me.

```razor
@* MainLayout.razor *@
<MudThemeProvider IsDarkMode="true"/>
```

```html
<!-- index.html -->
...
<body>
    <div id="app">
        <div class="progress progress-striped">
            <div class="progress-bar" />
        </div>
    </div>
...
</body> 
```

Add the CSS class to `app.css` using `--blazor-load-percentage` to control the progress bar progress.

```css
.progress {
    position: absolute;
    text-align: center;
    width: 20rem;
    top: 50%;
    left: 50%;
    transform: translateX(-50%);
    margin-left: auto;
    margin-right: auto;
    padding: 6px;
    background: rgba(0, 0, 0, 0.25);
    border-radius: 6px;
    box-shadow: inset 0 1px 2px rgba(0, 0, 0, 0.25),
        0 1px rgba(255, 255, 255, 0.08);
}

.progress-bar {
    height: 18px;
    background-color: #ee303c;
    border-radius: 4px;
    transition: 0.4s linear;
    transition-property: width, background-color;
}

.progress-striped .progress-bar {
    width: var(--blazor-load-percentage, 0%);
    background-color: #3c6e71;
    background-image: linear-gradient(45deg,
            #353535 25%,
            transparent 25%,
            transparent 50%,
            #353535 50%,
            #353535 75%,
            transparent 75%,
            transparent);
    transition: width 0.4s ease-in-out;
}
```

## Pages

<br>

Remove all default pages created by the `blazorwasm` template. You can keep `Home.razor` if you want.

<br>

Add the pages you defined in your navigation component (`Layout/NavMenu.razor`) in the `Pages` folder. For me this meant adding `Pages/Technologies.razor` and `Pages/Contact.razor`  

<br>

Build your pages however you like. Make good use of MudBlazor Components like `MudCard` and `MudPaper`, as they can help you get all your content nicely centered and responsive.

<br>

Don't forget to set the correct `@page` at the top of every page to make the routing work.

<br>

I won't go into detail on how I built my pages.  
The Technologies page doesn't have any extra integrations, and you can find the source on this site's Github repository.
The Contact page, using EmailJS + ReCaptcha deserves a separate shorter blog post.

<br>

## Github Pages

<br>

Please reference the documentation on [GitHub](https://pages.github.com/) for the latest way to setup GitHub pages.

There's an important thing you need to consider when hosting your site on Github pages, that I ran into myself.

<br>

### User / Organization or Project site 

<br>

Choose whether you want to host your site on:  
`{your_gh_username}.github.io` or on  
`{your_gh_username}.github.io/{your_repo}`

<br>

If you want to have a "User / Organization site", you repository must be named `{your_gh_username}.github.io`.  
Setting up GitHub pages for any other repository name will result in a project site.

<br>

It's important to note that on a "Project site", your site's `index.html` will be hosted on a path (the name of your repository). Blazor will take this route and use it to route to the the page. Make sure to prefix all your `@page` routes with your repository name (e.g. `@page /your_repo/contact`).  
This is not needed when using a "User / Organisation site", as this will be running on the root path.

<br>

Test your site locally to make sure everything is working as expected.

```shell
$ dotnet run
```

### Github Workflows / Build and Deploy

<br>

You'll want to automate deploying your page in a CI/CD pipeline. I set it up so every new commit on the `main` branch triggers the pipeline and updates my page.

<br>

Add a new file `.github/workflows/build-and-deploy.yaml`.

```yaml
name: Build and Deploy

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 9.0.101
    - name: Publish
      run: dotnet publish {your_project_name}.csproj -o ./publish

    - name: Upload artifact
      uses: actions/upload-pages-artifact@v3
      with:
        # Upload build directory content
        path: 'publish/wwwroot'

  deploy:
    needs: build  # wait for the "build" job to get done before executing this "deploy" job

    # Grant GITHUB_TOKEN the permissions required to make a Pages deployment
    permissions:
      pages: write      # to deploy to Pages
      id-token: write   # to verify the deployment originates from an appropriate source

    runs-on: ubuntu-latest

    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}

    steps:
      - name: Setup Pages
        uses: actions/configure-pages@v5
      - name: Deploy to GitHub Pages
        id: deployment
        uses: actions/deploy-pages@v4
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

Make sure to replace `{your_project_name}` and check the `dotnet-version`.

<br>

Once you've pushed your code to the github repository, and the build and deploy pipeline finished, you should have live site at `{your_gh_username}.github.io`!

<br>

### 404 Page

<br>

You might notice when you navigate to one of your pages and refresh, your end up on a GitHub Pages 404 page. This is because `index.html` is only served from the root route, and all other navigation happens purely client side once the site is loaded.

<br>

To resolve this you can add a `404.html` page next to your `index.html`. GitHub page will load this page on `404` results, so it can be leveraged to rewrite to the root, while retaining path information in query parameters.

Get the `404.html` page I am using [HERE](https://raw.githubusercontent.com/rafgraph/spa-github-pages/refs/heads/gh-pages/404.html)!