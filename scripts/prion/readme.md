# Prion: Presenting Riley's Intriguing Object Notation

A statically typed human read and writable object notation language.
This is the reference implementation, written in C#. References to the api reflect idiomatic C#. When ported this api will change to match the needs of the target language.

## Features

Prion is a simple text based object notation, similar to Json. 
It is easy to convert back and forth from Json, Toml, Yaml, and XML (if I can be bothered).
First class support for schemas and validation.
It is safe to deserialize by design and has no side effects while doing so.

## Why, oh God, Why?

Prion is designed for games and the needs of games.

## Main Classes and Types

### PrionNode

#### Methods

```c#
public static bool TryFrom<T>(T value, out PrionNode result);
public static PrionNode FromNull();
public static PrionNode FromError(string message);
public bool TryAs<T>(out T result);
public string ToString();
public JsonNode ToJson();
```

### PrionType

An enum describing the various supported 

### PrionSchema

```c#
public static bool TryValidate(PrionNode node, out string[] errors);
```