

## Motivation

## Features

#### On Mock
- Setup methods without providing al arguments, jsut the ones you specifically want
- Setup verification times along with the method setup (currently Moq only suuports to setup that it has to be called using `Verifiable`)
- Provide matchers for generics with constraints
- The AutoMock gives  a list of setup and non setup methods

####### TODO
- Support verify that an event was raised and it provided the default AutoFxitrue implementation

#### On AutoFixture
- Automatically mock all dependencies including mockable property and field values (as well as all dependencies for non mockable)
- For `AutoMock` we automatically setup all properties and fields that are mockable to be `AutoMock` as well as having all methods and out params return `AutoMock`,_Once an `AutoMock` always an `AutoMock`_
- Freeze if the type has the `Singleton` or `Scoped` attribute
- Provide the option of injecting a particular constructor argument
- Provide the ability to access any mocks down the object graph
- Verify all mocks in the fixture at once
- Sets up `GetHashCode` and `.Equals` to work correctly even when `Callbase` is false
- Tries to set `Callbase` to `false` and `true` for non delagates, in order to create the object 
            - (sometimes a ctor calls a function that sets up something that can't be mocked, on the other hand sometimes a ctor expects a function to work correctly)

####### TODO
- Get the fixture from the object without having to call it manually
- Make sure that the `Frozen` attribute works with out new system
- Give the option of passing Constructor arugemts via attributes
- Add support for constructors marked with `ForMock` (also it should better remove all readonly warning for it, and disallow newing it up with this constructor, [we might even control it with reflection by restricting getting the type of it... by using our special Stub type])
- Take a list of paths to verify

## Comparison Demo

## Architechture

#### AutoFixture

#### AutoMoqExtensions