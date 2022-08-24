## Imato.Net.Cmd
cmd utils for .net

### Run shell command

```csharp
using Imato.Net.Cmd;

protected readonly ILogger Logger;

var command = $"curl.exe -X GET \"{builder}\"";
var workDir = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath),
    "External\\curl\\bin");

var runner = new ShellRunner($"{workDir}\\{command}", workDir, Logger);
runner.Run();
```
