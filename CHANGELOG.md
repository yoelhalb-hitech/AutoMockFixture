## [8.0.0] - 2024-08-09

### Breaking changes
- Overhauled the CallBase system. See README for details.
- Revised the Freeze system. Refer to README for new behavior.
- IntegrationFixture now defaults to transform based on DI attributes in DotNetPowerExtensions.
- The path for setting up a mock by path will use c# keyword strings for the built in types (i.e. string instead of String, int instead of Int32, int[] instead of Int32[])

### Added
- Option to transform to subclasses based on DI attributes in DotNetPowerExtensions via AutoTransformBySericeAttributes on the fixture.
- Option in `.Freeze()` to specify callBase on the returned object
- Add methods `ItIs.DefaultValues<T>()` and `ItIs.False()`/`ItIs.True()`

### Changed
- FreezeCustomization and SubclassCustomization (all variants) are now removable.
- Updated behavior differences between UnitFixture and IntegrationFixture.
- Optimized virtual read-write properties creation when not CallBase.
- For nullable value types a default on the inside type (such as default(int) instead of default(int?)) will also setup everything

### Removed
- NoConfigureMembers option from individual requests.

### Fixed
- Various bug fixes and optimizations.
- Fixed creating a reflection object (such as Type, MethodInfo etc.)
- Fixed creating value types as well as freezing value types

## [7.0.1] - 2024-07-26

### Added
- New attribute overloads on `UnitAutoData`/`IntegrationAutoData` for freezing types

### Fixed
- The generic versions of `UnitAutoData`/`IntegrationAutoData` should also have all ctor overloads fo the non generic versions

## [7.0.0] - 2024-07-26

### Breaking change
- Will no longer support .Net Framework as it appears to cause too much headache and not working correctly anyways
- The `SubclassCustomization` type arguments will be a compile time error if the type args don't derive from each other
- The `UnitAutoDataAttribute`/`IntegrationAutoDataAttribute` will no longer take arguments `noConfigureMemebrs`/`generateDelegates` and instead will take `callBase`
- `CreateNonAutoMock()` (or `CreateAutoMock()` when called on the `IntegrationFixture`) will always respect the `callBase` setting if explictly provided (in the call or on the fixture), the legacy behaviour of always calling base when mocking because of abstract/interface will only be used if no call base was specified at all

### Changed - possibly breaking
- Provide default implementation for many Enumerable interfaces such as `IList`/`ICollection`/`ISet` and many immutable and readonly interfaces

### Added
- Add fixture wide `CallBase` option to be able to change the default from `false` to `true`

### Fixed
- Throw detailed error for `UnitAutoData`/`IntegrationAutoData` when there is an error in the Engine
- Fix issues with Enumerable and specifically multi dimensional arrays or collections
- Fix `ValueTask` and `ValueTask<>`
- Add more doc comments

## [6.0.0] - 2024-02-08

### Breaking change
- Will no longer initialize properties/fields that are internal to microsoft assemblies

### Changed
- Fix bug with delegates
- Fix more bugs with paths

### Fixed
- Throw detailed error when requesting the mock via wrong type of mocked (in `AutoMock.Get()`)
- Use a better description for string representation of types

## [5.0.5] - 2024-02-08

### Fixed
- Fix bug setting up `.Equals` `.GetHashCode` `.ToString` when it was overriden in a base class
- Fix bugs with method paths when methods were declared in a base class

## [5.0.4] - 2024-02-06

### Added
- `IAutoMockFixture.TypesToSetupPrivateGetters` property

### Fixed
- Fix to consider the start tracker `MockShouldCallBase` setting, in particular when creating `Enumerable` mocks
- Fix bugs in handling properties/fields private getters/setters when setting up `AutoMock` properties

### Changed
- Allow creation of selaed classes with `AutoMockDependency` and `NonAutoMock`
- To setup properties/fields private/missing getters on an `AutoMock` one has to add the type containing the property/field to the `IAutoMockFixture.TypesToSetupPrivateGetters` list (and only if `!CallBase`), the default is changed to only handle private setters but not private or missing getters

### Removed
- Remove support for .Net core 2.1 as .Net has security warnings on it

## [5.0.3] - 2024-02-06

### Changed
- All names containing `Callbase`\ `callbase` will now have the `B` uppercased `CallBase`\ `callBase` (unlike till now where some were `callbase` or `Callbase`), this time all of them are actually updated (hoepfully...)

## [5.0.2] - 2024-02-06

### Added
- The `AutoMock` `Setup` methods will now support more complicated strings representing overloads
- While in general the rules are strict currently on what strings can be provided to the `AutoMock` `Setup` methods, still it will attempt to match similar methods if possible
- Add `JustFreeze` in the `Fixture` for freezing without creating

### Removed
- Remove the `NoConfigureMembers` option from the individual requests, it can only be set on the `Fixture`

