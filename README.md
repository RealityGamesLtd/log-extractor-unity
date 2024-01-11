# Log Extractor for Unity

## Prerequisites
Check and install all [DEPENDENCIES](#dependencies) before importing this package to avoid errors.

## Usage
Attach LogExtractionTool as a component to a Game Object.
By default logs will be saved on every OnApplicationPause `Pause == true`.
Logs can be accessed at: `{Application.persistentDataPath}/data/{dataFileName}.txt`
or shared directly from the device.

Reference to the component
```csharp
public LogExtractionTool logExtractionTool;
```

Optional setup:
```csharp
public LogExtractionTool logExtractionTool;
logExtractionTool.Setup(enableSaveLogsOnPause: true);
```

Sharing logs:
(It will open a share sheet on the device)
```csharp
public LogExtractionTool logExtractionTool;
logExtractionTool.SendLogs();
```

## Dependencies

### UnityNativeShare
https://github.com/yasirkula/UnityNativeShare