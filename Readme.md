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
<br />

$${\color{red}Please \space note,  \space there   \space are   \space breaking   \space changes   \space in   \space  version  \space 8.0 }$$


## Table of Contents
- [Motivation](#motivation)
- [Examples And Comparison](#examples-and-comparison)
- [Usage](#usage) $${\color{red}Please \space read  \space carefully   \space as \space our   \space options   \space have   \space  changed... }$$
	- [Nuget Packages]
	- [Supported Technologies]
	- [AutoMock](#automock)
	- [NUnit Generic Test Cases]
	- [Fixtures]
	- [UnitFixture]
	- [IntegrationFixture]
	- [Freeze]
	- [SubClassCustomization]
	- [Method Setup Type]
- [Features over AutoFixture and Moq](#features-over-autofixture-and-moq)
- [Common Issues](#common-issues)
- [Architechture](#architechture)

# AutoMockFixture

AutoMockFixture is an extended mocking and AutoFixture tool for automated testing, enhancing the capabilities of Moq and AutoFixture with additional features for more sophisticated test scenarios.

## Table of Contents
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Motivation](#motivation)
- [Features](#features)
- [Usage](#usage)
- [Examples](#examples)
- [Comparison](#comparison)
- [Architecture](#architecture)
- [Common Issues](#common-issues)
- [Contributing](#contributing)
- [License](#license)
- [Version History](#version-history)

## Installation

Install AutoMockFixture via NuGet:

Install-Package AutoMockFixture.NUnit3.Moq4

Or if not using the attribute based features:

Install-Package AutoMockFixture.Moq4

## Quick Start

### Basic Usage

To get started, create an instance of `AutoMockFixture` or its attribute counterpart:

```csharp
var fixture = new UnitFixture();

var foo = fixture.Create<Foo>();
```

### Attribute-based Usage

AutoMockFixture provides attribute-based testing for easier setup:

```csharp
[NUnit.Framework.Test]
[UnitAutoData] // [UnitAutoData] uses the UnitFixture, while [IntegrationAutoData] uses the IntegrationFixture
public void MyTestMethod([CallBase]Order order1, [CallBase]Order order2, IAutoMockFixture fixture)
{
	// Verify as above
}
```



## Motivation

AutoMockFixture addresses limitations in existing mocking frameworks and AutoFixture:

- More flexible constructor handling
- Less verbose mock setup
- Combined setup and verification calls
- Improved handling of circular object graphs
- Better support for internal members

While existing Mocking frameworks are great, they have many shortcoming:
   - Require calling the real constructor which is not always desirable
   - Require that setting up be very verbose, a sample mock setup would be `mock.Setup(m => It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MyClass>())`
   - Mock frameworks require separate calls for setting up and for verification
   - In `Moq` the actual object has to be called explictly by doing `.Object` which is annyoing sometimes

While AutoFixture helps with a lot of the setups, it does have many shortcomings, for example:
   - When setting up generic methods, and defaulting to `CallBase = false`, as well as providing real arguments to the mock instead of mocks
   - Inability to instantiate a ciruclar graph (i.e. an object `Foo` that has a ctor argument `Boo` that in turn has a ctor arg of `Foo` )
   - Inability to provide custom constructor arguments
   - We might want that a non mock object should be provided with mock arguments, (very useful when for testing when we want the SUT to be real but all arguments to be mocks)
   - AutoFixture ignores `internal` members, (although they are as valid as `public` memebers and as vital for the code funtions)
   - It's very hard to troubleshoot which builder was actually used to build the object in AutoFixture

## Features

- **Recursive object creation**: Handle circular dependencies in constructors
- **Flexible mocking**: Easily mock dependencies and control mocking behavior
- **Attribute-based customization**: Use attributes for fine-grained control
- **Enhanced NUnit integration**: Improved support for NUnit, including generic test methods
- **Tracing and debugging**: Better insight into object creation process

[More detailed feature list](#features-details)

## Examples and Comparison

| Feature | AutoMockFixture | Moq | AutoFixture |
|---------|-----------------|-----|-------------|
| Circular dependencies | ✅ | ❌ | ❌ |
| Mock internal members | ✅ | ❌ | ❌ |
| Automatically mock dependencies | ✅ | ❌ | ❌ |
| Combined setup and verification | ✅ | ❌ | ❌ |
| Flexible constructor arguments | ✅ | ❌ | ✅ |
| Not calling the mock base ctor (for non CallBase) | ✅ | ❌ | ❌ |
| Use DI services dependecies for interfaces/abstract classes | ✅ | ❌ | ❌ |
| Retrieve objects from Fixture via path | ✅ | ❌ | ❌ |
| Retrieve mocks from Fixture via type | ✅ | ❌ | ❌ |
| Setup and verify mocks via path | ✅ | ❌ | ❌ |
| Setup and verify mocks via MethodInfo | ✅ | ❌ | ❌ |
| Chain setup and verification | ✅ | ❌ | ❌ |
| Setup Mock to automatically generate different result for each mock method call | ✅ | ❌ | ❌ |
| Verify all mocks in a fixture at once | ✅ | ❌ | ❌ |
| Keep track of created mocks | ✅ | ❌ | ❌ |
| Keep track of methods not setup and why | ✅ | ❌ | ❌ |

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
   public virtual int Method2(int intArg, Bar2 bar2){}
   protected virtual int ProtectedMethod(Bar1 bar1, Bar2 bar2){}
}
```

### On Moq

Here is the test code:

In Moq:

```cs
var mock = new Mock<Foo>(Mock.Of<Bar>()); // Will call the ctor even when callBase is false

mock.Setup(m => m.Method1(It.IsAny<Bar1>(), It.IsAny<Bar2>()));
mock.Setup(m => m.Method2(It.Is<int>(b => b == 4), It.IsAny<Bar2>())).Returns(10);
mock.Protected().Setup<int>("ProtectedMethod", ItExpr.IsAny<Bar1>(), ItExpr.IsAny<Bar2>())).Returns(10);

var obj = mock.Object;
obj.Method1(Mock.Of<Bar1>(), Mock.Of<Bar2>());

mock.Verify(m => m.Method1(It.IsAny<Bar1>(), It.IsAny<Bar2>()), Times.Once());
mock.Verify(m => m.Method2(It.Is<int>(b => b == 4), It.IsAny<Bar2>()), Times.Never());
mock.Protected().Verify("ProtectedMethod", Times.Never(), ItExpr.IsAny<Bar1>(), ItExpr.IsAny<Bar2>());
```

In AutoMockFixture:

```cs
var mockObj = new AutoMock<Foo>() // Won't call the ctor since callBase is false
               .Setup(nameof(Foo.Method1), Times.Once())
               .Setup(nameof(Foo.Method2), new { intArg = 4 }, 10, Times.Never()) // We can chain it, note the proeprty has to match the argument name, CAUTION: This only works so far with primitive types
               .Setup("ProtectedMethod", new {}, 10, Times.Never()); // We can also do it for protected without all the ceremony

mockObj.Method1(AutoMock.Of<Bar1>(), AutoMock.Of<Bar2>());

mockObj.Verify();
```

### On Autofixture and Autofixture.AutoMoq

#### Simple Example

Let's say that you want to make a concrete instance of `Foo` but mock all arguments with callBase false, so that you can verify it uses the arguments correctly.

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

var order1 = fixture.CreateWithAutoMockDependencies<Order>(callBase: true); // If not callBase it won't call the ctor
var order2 = fixture.CreateWithAutoMockDependencies<Order>(callBase: true); // If not callBase it won't call the ctor

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
## Usage
- [Nuget Packages]
- [Supported Technologies]
- [AutoMock](#automock)
- [NUnit Generic Test Cases]
- [Fixtures]
- [UnitFixture]
- [IntegrationFixture]
- [Freeze]
- [SubClassCustomization]
- [Method Setup Type]

### Nuget Packages

The following Nuget packages are available:
	- [AutoMockFixture.NUnit3.Moq4](https://www.nuget.org/packages/AutoMockFixture.NUnit3.Moq4): This is the main na drecommended package for anyone using NUnit and would like to make use of the attribute annotations
	- [AutoMockFixture.Moq4](https://www.nuget.org/packages/AutoMockFixture.Moq4): This is the main package for the non attribute based usage, if you are using however the [attribute based package](https://www.nuget.org/packages/AutoMockFixture.NUnit3.Moq4) then this package will be atuomatically installed
	- [AutoMockFixture.AnalyzerAndCodeCompletion](https://www.nuget.org/packages/AutoMockFixture.AnalyzerAndCodeCompletion): A Rolsyn analyzer to provide intellisense for using the path system to retrieve objects from the Fixture

Other supporting packages are:
	- [AutoMockFixture](https://www.nuget.org/packages/AutoMockFixture): This is used by the other Nuget packages, no need to install it manually
	- [AutoMockFixture.NUnit3](https://www.nuget.org/packages/AutoMockFixture.NUnit3): This is used by the other Nuget packages, typically there is no need to install it manually (unless you do not install the other packages but want the generic NUnit attributes for `TestCaseData`)

### Supported Technologies

- Currently only .Net 5 and up is supported, we don't support the legacy .Net Framework
- The mocking system currently supported is using Moq behind the scenes
- The attribute based features are currently only available for NUnit

### AutoMock

AutoMock is currently based on Moq, but has been modified to support specific features:
- Bug fixes: Many bugs especially in regards to interfaces, explicit interface implementation, default interface implementation have all been fixed
- Non CallBase Ctor: When `CallBase == false` then the base ctor is not being called, unlike Moq that always calls the Base ctor, CATUION: Keep in mind that this also means that property/field initializers won't be called (as internally they are actually part of the ctor)
- Implements the `IAutoMock` interface
- The following options are available to get the mocked object:
	- The Moq `.Object`
	- `.GetMocked`()
	- An implicit or explicit conversion to the mocked object type
	- AutoMock.Of<T>() will create a new AutoMock and return the mocked object (if you then want to set it up use `AutoMock.Get()` )
- To get the `AutoMock<>` from the object use `AutoMock.Get()` (note that unlike Moq if the type is an AutoMock but not the provided type <T> then the error message will indicate that)
- You can check if it is an `AutoMock` via `AutoMock.IsAutoMock()`
- Setup for any argument by passing `default` instead of the verbose `It.IsAny<T>()` (example: `mock.Setup(x => x.MyMethod(default))`)
	- CAUTION: Note that if you want to setup for the default value only (such as `null` `0` or `DateTime.MinValue`) you should NOT pass `default` and also NOT the actual default value (such as `null` `0` or `DateTime.MinValue`) instead use the special `ItIs.DefaultValue<T>()` method
	- CAUTION: For boolean values, if you want to set up only for `false` values, do not pass `false` (as it the default value and thus will setup for everything), use the `ItIs.False()` method instead
	- CAUTION: For nullable value types specifying the default for the underlying value type has the same effect (i.e. both `default(int)` and `default(int?)` will setup everything), use `ItIs.DefaultValue<int>()` method instead
		- Be carefull to pass the inner value type instead of the nullable type, as this will make a difference on whether it sets up null or 0 etc.
		- Again for bool you can use `ItIs.False()`
	- The complimentry [Analyzer Nuget Package](https://www.nuget.org/packages/AutoMockFixture.AnalyzerAndCodeCompletion) will warn if using default values like 0 or null directly in the setup

For setup and verification there are the following options:
	- `.Setup()`/`Verify()` just like in Moq
	- There is also the option to setup for verification in the Setup methods with the specific number of times(unlike Moq which only has the option to setup as `Verifiable` but not the specific number of times)
		- NOTE: You will have to call `.Verify()` on the mock to trigger the verification
		- If the mock was created from a fixture then you can call .Verify() on the fixture
	- Instead of using `It.IsAny<T>()` you can just pass the keyword `default` or the defualt value for the type
		- CAUTION: For the literal null or default value please use `It.IsNull()` or `It.Is()`, otherwise it would be intepereted as `It.IsAny()`
	- You can also setup the method just via a string of the name or a `MethodInfo` representing the method
		- To provide filtering by arguments, pass an anonymous obejct as the filtering data
			- The name of each property should match the respective argument name and should be a valid value for the type
			- Any argument not provided as property is considered to be implicit `It.IsAny()`
			- An empty anonymous or null means that it will always match
		- To provide a return value you should pass it directly as the reuslt argument
			- CAUTION: When providing a result you have to provide also a value for the filtering, otherwise the system will think that you provided a value for argument filtering

Sample Code:

```cs
var mockObj = new AutoMock<Foo>() // Won't call the ctor since callBase is false
               .Setup(nameof(Foo.Method1), Times.Once())
               .Setup(nameof(Foo.Method2), new { intArg = 4 }, 10, Times.Never()) // We can chain it, note the proeprty has to match the argument name, CAUTION: This only works so far with primitive types
               .Setup("ProtectedMethod", new {}, 10, Times.Never()); // We can also do it for protected without all the ceremony

mockObj.Method1(AutoMock.Of<Bar1>(), AutoMock.Of<Bar2>());

mockObj.Verify();
```


There are basically two types of Fixtures:
- `UnitFixture`: Geared for Unit tests - Mocks all ctor dependencies and settable properties/fields beside the SUT
- `IntegrationFixture`: Geared towards Integration tests and will not mock be defualt anything unless it is not createable (i.e. abstarct or interface)

There are also the corraspanding attributes for annotating a test function for automatically injecting the parameters:
- `UnitAutoDataAttribute`: Uses the `UnitFixture`
- `IntegrationAutoDataAttribute`: Uses the `IntegrationFixture`

Besdies the basic `Create` method comparable to AutoFixture, they also support specialized methods (and comparable attributes)
- `CreateWithAutoMockDependecies`: Create a SUT with all ctor args and settable properties/fields (aka _dependencies_) mocked (like the `UnitFixture` `Create` method)
- `CreateAutoMock`: Create a Mocked SUT instead of a real SUT (the depdencies will be mocked if the fixture is a `UnitFixture` but not if it is an `IntegrationFixture`)
- `CreateNonAutoMock`: Will create a full object graph without mocking unless needed (like the `IntegrationFixture` `Create` method)

The comparable attributes are:
- `AutoMockDependenciesAttribute`: Applied to a parameter will behave as `CreateWithAutoMockDependecies`
- `AutoMockAttribute`: Applied to a parameter will behave as `CreateAutoMock`
- `NonAutoMockAttribute`: Applied to a parameter will behave as `CreateNonAutoMock`

It is also possible to control the CallBase behaviour of the mocks created, as follows:
- The default for `CreateWithAutoMockDependecies` or the `UnitFixture` `Create` methods is to callbase if the SUT is mocked but not for any ctor args and settable properties/fields
- The default for the `UnitFixture` `CreateAutoMock` method is to never callbase
- The default for `CreateNonAutoMock` and the `IntegrationFixture` `Create` and `CreateAutoMock`  methods is to always callbase
- There is no direct equivalent on the `IntegrationFixture` to the `UnitFixture.CreateAutoMock` method, however you can get the same results by calling `CreateWithAutoMockDependecies<AutoMock<T>>` and explictly specifying `callBase: false`
- You can explictly specify the callbase true/false behaviour either on the fixture level or on the indivdual request level, in which case the callbase will always follow the specified callbase (currently there is no way to bring back the default behaviour when it has been set on the fixture level, however you can reset the fixtrue level `CallBase` property to null)
- On the fixture level there is a property `CallBase`
- Each `Create` variant has overloads that accept a `callBase` argument
- The `UnitAutoDataAttribute` and `IntegrationAutoDataAttribute` have ctor overloads accepting `callBase`
- There is a special `CallBaseAttribute` that can be applied on a parameter level, note that the default for this attribute is `true`, however there are ctor overloads to set it otherwise

Similarly there is a specific control system to specify explictly types that should always be mocked or should never be mocked:
- On the fixture level there is a property `AutoMockTypeControl`
- Each `Create` variant has overloads that accept a `AutoMockTypeControl` argument
- There is a special `AutoMockTypeControlAttribute` that can be applied on a parameter level

### Unit and Integration Fixture and their differences

- `Create` method (or no attribute specified on parameters): Will behave as `CreateWithAutoMockDependecies` in `UnitFixture`, but like `CreateNotAutoMock` in the `IntegrationFixture`
- `CreateAutoMock` (or the `AutoMockAttribute`): will behave on the `IntegrationFixture` as if `CreateNonAutoMock<AutoMock<T>>`, but on the `UnitFixture` it will behave `CreateWithAutoMockDependecies<AutoMock<T>>` with explicit `callBase: false`
- The `Freeze` method: Will behave as the corrapanding `Create` method
- The `FreezeAttribute`: Will consider the other attributes applied to the specific parameter

The integration fixture (besides defaulting to not creating mocks and to callbase when it does create) it also has another feature:
- Match up automatically services via the [SequelPay.DotNetPwerExtensions](#https://github.com/yoelhalb-hitech/DotNetPowerExtensions/blob/main/DotNetPowerExtensions/README.md) DI attributes `TransientAttribute`/`ScopedAttribute`/`SingletonAttribute`
- This behaviuor can be turned off on the Fixture by setting the `AutoTransformBySericeAttributes` property to `false` (before creating the object...)
- Similarly this behaviour can be turned on for `UntiFixture` by setting `AutoTransformBySericeAttributes` to `true` on the fixture

CAUTION: Whenever we say that something can be turned on or off in the fixture, it only referes to the objects created after setting the relevent property, it won't currently change objects created before

The following is a summary

### Freeze
- For the general approach to mocking and callbase [see above](#unit-and-integration-fixture-and-their-differences)
- For the attributes based fixtures (`UnitAutoDataAttribute` and `IntegrationAutoDataAttribute`) there are ovelaods accepting a list of types to freeze as well as generic overloads to supply it via the C# 11 generic attribute feature
- Freeze will also automatically freeze/reuse subtypes for any base type specified, and also for the corraspanding AutoMock types
- Freeze will not freeze the non automock type when an AutoMock type is specified

- All classes marked with the `DotNetPowerExtensions` `[Singleton]` attribute will automatically be frozen

- If you don't need the return type then you can use `.JustFreeze<TType>()` instead

NOTE: There is an overload on `.Freeze()` and `.FreezeAsync()` to spceify the callBase for the returned object, it does however NOT effect the freeze behaviour and objects of the given type will be frozen regardless of their callBase status
For the attribute version of `[Frozen]` just use the `[CallBase]` attribtue to specify the callbase behaviour

Code example:

```cs
class BaseBase {}
class Base : BaseBase{}
class Sub : Base {}

[Test, UnitAutoData]
public void Test_FreezeBy_BaseClass([Frozen] Base b, Base b1, BaseBase bb, BaseBase bb1, Sub s, Sub s1, AutoMock<Base> mb, AutoMock<Base> mb1, AutoMock<Sub> ms, AutoMock<Sub> ms1)
{
	Assert.Equals(b1, b); // Explictly frozen
	Assert.Equals(s1, s); // Although `Sub` was never explictly frozen

	Assert.Equals(mb1, mb); // Although `AutoMock<Base>` was never explictly frozen
	Assert.Equals(ms1, ms); // Although `AutoMock<Sub>` was never explictly frozen

	Assert.NotEquals(bb, bb1); // Base classes are not frozen via a sub class, only the opposite is true
}

[Test, UnitAutoData]
public void Test_FreezeBy_AutoMockBaseClass([Frozen] AutoMock<Base> mb, AutoMock<Base> mb1, AutoMock<Sub> ms, AutoMock<Sub> ms1, Base b, Base b1, Sub s, Sub s1)
{
	Assert.Equals(mb1, mb); // Explictly frozen
	Assert.Equals(ms1, ms); // Although `AutoMock<Sub>` was never explictly frozen

	Assert.NotEquals(b1, b); // When AutoMock<Base> is frozen then Base is not frozen, unlike when Base is frozen
	Assert.NotEquals(s1, s);
}
```

#### Caution when dealing with frozen objects

- *Value Types*: Value types (in particular primitive types) will only be equal in value but will not refer to the exact same object
- *Order*: When creating an object via `.Freeze()` or `[Frozen]` the returned frozen object will not neceserily be the same as another object of the same type, beacuse of the following limitations:
	- `.Freeze()` or `.JustFreeze()` only applies from this point onwards, so any objects created before will be different
		Sample code:
		```cs
			var obj1 = fixture.Create<Test>();
			var obj2 = fixture.Freeze<Test>();
			var obj3 = fixture.Create<Test>();
			var obj4 = fixture.Create<Test>();
			// obj1 will be different while obj2,obj3,obj4 will all be the same object
			
			[Test, UnitAutoData]
			public void TestMethod(Test test1, [Frozen] Test test2. Test test3){} // test2 and test3 will be the same object while test1 will be different 
		```
- *`.Freeze()` and `[Frozen]` returns a SUT*: Objects differing either by them being mocked objects or not, callBase or not, mock dependecies or not, will all be different objects
	- *UnitFixture*: This is very important as for a `UnitFixture` the default of a SUT is to not be mocked and the default for it is non callbase, [as in the comparison table below](#comparison-table)
	Sample Code
	```cs
		var frozen = unitFixture.Freeze<Inner>(); // Returns a non automock with callBase=true, as it is the SUT
		var outer = unitFixture.Create<Outer>(); // `outer.Inner != frozen` as `outer.Inner` is not a SUT and therefore will NOT be the same as `frozen` as it is an autoMock and callBase=false, NOTE: only applies to the UnitFixture
		var inner = unitFixture.Create<Inner>(); // `inner == frozen` as both are a SUT
		var mock = unitFixture.CreateAutoMock<Inner>(); // `mock == outer.Inner` but `mock != frozen` as `.CreateAutoMock()` on a `UnitFixture` defaults to callBase=false, NOTE: only applies to the UnitFixture
		var frozenMock = unitFixture.Freeze<AutoMock<Inner>>(callBase: false).Object; // `frozenMock == outer.Inner` but `frozenMock != frozen`, CAUTION: If this is the only `.Freeze()` it will only freeze automocks, NOTE: only applies to the UnitFixture
		var frozenMockNotCallbase = unitFixture.Freeze<AutoMock<Inner>>().Object; // `frozenMockNotCallbase != outer.Inner` and also `frozenMock != frozen` as `.Freeze()` is a SUT and therefore defaults to callBase=true, CAUTION: If this is the only `.Freeze()` it will only freeze automocks, NOTE: only applies to the UnitFixture
	```
	Or in attribute form 
	```cs
			[Test, UnitAutoData]
			public void TestMethod([Frozen] Inner inner, [Frozen, AutoMock] Innner innerWithMockAttribute, 
				[Frozen] AutoMock<Innner> mock, [Frozen, CallBase(false)] AutoMock<Innner> mockNoCallBase, Outer outer)
			{
				Assert.Equals(innerWithMockAttribute, outer.Inner); // [AutoMock] is the same as `.CreateAutoMock()` which on a `UnitFixture` or `[UnitAutoData]` defaults to callBase=false which is the same as the default for non SUT
				Assert.Equals(mockNoCallBase.Object, outer.Inner); // Has `[CallBase(false)]` and is of type `AutoMock` so it matches the non SUT
				
				Assert.NotEquals(inner, outer.Inner); // `inner` is a SUT since it was requested directly and therefore it is not AutoMock and also callBase=true
				Assert.NotEquals(mock.Object, outer.Inner); // `mock` is a SUT since it was requested directly, and while it is an autoMock it is still callBase=true, unlike non SUTs which are callBase=false
			}
	```
	- *IntegrationFixture*: This code will not work however in an `IntegrationFixture` as the defaults for them are completely different, but in an `IntegrationFixture` the frozen will by default match the inner
	Sample Code for `IntegrationFixture`
	```cs
		var frozen = integrationFixture.Freeze<Inner>(); // Returns a non automock with callBase=true
		var outer = integrationFixture.Create<Outer>(); // `outer.Inner == frozen` as `outer.Inner` (although not a SUT) will setill not be an autoMock and also callBase=true by default, NOTE: only applies to the IntegrationFixture
		var inner = integrationFixture.Create<Inner>(); // `inner == frozen` as both are a SUT
		var mock = integrationFixture.CreateAutoMock<Inner>(); // `mock != outer.Inner` as `outer.Inner` is not a mock, NOTE: only applies to the UnitFixture
		var frozenMock = integrationFixture.Freeze<AutoMock<Inner>>(callBase: false).Object; // `frozenMock != outer.Inner` since it is a mock and also callBase=false, CAUTION: If this is the only `.Freeze()` it will only freeze automocks, NOTE: only applies to the UnitFixture
		var frozenMockNotCallbase = integrationFixture.Freeze<AutoMock<Inner>>().Object; // `frozenMockNotCallbase != outer.Inner` as this is a mock (but `frozenMockNotCallbase == mock`), CAUTION: If this is the only `.Freeze()` it will only freeze automocks, NOTE: only applies to the UnitFixture
	```
	Or in attribute form 
	```cs
			[Test, IntegrationAutoData]
			public void TestMethod([Frozen] Inner inner, [Frozen, AutoMock] Innner innerWithMockAttribute, 
				[Frozen] AutoMock<Innner> mock, [Frozen, CallBase(false)] AutoMock<Innner> mockNoCallBase, Outer outer)
			{
				Assert.NotEquals(innerWithMockAttribute, outer.Inner); // `innerWithMockAttribute` is a mock but `outer.Inner` is not
				Assert.NotEquals(mockNoCallBase.Object, outer.Inner); // `mockNoCallBase.Object` is a mock and also callBase=false but `outer.Inner` is not a mock and callBase=true
				
				Assert.Equals(inner, outer.Inner);
				Assert.NotEquals(mock.Object, outer.Inner); // `innerWithMockAttribute` is a mock but `outer.Inner` is not
				
				Assert.Equals(mock.Object, innerWithMockAttribute); // Both are mocks and both default to callBase=true in the IntegrationFixture
			}
	```
	
### Accessing inner properties and ctor args

When using objects we want sometimes to access specific properties (or sub properties) or fields, or ctor args, or method return values etc.
This can be either to set up or to test and verify.

Here is a sample:
```cs
class Inner 
{
	public int Test { get; set; }
}
class Outer 
{
	public Inner Inner { get; set; }
}

var outer = fixture.Create<Outer>();
// We need to access to Outer.Inner...
```

This can be done in multiple ways:

#### Accessing the sub object directly

You can use the traditional way of accessing the sub object directly, (just as you would normally do when coding)

In the above example that would be equivalent to `outer.Inner`.

This however has many limitations, such as:
	- Not all properties/fields/methods are public
	- Even public properties can be write only with no simple way to read them
	- Ctor args are be default not public and in order to access them you must vapture them in the class and expose them
	- Calling methods in order to setup their return value can interfere with tracking call counterpart
	- Sometimes we want access to all object of the same type which is very tedious to find all
	- Sometimes we need access to an object deep down the chain

#### Via freezing

Freezing the type of the required sub object and then using the frozen object to access it as it will be the same as the sub object.

Continuing with the above example that would be something like (on an integration fixture):

```cs
var frozenInner = fixture.Freeze<Inner>(); //CAUTION: This has to be BEFORE creating the outer, as Freeze only applies from this point onwards
// Or for Integration fixture it should be fixture.Freeze<Inner>();

var outer = fixture.Create<Outer>();

// frozenInner should now be the same as outer.Inner, PROVIDED THE CONDITIONS BELOW ARE MET
```

Or for a unit fixture (method 1), read on for why this is needed

```cs
fixture.JustFreeze<Inner>(); //CAUTION: This has to be BEFORE creating the outer, as Freeze only applies from this point onwards
var frozenInner = fixture.CreateAutoMock<Inner>();

var outer = fixture.Create<Outer>();

// frozenInner should now be the same as outer.Inner, PROVIDED THE CONDITIONS BELOW ARE MET
```

Unit fixture (method 2), note that as specified in [The section on freezing above](#freze) this will not freeze any `Inner` that is not an autoMock

```cs
var frozenInner = fixture.Freeze<AutoMock<Inner>>(false).Object; //CAUTION: This has to be BEFORE creating the outer, as Freeze only applies from this point onwards

var outer = fixture.Create<Outer>();

// frozenInner should now be the same as outer.Inner, PROVIDED THE CONDITIONS BELOW ARE MET
```

*Limitations*

However you should be cautious of the following:
	- The object returned by `.Freeze()` or `[Frozen]` might not be the same as the sub object (especially for a `UnitFixture`) since `.Freeze()` and `[Frozen]` return a SUT
		- For more details [see the above section on freezing](#caution-when-dealing-with-frozen-objects)
	- Any objects before `.Freeze()` or `[Frozen]` will be different, only from this point onwards it will be the same [checkout the section on freezing for more details](#caution-when-dealing-with-frozen-objects)
	- All objects and sub objects from the point of freezing will be the same which might not be what you want

#### Via the path system

You can get a specific object [via the path system](#get-by-path)

Note however that the path system is not yet battle tested and might buggy.

It is however limited to a single object at a time which might not be what you want.
It also has limitations on if the object has to be created already, see below.

It also has no built in compile time checking and no intellisense for the paths (especially as the structure of the code is subject to change), while we do provide an anlyzer for compile time checking and intellisense it is not yet battle tested and is still buggy.

#### 

### Comparison Table

| CB Specified  | Method | Fixture |      Mock SUT     |  CB SUT (when Mock) | Mock Dependencies | CB Dependencies (when Mock) |
|:-------------:|:------:|------|:-----------------:|:-------:|:-----------------:|:----------------:|
|  No/null | **Create** | **UnitFixture** |  ❌  | ✅  | ✅ | ❌  |
|  No/null | **Create** | **IntegrationFixture** |  ❌  | ✅ | ❌  | ✅ |
|  No/null | **CreateAutoMock** |  **UnitFixture** |   ✅ | ❌  | ✅ | ❌  |
|  No/null | **CreateAutoMock** |  **IntegrationFixture** |   ✅ | ✅ | ❌  | ✅ |
|  True | **Create** | **UnitFixture** |  ❌  | ✅ | ✅ | ✅ |
|  True | **Create** | **IntegrationFixture** |  ❌  | :✅ | ❌  | ✅ |
|  True | **CreateAutoMock** |  **UnitFixture** |  ✅ | ✅ | ✅ | ✅
|  True | **CreateAutoMock** |  **IntegrationFixture** | ✅ | ✅ | ❌  | ✅ | ✅ |
|  False (T is NOT AutoMock<>) | **Create** | **UnitFixture** |  ❌  | ❌  | ✅ | ❌  |
|  False (T is AutoMock<>) | **Create** | **UnitFixture** |  ✅ | ✅ | ✅ | ❌  |
|  False | **Create** | **IntegrationFixture** |  ❌ | ❌  | ❌  | ❌  |
|  False | **CreateAutoMock** |  **UnitFixture** |  ✅ |❌  | ✅ | ❌ |
|  False | **CreateAutoMock** |  **IntegrationFixture** |  ✅ | ❌  | ❌  | ❌  |

## Rereiving objects and mocks from the fixture

Note that in `LazySame` mode an object might not show up because it has not yet been generated, here are some options, but be careful making sure it works for you:
	- Change to Eager
	- Use the option of Freeze in the retrivel method
	- Call the specific property/method to make sure it has been generated, only works for `LazySame` but not `LazyDifferent`

CAUTION: In `LazyDifferent` the object retrieved might not reflect the one of a specific invocation, so unless the object is frozen then make sure that both of the following are true:
	- The specific invocation has been executed already
	- Retrieve everything for the specific path and find the correct one you are looking for
	
### Get Mock by type
TODO

### Get by path
TODO (will return the mocked object always)


## Options and Customizations
TODO Remove customizations

### Constructor arguments
TODO non callbase or `ConstructorArgumentCustomization`

### SubClassCustomization

### DI services

### AutoMockTypeControl

### Method Setup Type

### Setup private getters

### Writing your own customization

## Features over AutoFixture and Moq

#### On Moq
- **Minimal setup**: You can setup methods without providing all arguments, just the ones you specifically want
- **Setup verification times**: Setup verification times along with the method setup (currently Moq only suuports to setup that it has to be called using `Verifiable`)
- **Default Interface Implmentation - no callBase**: Moq is currently using default interface implmenetations when mocking a class that has a default interface implementation and `callBase` is false (in which case it shouldn't)
- **Default Interface Implmentation - events**: Moq is currently not working correctly with default interface implementations of events
- **Default Interface Implmentation - abstract base**: Moq is currently not working correctly when the original interface is abstract and the default implementation is in an inherited interface
- **Interface ReImplmentation - with late binding**: When a class implements an interface then in Moq when creating the `Mock` via late binding (such as generic on the interface) and also calling `As<that interface>` it will not call the original implementation even if the base is not virtual
- **Generic constrains**: Provide matchers for generics with constraints
- **Ignore ctor**: When `CallBase = false` it does not call any constructors on the object
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
- **Explicit interface implementation**: Sets up explictly implemented interface members when `CallBase` is false
- **Can register a derived class to replace the original request**: Either replace a concrete class with a subclass (via `SubClassCustomization` or `SubClassTransformCustomization`) or an open generic class (via `SubClassOpenGenericCustomization` or `SubClassTransformCustomization`), can be useful to replace for example `DbSet<>` with a dervied class for any concrete instance of `DbSet<>`

- Sets up `GetHashCode` and `.Equals` to work correctly even when `CallBase` is false
- Defaults to `CallBase = false` however for Relay obejcts it sets `CallBase = true`, but there is an option to pass in the `Createxxx` calls to change the default for non relays
- When `CallBase = true` it does not setup implemented methods (i.e. not in an interface and the method is not abstract)

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
