# LogicBuilder.Workflow.ComponentModel.Serialization

[![Build Status](https://github.com/BpsLogicBuilder/LogicBuilder.Workflow.ComponentModel.Serialization/actions/workflows/ci.yml/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.Workflow.ComponentModel.Serialization/actions/workflows/ci.yml)
[![CodeQL](https://github.com/BpsLogicBuilder/LogicBuilder.Workflow.ComponentModel.Serialization/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.Workflow.ComponentModel.Serialization/actions/workflows/github-code-scanning/codeql)
[![codecov](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.Workflow.ComponentModel.Serialization/graph/badge.svg?token=CTUXSQYTCV)](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.Workflow.ComponentModel.Serialization)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=BpsLogicBuilder_LogicBuilder.Workflow.ComponentModel.Serialization&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=BpsLogicBuilder_LogicBuilder.Workflow.ComponentModel.Serialization)
[![NuGet](https://img.shields.io/nuget/v/LogicBuilder.Workflow.ComponentModel.Serialization.svg)](https://www.nuget.org/packages/LogicBuilder.Workflow.ComponentModel.Serialization)

## Integration with LogicBuilder.Rules

This library serves as the serialization layer for the LogicBuilder.Rules rulesets, enabling:
- Persistence of workflow rule definitions
- Loading rules from XOML files
- Runtime rule modification and storage
- Design-time rule editing support

## Architecture

The library follows the **Serialization Provider Pattern**, allowing custom types to provide their own serialization logic through:
1. Implementing `WorkflowMarkupSerializer` for custom types
2. Registering serializers via `DefaultSerializationProviderAttribute`
3. Using `WorkflowMarkupSerializationProvider` for service resolution

## Testing

Comprehensive test suite included in `Workflow.ComponentModel.Serialization.Tests` covering:
- Simple and complex property serialization
- Collection and dictionary serialization
- Markup extension processing
- Extended property handling
- Error handling and edge cases
- Round-trip serialization validation

## Contributing

Contributions are welcome.

## Related Projects

- [LogicBuilder.Workflow.Activities.Rules](https://github.com/BpsLogicBuilder/LogicBuilder.Workflow.Activities.Rules) - The workflow rules engine that uses this serialization library