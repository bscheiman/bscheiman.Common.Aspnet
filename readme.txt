===============================
= bscheiman.Common // ASP.NET =
===============================

Thanks for downloading this package. If you need help, hit me up on Twitter: @bscheiman.


If you're using ASP.NET Identity + OWIN, there are a few things you have to take care of:

1. Remove the default Startup.Auth.cs calls to CreateOwinContext:
//app.CreatePerOwinContext(YourContext.Create);
//app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

2. Use the GlobalAsaxManager.Config overload that allows for a builder:
- b.RegisterType<UserStore<...>>().AsSelf().AsImplementedInterfaces();
- b.RegisterType<UserManager>().AsSelf().AsImplementedInterfaces();

===============
= SUGGESTIONS =
===============

1. Your DB context should derive from TrackingDbContext.
- TrackingDbContext = TrackingDbContext<IdentityUser>
- If you have a custom user, you can use TrackingDbContext<YourUser>
- If the main key isn't a string, use the whole enchilada: TrackingDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>

2. Startup.cs / Startup.Auth.cs
- Call: StartupManager.Config<YourContext, AnyController>(this, true, builder => { });

3. Add markdown.css & markdown-email.css to project root, build action => Content
These files are used for styling Markdown controllers & e-mail templates, respectively.

4. Time javascript:
---
$("time").each(function() {
    var t = $(this);
    var m = moment.utc(t.attr("datetime") * 1000).local();

    t.html(m.format(t.attr("data-format")));
});
---