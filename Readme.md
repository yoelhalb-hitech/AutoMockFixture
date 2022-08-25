

## Motivation

1. While existing Mocking frameworks are great, they require calling the real constructor which is not always desirable
2. Existing Mocking frameworks require that setting up be very verbose, a sample mock setup would be `mock.Setup(m => It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MyClass>())`
3. Mock frameworks require separate calls for seeting up and for verification
4. In `Moq` the actual object has to be called explictly by doing `.Object` which is annyoing sometimes
5. While AutoFixture helps with a lot of the setups, it does have many shortcomings, including when setting up generic methods, and defaulting to `Callbase = false`, as well as providing real arguments to the mock instead of mocks
6. Other issues with AutoFixture includes the inability to instantiate a ciruclar graph, the inability to provide custom constructor arguments
7. Similarly AutoFixture is missing the ability to require that an object should be provided with mock arguments, which is very useful when for testing in which we many times want the SUT to be real but all arguments to be mocks
8. AutoFixture ignores `internal` members, although they are as valid as `public` memebers and as vital for the code funtions (unlike `private` members which are only needed within the containing class and are supposed to be set by the it)
9. It's very hard to troubleshoot which builder was actually used to build the object in AutoFixture

## Features

#### On Mock
- Setup methods without providing all arguments, just the ones you specifically want
- Setup verification times along with the method setup (currently Moq only suuports to setup that it has to be called using `Verifiable`)
- Provide matchers for generics with constraints
- The AutoMock gives a list of setup and non setup methods
- When `Callbase = false` it does not call the constructors on the object
- An `AutoMock` has an implicit cast to the mocked object, in many cases there is no need to call `.Object` on it
- Has the ability to mock around an existing Target object

####### TODO
- Support verify that an event was raised and it provided the default AutoFxitrue implementation

#### On AutoFixture
- Optionally automatically mock all dependencies including mockable property and field values (as well as all dependencies for non mockable) via the `CreateWithAutoMockDependecies` call or `UnitFixture`
- Can specify specific types that shold always be mocked within the object graph (and without having the object frozen)
- For `AutoMock` we automatically setup all properties and fields that are mockable to be `AutoMock` as well as having all methods and out params return `AutoMock`,_Once an `AutoMock` always an `AutoMock`_
- For `AutoMock` we setup the methods to return unique results per invocation, unlike `AutoFixture.AutoMoq` which returns the same object
- Freeze if the type has the `Singleton` or `Scoped` attribute
- Provide the option of injecting a particular constructor argument by type or by name, and also to remove the customization, (this way multiple calls to `Create` can have different constructor arguments)
- Provide the ability to access any objects and mocks down the object graph
- Verify all mocks in the fixture at once
- Sets up `GetHashCode` and `.Equals` to work correctly even when `Callbase` is false
- Defaults to `Callbase = false` however for Relay obejcts it sets `Callbase = true`
- When `Callbase = true` it does not setup implemented methods (i.e. not in an interface and the method is not abstract)
- On Mock/AutoMock sets up all virtual properties with default values before constructing the object, this way if the constructor needs any property it is ready to go
- Can create recursive object graphs (i.e. classes that their constructor arguments eventually contain the class as an argument), in this case all of them will use the same object

####### TODO
- Get the fixture from the object without having to call it manually
- Make sure that the `Frozen` attribute works with out new system
- Give the option of passing Constructor arugmets via attributes
- Add support for constructors marked with `ForMock` (also it should better remove all readonly warning for it, and disallow newing it up with this constructor, [we might even control it with reflection by restricting getting the type of it... by using our special Stub type])
- Take a list of paths to verify
- Add the ability to request that the `AutoMock` methods should be setup to return the same object for each invocation of the method

## Comparison Demo

## Architechture

#### AutoFixture

The main components in Autofixture are as follows:

##### Vocabulary
- *Specimen*: Refers to the object being built (either the object requested, or a dependency, a property/field or method return object, or out/ref parameter)
- *Builder*: An object that is building a specimen or returns `NoSpeicmen` if it is unable to do so, should implement `ISpecimenBuilder`
- *Request*: Is the request to build a given specimen type, it might be a `Type` object, as well as a `PropertyInfo`/`FieldInfo`/`ParameterInfo` object, or more specific requests such as a `SeededRequest`
- *Relay*: Is a special *Builder* that is used in case all other builders could not create a specimen, by adding it to the `fixture.ResidueCollectors` list
- *Behavior*: An object that specifies custom behavior for the AutoFixture engine, such as what to do if there is a circular graph, should be an implementation of `ISpecimenBuilderTransformation`
- *Specification*: An instance of `IRequestSpecification`, provides an easy unified way of checking if a request is matching a specific criteria
- *Command*: An object that does some actions on a specimen, for example setting up properties, should implemement `ISpecimenCommand`
- *Customization*: An object that custmizaes the creation of specimens, it is either 1) an instance of `ICustomization`, or 2) a Builder passed in to the `fixture.Customizations` list
- *MethodInvoker*: A special *Builder* that is used to invoke the constructor of the *specimen* if there is one
- *Context*: Is the one that handles creating a specimen (via calling `context.Resolve(request)`) by going through all builders registered and checking for the first one that does not return `NoSpecimen` (unless `OmitSpecimen` has been returned)

