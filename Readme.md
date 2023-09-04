<h1 align="center">AutuMockFixture</h1>

<div align="center">
  <strong>Extended Mocking and AutoFixture Tool for Automated Testing</strong>
</div>
<br />
<div align="center">
  A framework for creating more sophiscticated mocks maunally or by using an automaitc fixture
  <br /><br />
  It's currently based on <a href="https://github.com/moq/moq4">Moq</a> and <a href="https://github.com/AutoFixture/AutoFixture">AutoFixture</a> but with many added features
</div>


## Table of Contents
- [Motivation](#motivation)
- [Examples And Comparison](#examples-and-comparison)
- [Features](#features)
- [Common Issues](#common-issues)
- [Architechture](#architechture)

## Motivation

While existing Mocking frameworks are great, they have many shortcoming:
   - Require calling the real constructor which is not always desirable
   - Require that setting up be very verbose, a sample mock setup would be `mock.Setup(m => It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MyClass>())`
   - Mock frameworks require separate calls for setting up and for verification
   - In `Moq` the actual object has to be called explictly by doing `.Object` which is annyoing sometimes
   
While AutoFixture helps with a lot of the setups, it does have many shortcomings, for example:
   - When setting up generic methods, and defaulting to `Callbase = false`, as well as providing real arguments to the mock instead of mocks
   - Inability to instantiate a ciruclar graph (i.e. an object `Foo` that has a ctor argument `Boo` that in turn has a ctor arg of `Foo` )
   - Inability to provide custom constructor arguments
   - We might want that a non mock object should be provided with mock arguments, (very useful when for testing when we want the SUT to be real but all arguments to be mocks)
   - AutoFixture ignores `internal` members, (although they are as valid as `public` memebers and as vital for the code funtions)
   - It's very hard to troubleshoot which builder was actually used to build the object in AutoFixture

## Examples and Comparison

Consider the following class:

```cs
class Foo
{
   public Foo(Bar bar)
   {
      var p = new Process(); // Should not arrive here on testing
   }
   
   internal string InternalProp { get; set; }
   
   public virtual void Method1(Bar1 bar1, Bar2 bar2){}
   public virtual void Method2(Bar1 bar1, Bar2 bar2){}
}
```

### On Moq

Here is the test code:

In Moq:

```cs
var mock = new Mock<Foo>(Mock.Of<Bar>()); // Will call the ctor even when callbase is false

mock.Setup(m => m.Method1(It.IsAny<Bar1>(), It.IsAny<Bar2>()));
mock.Setup(m => m.Method2(It.IsAny<Bar1>(), It.IsAny<Bar2>()));

var obj = mock.Object;
obj.Method1(Mock.Of<Bar1>(), Mock.Of<Bar2>());

mock.Verify(m => m.Method1(It.IsAny<Bar1>(), It.IsAny<Bar2>()), Times.Once());
mock.Verify(m => m.Method2(It.IsAny<Bar1>(), It.IsAny<Bar2>()), Times.Never());
```

In AutoMockFixture:

```cs
var mockObj = new AutoMock<Foo>() // Won't call the ctor since callbase is false
               .Setup(nameof(Foo.Method1), Times.Once())
               .Setup(nameof(Foo.Method1), Times.Never()); // We can chain it

mockObj.Method1(Mock.Of<Bar1>(), Mock.Of<Bar2>());

mockObj.Verify();
```

### On Autofixture and Autofixture.AutoMoq

#### Simple Example

Let's say that you want to make a concrete instance of `Foo` but mock all arguments with callbase false, so that you can verify it uses the arguments correctly.

In AutoFixture:

```cs
var fixture = new Fixture();

var bar = fixture.Create<Mock<Bar>>(); // Assuming that you use Autofixture.AutoMoq, otherwise you need to set up all methods manually!!
bar.CallBase = false; // Autofixture.AutoMoq defaults to true

fixture.Freeze(bar.Object);

var foo = fixture.Create<Foo>();

bar.Verify(m => m.SomeMethod(It.IsAny<Bar1>(), It.IsAny<Bar2>()), Times.Once());

Assert.Equals(bar.Object.InternalProp, null); // AutoFixture ignores internal properties and methods
```

In AutoMockFixture:

```cs
var fixture = new UnitFixture(); // Unit fixture is better suited for unit testing by automatically mocking all ctor args and property/field value

var foo = fixture.Create<Foo>();

fixture.On<Bar>().Verify(m => m.SomeMethod(It.IsAny<Bar1>(), It.IsAny<Bar2>()));

Assert.NotEquals(bar.Object.InternalProp, null); // AutoMockFixture sets up internal properties and methods
```

Or you can pass it in as a ctor arg:

```cs
var fixture = new UnitFixture();

var bar  = fixture.CreateAutoMock<Bar>()
               .Setup(nameof(SomeMethod), Times.Once());

fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(bar)));

var foo = fixture.Create<Foo>();
```

Or if you don't have an issue to freeze it then do:

```cs
var fixture = new UnitFixture();

fixture.On<Bar>().Setup(nameof(SomeMethod), Times.Once());

var foo = fixture.Create<Foo>();
```

#### More Complex Example

Let's say we have the following code (this is a contrived example of course):

```cs
class Address 
{ 
   public void SetZip(string zip){};
}

class Customer
{   
   public Customer(Address billingAddress, Address shippingAddress)
   {
      billingAddress.SetZip("11111");
      shippingAddress.SetZip("11111");
   }
}

class Order
{
   public Order(Customer customer){}
}
```

Now we want to create two orders and ensure that the second order has called the `SetZip()` method on the customer BillingAddress.

Here is one way to do it, (NOTE: For help with the path you can use the `AutoMockFixture.AnalyzerAndCodeCompletion` analyzer package):

```cs
var fixture = new UnitFixture();

var order1 = fixture.CreateWithAutoMockDependencies<Order>(callBase = true); // If not callbase it won't call the ctor
var order2 = fixture.CreateWithAutoMockDependencies<Order>(callBase = true); // If not callbase it won't call the ctor

fixture.GetAutoMock<Address>(order2, "..ctor->customer..ctor->billingAddress").Verify(a => a.SetZip("11111")); // If you want only want for order2 and billing
fixture.GetAutoMocks<Address>(order2).Verify(a => a.SetZip("11111")); // To verify for all addresses for order 2
fixture.GetAutoMocks<Address>().Verify(a => a.SetZip("11111")); // To verify for all orders and all addresses
```

Or:

```cs
var fixture = new IntegrationFixture(); // IntegrationFixture is better suited for integration testing as it won't mock anything unless specified via the AutoMockTypeControl setting or the class is abstarct/interface
fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(Address));

var order1 = fixture.Create<Order>();
var order2 = fixture.Create<Order>();

// Verify as above
```

Or in attribute form (currently supporting NUnit but you need to install the corresponding nuget package)

```cs
[NUnit.Framework.Test]
[UnitAutoData] // [UnitAutoData] uses the UnitFixture, while [IntegrationAutoData] uses the IntegrationFixture
public void MyTestMethod([CallBase]Order order1, [CallBase]Order order2, IAutoMockFixture fixture)
{
	// Verify as above
}
```

## Features

#### On Moq
- **Minimal setup**: You can setup methods without providing all arguments, just the ones you specifically want
- **Setup verification times**: Setup verification times along with the method setup (currently Moq only suuports to setup that it has to be called using `Verifiable`)
- **Default Interface Implmentation - no callbase**: Moq is currently using default interface implmenetations when mocking a class that has a default interface implementation and `callbase` is false (in which case it shouldn't)
- **Default Interface Implmentation - events**: Moq is currently not working correctly with default interface implementations of events
- **Default Interface Implmentation - abstract base**: Moq is currently not working correctly when the original interface is abstract and the default implementation is in an inherited interface
- **Interface ReImplmentation - with late binding**: When a class implements an interface then in Moq when creating the `Mock` via late binding (such as generic on the interface) and also calling `As<that interface>` it will not call the original implementation even if the base is not virtual 
- **Generic constrains**: Provide matchers for generics with constraints
- **Ignore ctor**: When `Callbase = false` it does not call any constructors on the object
- **No casting**: An `AutoMock` has an implicit cast to the mocked object, in many cases there is no need to call `.Object` on it
- **Targeted object**: Has the ability to mock around an existing Target object (as in [Castle Project](https://github.com/castleproject/Home))
- **Check if mock**: Can check if an object was created by AutoMock without throwing and capturing as in Moq

####### TODO
- Support verify that an event was raised and it provided the default AutoFixtrue implementation

#### On NUnit
- **Generic test methods**: Supporting now generic test methods via the `TestCaseGenericAttribute` and `TestCaseSourceGenericAttribute`, and for C#11 one can use a generic `TestCaseAttribute<>`

#### On AutoFixture
- **Recursive ctor**: Can create recursive object graphs (i.e. if the ctor of `Foo` requires a `Bar` that in turn requires `Foo`), in this case all of them will use the same object
- **Freeze by attribute on class**: Freeze if the type has the `Singleton` or `Scoped` DI attribute from [our DotNetPowerExtensions framework](https://github.com/yoelhalb-hitech/DotNetPowerExtensions), note that any frozen object will not be garabage collected while the fixture is in scope
- **Provide ctor arguments manually**: Can inject a particular constructor argument by type or by name via the `ConstructorArgumentCustomization` customization, and also providing the ability to remove the customization via `RemoveCustomization()`, (this way multiple calls to `Create` can have different constructor arguments)
- **Trace builder**: Can use `TraceBehavior` to trace the builders use to create the objects as well as all builders that have been attempted
- **Dispose**: Can use `.Dispose()` to automatically to dispose all disposable created objects and disposable customizations
- **Can register a derived class to replace the original request**: Either replace a concrete class with a subclass (via `SubClassCustomization` or `SubClassTransformCustomization`) or an open generic class (via `SubClassOpenGenericCustomization` or `SubClassTransformCustomization`), can be useful to replace for example `DbSet<>` with a dervied class for any concrete instance of `DbSet<>`
    
	*NOTE*: For `SubClassOpenGenericCustomization` you should use any generic parameter and it will be ignored
	
- **Access object in graph**: Provide the ability to access any objects and mocks down the object graph by type (for mocks) or by path, it alos provides the list of paths if needed
    
    *CAUTION*: Since return values of method calls and some property access might be created lazily then if the path/mock doesn't exist it won't show up
    
    *WORKAROUND*: For mocks we can freeze the type and then create it directly from the fixture and use it, also `GetAutoMocks` and `GetAutoMock` have an overload that does it automatically, as well as `For` and `Object`
        
    *CAUTION*: `Freeze` won't freeze existing objects, so if writing this workaround directly it should NOT be used if it is already in the mock

#### On AutoFixture.AutoMoq
- **Attributes for Moq fixtures**: Use the `UnitAutoData` or `IntegrationAutoData` attribtues on the method to get passed in a `UnitFixture` or `IntegrationFixture` respectively (currently only available for NUnit), will also dispose of the fixture after the test run
- **Attribute support for fixture customization**: Use the `UnitAutoData` or `IntegrationAutoData` attribtues also have a generic version that supports passing in an ICustomization to customize the fixture, requires the customization to have a defualt ctor (currently only available for NUnit)
- **SUT with mock dependecies**: Can automatically mock all dependencies including mockable property and field values (as well as all dependencies for non mockable) via the `CreateWithAutoMockDependecies` call or use `UnitFixture` (or via attributes when using AutoData to inject the arguments in the test method)
- **Force Mock**: Can specifically demend that an object should be a mock via the `CreateAutoMock` call (or via attributes when using AutoData to inject the arguments in the test method)
- **Force Mock by Type**: Can specifically demend that an object should be always mocked or not mocked via the `AutoMockTypeControl` on the fixture or the `Create` call
- **List setup**: The AutoMock gives a list of methods and properties that have been setup with Moq and also non setup methods/properties and the reason why it hasn't
- Can specify specific types that should always be mocked within the object graph (without having the objects frozen)
- **Setup dependencies**: For `AutoMock` we automatically setup all properties and fields that are mockable to be `AutoMock` as well as having all methods and out params return `AutoMock`,_Once an `AutoMock` always an `AutoMock`_ (unless you specify via `AutoMockTypeControl`)
- **Unique method return**: For mocks there is the option to have the methods setup to return unique results per invocation (by passing in `MethodSetupTypes.LazyDifferent` to the fixture)
- **Eager vs Lazy**: For mocks by default the return value for methods is created when first time called (to optimize the generation of the mock), this can be changed by passing in `MethodSetupTypes.Eager` to the fixture
- **Verify fixture**: Verify all mocks in the fixture at once
- **Explicit interface implementation**: Sets up explictly implemented interface members when `Callbase` is false
- **Can register a derived class to replace the original request**: Either replace a concrete class with a subclass (via `SubClassCustomization` or `SubClassTransformCustomization`) or an open generic class (via `SubClassOpenGenericCustomization` or `SubClassTransformCustomization`), can be useful to replace for example `DbSet<>` with a dervied class for any concrete instance of `DbSet<>`

- Sets up `GetHashCode` and `.Equals` to work correctly even when `Callbase` is false
- Defaults to `Callbase = false` however for Relay obejcts it sets `Callbase = true`, but there is an option to pass in the `Createxxx` calls to change the default for non relays
- When `Callbase = true` it does not setup implemented methods (i.e. not in an interface and the method is not abstract)

####### TODO
- Get the fixture from the object without having to call it manually
- Give the option of passing Constructor arugmets via attributes
- Add support for constructors marked with `ForMock` (also it should better remove all readonly warning for it, and disallow newing it up with this constructor, [we might even control it with reflection by restricting getting the type of it... by using our special Stub type])
- Take a list of paths to verify
- Do we need to implement manually non public members in an interface?


## Common Issues
#### Main object not found
- Ensure that the obejct is not garbage collected
- Ensure that the .Equals is not overriden in a way that can cuase it to happen

#### Object not garbage collected
- Ensure that the object is not Frozen (either explictly or implectly by the `Singleton` or `Scoped` attribute)

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
- *NoSpecimen*: Is a special indicator that a given builder cannot handle/create a specific request for a specimen, do not use `null` as it might be a legitimate result
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

#### AutoMockFixture

##### Fixtures
- *UnitFixture*: A fixture better suitbale for unit testing, it will by default (when generating the object calling `Create()`) will try to generate the object and mock all dependencies (such as ctor arguments and property/field values)
- *IntgerationFixture*: A fixture better suitbale for unit testing, it will by default (when generating the object calling `Create()`) will not mock any dependencies (such as ctor arguments and property/field values) unless explictly specified or it is impossible to generatere another way (i.e. interfaces/abstract classes)
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
