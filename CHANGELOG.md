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
