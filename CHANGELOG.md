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
