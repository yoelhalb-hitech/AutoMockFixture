

## Motivation

## Features

#### On Mock
- Setup methods without providing al arguments, jsut the ones you specifically want
- Setup verification times along with the method setup (currently Moq only suuports to setup that it has to be called using `Verifiable`)
- Provide matchers for generics with constraints


#### On AutoFixture
- Automatically mock all dependencies including depdnecies for property and field values
- For `AutoMock` we automatically setup all properties and fields that are mockable to be `AutoMock`, _Once an `AutoMock` always an `AutoMock`_
- Provide the option of injecting a partiuclar constructor argument
- Provide the ability to access and objects mocks down the object graph
- Get the fixture from the object without having to call it manually
- Verify all mocks in the fixture at once
- Sets up `GetHashCode` and `.Equals` to work correctly even when `Callbase` is false
- Tries to set `Callbase` to `false` and `true` for non delagates, in order to create the object 
            - (sometimes a ctor calls a function that sets up something that can't be mocked, on the other hand sometimes a ctor expects a function to work correctly)

## Comparison Demo

## Architechture

#### AutoFixture

#### AutoMoqExtensions