# Discord Rich Presence Integration for FL Studio (Windows)


## Prerequisites

- .NET Framework (minimum version: 4.5)
- FL Studio installed on your system


## Usage

1. Upon launching the application, it attempts to read the FL Studio path from a configuration file (`config.txt`).
2. If the FL Studio path is not found in the configuration file, a file dialog prompts you to select the FL Studio executable (`FL64.exe` or `FL.exe`).
3. Once the FL Studio path is determined, the application initializes the Discord RPC client and starts the FL Studio process.
4. The application continuously monitors the FL Studio process and updates the elapsed time in the Discord Rich Presence.
5. When FL Studio is closed, the application exits.

## Customization

You can customize the Discord Rich Presence by modifying the code in the `DiscordRPCApplication` class. For example, you can change the large image key and text to suit your preferences or add additional details.

```csharp
var presence = new RichPresence()
{
    Details = elapsedTime,
    Assets = new Assets()
    {
        LargeImageKey = "icon",
        LargeImageText = "FL Studio"
    }
};
client.SetPresence(presence);
```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

This project utilizes the [DiscordRPC](https://github.com/Lachee/discord-rpc-csharp) library.
