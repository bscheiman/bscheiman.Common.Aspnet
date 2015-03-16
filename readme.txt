===============================
= bscheiman.Common // ASP.NET =
===============================

Thanks for downloading this package. If you need help, hit me up on Twitter: @bscheiman.


If you're using ASP.NET Identity + OWIN, there are a few things you have to take care of:

1. Remove the default Startup.Auth.cs calls to CreateOwinContext, replacing those with:
XXX


===============
= SUGGESTIONS =
===============

1. Your DB context should derive from TrackingDbContext.
- TrackingDbContext = TrackingDbContext<IdentityUser>
- If you have a custom user, you can use TrackingDbContext<YourUser>
- If the main key isn't a string, use the whole enchilada: TrackingDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>

2. Application_Start / Global.asax.cs
- Call: GlobalAsaxManager.Config<YourContext>(this, true, builder => { });

3. 