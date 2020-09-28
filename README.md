# RollerCoaster.Coaster.Proxy

<a href="https://www.nuget.org/packages/RollerCoaster.Coaster.Proxy/">
    <img src="https://img.shields.io/nuget/v/RollerCoaster.Coaster.Proxy">
</a>

Account Proxy

Features
* All API End Points from Account API
* Policy based retrys and timeouts.
* Logs for all successful and exceptional runs
* Telemetry for all calls

<a href="https://dev.azure.com/marksamdickinson/RollerCoaster/_build?definitionScope=%5CRollerCoaster.Coaster.Proxy">Builds</a>

<h2>Example Usage</h2>

```C#
 var restResponse = await coasterProxyService.LogAsync();
```