### Fixed
- Fix bugs with `EnumerableBuilder`
- The generic `Create`/`CreateXXX` will throw an `InvalidOperationException` if the generic type provided is `AutoMock` and type is not mockable
- Fix issue with `MatcherGenerator` for partially constructed types
- Fix bug with explicit interface tracking paths

### Changed
- All callbase parmaeters will now be all lowercase `callbase` (unlike till now where some were `callBase`)
- Change the `UnitAutoData` and `IntegrationAutoData` not to throw any exceptions as this might crash the test runners
- Optimize virtual read-write properties (when not call base) to be created lazily (unless we use `MethodSetupType.Eager`)

## [5.0.1] - 2023-09-07

### Fixed
- Fix a bug that for properties or fields the type is a value type it didn't set values

## [5.0.0] - 2023-09-07

### Changed
- Update the `SequelPay.DotNetPowerExtensions` to version 4.0.0 which is a breaking change as the DLL names are changed

## [4.0.0] - 2023-09-06

### Added
- Added async support in the fixture

### Removed
- Remove some overloads of Fixture that are unneeded as the other overloads aready cover them

### Changed
- Change the `UnitAutoData` and `IntegrationAutoData` to be async based internally

### Fixed
- Fix bug when requesting an `IAutoMockFixture` from the fixture
- Fix completion in analyzer

## [3.0.0] - 2023-09-04

### Changed
- Avoid loophole of being able to create an `AutoMock` via `MethodInvokerWithRecursion`
- Change a constructor argment path should include the ctor as `..ctor`, the rationale is to avoid a conflict with method out argument when trying to access a ctor arg in the return type that is the same as the out arg

### Fixed
- Fix stackoveflow bug in the `LastResortBuilder` that was caused by AutoMock type requests being able to pass through
- Fix completion in analyzer when arguments are missing
- Handle better quotations in the completion analyzer

## [2.0.1] - 2023-08-31

### Changed
- Change that AutoMockFixture should never mock a Type object as it causes issues (although AutoMock can do it)

### Fixed
- Fix issues with null in the fixture
- Fix completion display and insertion
- Fix completion should also work inside the existing string (not just at the comma)
- Fix completion should only work on the correct arg

## [2.0.0] - 2023-08-30

### Changed
- Change that AutoMockFixture and AutoMockFixture.NUnit should be separate projects

### Fixed
- Fix package reference issues

## [1.1.9] - 2023-08-30

### Added
- Add analyzer for code completion

### Fixed
- Fix tracking path for out argument
- Fix issue with fixture mock tracking and retrieving

## [1.1.8] - 2023-08-17

### Added
- Add `fixture.Dispose()` to dispose all generated results and all disposable customizations
- Add that the `UnitAutoData` or `IntegrationAutoData` should automatically dispose after test run

### Fixed
- Fix issue with `fixture.Freeze()` when using `AutoMockTypeControl` together with `SubClassTransformCustomization`
- Fix the `AutoMockTypeControlAttribute` to actually be usable

## [1.1.7] - 2023-08-16

### Fixed
- Fix to respect the NonAutoMock/IntegrationFixture request to not mock dependencies
- Fix `fixture.Freeze()` to not automatically also freeze subclasses
- Fix `fixture.Freeze()` to work correctly requests

## [1.1.6] - 2023-08-15

### Added
- Add `fixture.Create()` overload with `callbase`
- Pull members from the fixture to `IAutoMockFixture`
- Add validation in `AutoMock.Get()` for when the object is itself an `AutoMock`

### Changed
- Rename `SubClassTransformCustomization` to `SubclassTransformCustomization`
- Rename `SubClassOpenGenericCustomization` to `SubclassOpenGenericCustomization`
- Rename `SubClassCustomization` to `SubclassCustomization`

### Fixed
- Fix bug with `tracker.SetResult` when there are many levels of wrapper requests
- Fix `fixture.Create()` to also handle correctly the issue of casting when using `SubClassTransformCustomization` with an `AutoMock<>`
- Fix `fixture.Freeze()` to also handle correctly the issue of casting when using `SubClassTransformCustomization` with an `AutoMock<>`
- Fix `IEnuemrable` handling when requesting an `AutoMock`

## [1.1.5] - 2023-08-15

### Added
- Add support for transforming a base class to a dervied class via the `SubClassTransformCustomization`, `SubClassCustomization` and `SubClassOpenGenericCustomization` customizations

### Changed
- Change `FreezeCustomization` to public (this way it should be usable from the `UnitAutoDataAttribute` and `IntegrationAutoDataAttribute`, however currently it will require subclassing it to use it with the attributes)

### Fixed
- Fix regression with the `EnumerableBuilder` for a custom `IEnumerable<>` which is not abstract


## [1.1.4] - 2023-08-14

### Added
- Add support for ICustomization in the `UnitAutData` and `IntegrationAutoData` attributes
- Add Changelog file

### Fixed
- Fix issue with `fixture.GetAutoMock` and `.Verify` that caused it to throw
