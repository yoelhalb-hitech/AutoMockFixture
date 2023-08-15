## [1.1.5] - 2023-08-15

### Added
- Add support for transforming a base class to a dervied class via the `SubClassTransformCustomization`, `SubClassCustomization` and `SubClassOpenGenericCustomization` customizations
- Add Changelog file

### Changed
- Change `FreezeCustomization` to public (this way it should be usable from the `UnitAutoDataAttribute` and `IntegrationAutoDataAttribute`, however currently it will require subclassing it to use it with the attributes)

### Fixed
- Fix regression with the `EnumerableBuilder` for a custom `IEnumerable<>` whcih is not abstract


## [1.1.4] - 2023-08-14

### Added
- Add support for ICustomization in the `UnitAutData` and `IntegrationAutoData` attributes
- Add Changelog file

### Fixed
- Fix issue with `fixture.GetAutoMock` and `.Verify` that caused it to throw
