# LogicBuilder.Workflow.ComponentModel.Serialization

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

- [LogicBuilder.Rules](https://github.com/BpsLogicBuilder/LogicBuilder.Rules) - The workflow rules engine that uses this serialization library