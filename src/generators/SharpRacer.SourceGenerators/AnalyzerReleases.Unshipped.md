### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SR1010 | SharpRacer.InputFiles | Error | AdditionalTextContentReadError
SR1011 | SharpRacer.InputFiles | Error | AdditionalTextFileReadException
SR1100 | SharpRacer.InputFiles | Error | TelemetryVariablesFileNotFound
SR1101 | SharpRacer.InputFiles | Error | IncludedVariablesFileNotFound
SR1110 | SharpRacer.InputFiles | Error | AmbiguousTelemetryVariablesFileName
SR1111 | SharpRacer.InputFiles | Error | AmbiguousIncludedVariablesFileName
SR1112 | SharpRacer.InputFiles | Error | AmbiguousVariableOptionsFileName
SR2101 | SharpRacer.Syntax | Warning | DescriptorClassMustBeDeclaredPartial
SR2102 | SharpRacer.Syntax | Warning | DescriptorClassMustBeDeclaredStatic
SR2103 | SharpRacer.Syntax | Error | DescriptorClassAlreadyExistsInAssembly
SR2201 | SharpRacer.Syntax | Warning | ContextClassMustBeDeclaredPartial
SR2202 | SharpRacer.Syntax | Warning | ContextClassMustNotBeDeclaredStatic
SR2203 | SharpRacer.Syntax | Warning | ContextClassMustInheritIDataVariablesContextInterface
SR3100 | SharpRacer.Validation | Warning | TelemetryVariablesFileContainsNoVariables
SR3101 | SharpRacer.Validation | Error | TelemetryVariableAlreadyDefined
SR3102 | SharpRacer.Validation | Warning | DeprecatingVariableNotFound
SR3210 | SharpRacer.Validation | Error | VariableOptionsFileContainsDuplicateKey
SR3211 | SharpRacer.Validation | Error | VariableOptionsFileContainsDuplicateVariableName
SR3212 | SharpRacer.Validation | Error | VariableOptionsFileContainsDuplicateClassName
SR3300 | SharpRacer.Validation | Warning | IncludedVariablesFileContainsNoVariables
SR3301 | SharpRacer.Validation | Warning | IncludedVariablesFileAlreadyIncludesVariable
SR3302 | SharpRacer.Validation | Warning | IncludedVariablesFileContainsEmptyVariableName
SR3402 | SharpRacer.Validation | Error | DescriptorNameConflictsWithExistingVariable
SR3502 | SharpRacer.Validation | Error | ContextClassIncludedVariableNotFound
SR3503 | SharpRacer.Validation | Error | ContextClassVariableNameCreatesPropertyNameConflict
SR3504 | SharpRacer.Validation | Error | ContextClassConfiguredPropertyNameConflict