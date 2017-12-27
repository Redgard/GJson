# GJson
[![NuGet version](https://badge.fury.io/nu/GJson.svg)](https://badge.fury.io/nu/GJson)

User-friendly json C# library with serilization on .NET 3.0

 * Easy load and dump
 * Easy manipulating, no need to create objects yourself with many "new" 
 * Support serilization

Load from string
```csharp
var json = JsonValue.Parse("{\"a\" : 12, \"b\":[]}");
```

Pretty print to string
```csharp
var str = json.ToStringIdent();

//{
//	"a" : 12,
//	"b" : []
//}
```

Create empty and fill
```csharp
var json = new JsonValue();
json["a"] = 15;
json["b"][0] = "first";
json["b"][1] = "second";
json["c"]["some"] = 3.14;
```
