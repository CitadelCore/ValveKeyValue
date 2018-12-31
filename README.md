# Valve's KeyValue for .NET

This is a custom version of ValveKeyValue modified for my own use.
For the offical ValveKeyValue, see https://github.com/SteamDatabase/ValveKeyValue.

Differences:
- Different code style and refactorings
- Support for VBKV (Valve Binary KeyValues)
- Support for deserialising KvCollectionValue to a class and back
- Support for deserialising comma separated arrays and back
- Support for compacted brackets and quotation marks which handle escaped quotations properly