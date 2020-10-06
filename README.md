# Columbae
A geo library, based on Polylines, for dotnet core

# Polylines
The polylines concept is designed by google: https://developers.google.com/maps/documentation/utilities/polylinealgorithm

# Documentation

## Encoding & decoding

__Encoding multiple points__

Encoding polyline can be done, just by executing the `ToString()` method overload.

```csharp
var points = new List<Polypoint> {
    new Polypoint(latitude: 41.86231, longitude: -87.63804),
    new Polypoint(latitude: 41.87458, longitude: -87.63460),
};
var polyline = new Polyline(points);

Console.WriteLine(polyline.ToString()); // mfo~Fvx{uOukAoT
```

__Decoding a polyline string to Points__

Decoding a polyline can be done, just by passing the poly line string to the constructor.

```csharp
var polylineString = "mfo~Fvx{uOukAoT";
var polyline = new Polyline(polylineString);

foreach(var point in polyline.Points)
{
    Console.WriteLine(point);
}
```
# Origin

## Credits
The actual parsing logic is based on the repo of [Polyliner.NET](https://github.com/sglogowski/Polyliner.NET) by [sglogowski](https://github.com/sglogowski).

## Name
Named after the fast moving star [Mu Columbae](https://en.wikipedia.org/wiki/Mu_Columbae), which is a Runaway star.
A runaway star is one that is moving through space with an abnormally high velocity relative to the surrounding interstellar medium. The proper motion of a runaway star often points exactly away from a stellar association, of which the star was formerly a member, before it was hurled out.