##### Interfaces
- *ISpecimenBuilder*: The interface for any class that builds a given specimen
- *ISpecimenContext*: The base interface for any `Context`
- *IRequestSpecification*: Is an interface to specify creteria that can be used by many components in the system
- *ISpecimenCommand*: Executes a command on a specimen, typically in via a `Postprocessor`
- *ICustomization*: Is the registration of code that will be used to create or customize building a specimen, typically by adding a custom `ISpecimenBuilder` to the `fixture.Customizations` list
- *ISpecimenBuilderTransformation*: Interface for a *Behavior*, for example the RecusrionBehavior cretaes the `ResucrsionGuard` that controls the recursion level depth
- *IRecursionHandler*: Is the one that actually handles a recursive situation
- *IMethod*: An interface that encapsulates invoking a method
- *IMethodQuery*: Used to search for methods (for example searching all constructors on a type)

##### Specific Classes/Objects
- *NoSpecimen: Is a special indicator that a given builder cannot handle/create a specific request for a specimen, do not use `null` as it might be a legitimate result
- *OmitSpecification*: Is a special indicator that the request for a scpeific object should be omitted (typically if there is recursion involved)
- *RecursionGuard*: Is controlling the build process to monitor if there is any recursion (by seeing if there is the same request while processing the dependencies of the request) and invoking the supplied `IRecursionHandler`
- *Postprocessor*: Is a special builder that executes `Command` after building, optionally only if the constructed specimen matches a specification
        - CAUTION: The specification option on the Processor only determines whether to execute the commands on the specimen and works on the speicmen not the requet, but the specimen is returned regardless of the specification.
                 In order to prevent the builder from building or the specimen from returing you should use `FilteringSpecimenBuilder` around the Postprocessor

##### Utility Classes/Objects
- *FilteringSpecimenBuilder*: Executes a bulder only if the request matches the given `IRequestSpecification`
- *CompositeSpecimenBuilder*: Makes a new `ISpecimenBuilder` that takes a list of `ISpecimenBuilder` objects and goes through all the passed in checking for the first one that does not return `NoSpecimen` (unless `OmitSpecimen` has been returned)
- *CompositeSpecimenCommand*: Makes a new `ISpecimenCommand` that takes a list of `ISpecimenCommand` object and executes each of them
- *FixedBuilder*: Is a `ISpecimenBuilder` that always returns the suppleid object
- *OrRequestSpecification*: Is a `IRequestSpecification` that takes a list of `IRequestSpecification` objects and passes if one of them is satisfied

#### AutoMoqExtensions

##### Tracking
- There are a few types of requests that can be started with, namely `AutoMockRequest`/`AutoMockDirectRequest`/`AutoMockDependenciesRequest`/`NonAutoMockRequest`
- The first requests will then generate other requests based on the depednecies needed or when setting up properties/methods/fields
- Every request implements `ITracker` typically by inheriting `BaseTracker`
- Some requests, especially the starting requests, also implement `IFixtureTracker`,  typically by inheriting `TrackerWithFixture`
- Every request that requests an `AutoMock` implements `IAutoMockRequest`
- Each `ITracker` tracks the children that it creates, as well as returning a path constructed of it's place in the object graph, as well as references to it's parent and to the start tracker

##### AutoMock Requests
- *AutoMockDirectRequest*: Is for a request of type `AutoMock`
- *AutoMockRequest*: Is for a request of Type `T` that we want to create an `AutoMock<T>` for it
- *AutoMockDependenciesRequest*: Is for a request that we want to create an actual (non `AutoMock`) object (typicaly the SUT object), but have all depedencies and property/field valeus mocked, suitable for unit testing
- *NonAutoMockRequest*: Is for a request to create an object and not mock any dpendecies unless specifically specified, suitable for integration tests 

##### Recursion
- *RecursionContext*: Is an implementation of `ISpecimenContext` that keeps track of objects in process of bulding so to be abel to handle recursion
- *MethodInvokerWithRecursion*: First creates the object without calling the constructor and only then creates the depdenecies and calls the constructor, this way we can reference the same object on recursion
- *FreezeRecursionBehavior*: Is a behavior that specifies that in case of recursion it should reuse the original object that is currently being constructed