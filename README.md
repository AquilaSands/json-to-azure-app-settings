# JSON to Azure Portal Web App Configuration
Quick'n'dirty console app to convert .net core appsettings.json file to Azure Web App portal configuration advanced edit format so it can be copy pasted.

Built with .net core 3.

No release just clone the repo, build and run (it works on my machine :grin:).

![image](https://user-images.githubusercontent.com/5774614/66256263-01003180-e784-11e9-9b1d-0f5477ed514a.png)

**Turn this**
```JSON
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        }
    },
    "MyConfig": {
        "Args": [
            "first",
            true,
            false,
            {
                "objProp": "Quick and dirty!"
            },
            3.141
        ],
        "AnswerToLife": 42,
        "Directory": "c:\\temp",
        "Parent": {
            "Child": {
                "Bool1": true
            },
            "Bool2": false
        }
    }
}
```

**Into this**
```JSON
[
  {
    "name": "Logging:LogLevel:Default",
    "value": "Debug",
    "slotSetting": false
  },
  {
    "name": "Logging:LogLevel:System",
    "value": "Information",
    "slotSetting": false
  },
  {
    "name": "Logging:LogLevel:Microsoft",
    "value": "Information",
    "slotSetting": false
  },
  {
    "name": "MyConfig:Args",
    "value": "[\"first\",true,false,{\"objProp\":\"Quick and dirty!\"},3.141]",
    "slotSetting": false
  },
  {
    "name": "MyConfig:AnswerToLife",
    "value": "42",
    "slotSetting": false
  },
  {
    "name": "MyConfig:Directory",
    "value": "c:\\temp",
    "slotSetting": false
  },
  {
    "name": "MyConfig:Parent:Child:Bool1",
    "value": "True",
    "slotSetting": false
  },
  {
    "name": "MyConfig:Parent:Bool2",
    "value": "False",
    "slotSetting": false
  }
]
```

So you can paste it into your Azure portal Web App configuration application settings using the `Advanced Edit`.

![image](https://user-images.githubusercontent.com/5774614/66256111-926ea400-e782-11e9-84cd-47b1b477f9af.png)

Yes, there are better ways to deploy your settings with a proper CI tool but sometimes you just need quick and dirty :wink:.
