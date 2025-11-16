# Feature Specification: fluent-service-registration

**Feature Branch**: `1-fluent-service-registration`  
**Created**: 2025-11-16  
**Status**: Draft  
**Input**: User description: "Provide a thin, opt-in fluent API on top of Microsoft.Extensions.DependencyInjection.ServiceCollection to allow ergonomic service registration and stacking decorators accepting Func<T> next."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Basic binding (Priority: P1)

As a library consumer I want a concise fluent API to register a service interface to a concrete implementation so I can avoid repetitive ServiceCollection wiring but still rely on standard IServiceCollection behavior.

Why this priority: This is the core ergonomic gain and must map 1:1 to IServiceCollection semantics per the constitution.

Independent Test: Add a binding with To<TImpl>() and verify that resolving the service returns the implementation (and that transient/scoped/singleton lifetimes behave like standard registrations).

Acceptance Scenarios:

1. Given an empty IServiceCollection, When I call Bind<IService>().To<ServiceImpl>(ServiceLifetime.Transient), Then provider.GetRequiredService<IService>() returns an instance of ServiceImpl.
2. Given a singleton lifetime, When I register To<ServiceImpl>(ServiceLifetime.Singleton) then multiple resolves return the same instance.

---

### User Story 2 - Single decorator (Priority: P1)

As a developer I want to declare a decorator that accepts Func<TService> next so I can wrap behavior (logging, metrics) without custom factory boilerplate.

Why this priority: Decorating is a common pattern that benefits most users; it must be supported minimally and map to ServiceCollection semantics as stated in the constitution.

Independent Test: Register Bind<IService>().To<ServiceImpl>() and Decorate<IService>().With<DecoratorA>() then resolve IService and verify the decorator runs before/after the inner implementation.

Acceptance Scenarios:

1. Given a binding to ServiceImpl and a decorator DecoratorA that logs before/after, When resolving IService, Then output includes DecoratorA before, ServiceImpl output, DecoratorA after.

---

### User Story 3 - Stacked decorators with DI in decorators (Priority: P1)

As a developer I want to stack multiple decorators and allow decorators to depend on other services (e.g., Guid) so I can build decorated pipelines with dependencies resolved from DI.

Why this priority: Real-world decorators often need DI. The fluent API must allow stacking multiple decorators and allow decorators to receive other DI-provided params.

Independent Test: Using Bind<IService>().To<ServiceImpl() and Decorate<IService>().With<DecoratorA>().With<DecoratorB>(), where DecoratorB depends on Guid, verify that resolving IService yields the pipeline B(A(ServiceImpl)) and that each resolve produces a new Guid for transient Guid registration.

Acceptance Scenarios:

1. Given transient Guid registration, binding, and decorators A then B, When resolving twice, Then each resolution shows a different Guid in DecoratorB while DecoratorA wraps as expected.

---

### Edge Cases

- What happens when no Bind<T> was called but Decorate<T> is used? (Should be documented as invalid or require explicit seed factory; plan.md must document refusal or opt-in.)
- What happens if decorator constructor does not accept Func<TService> as a first parameter? (Spec requires constructor first parameter accept Func<TService>; otherwise the design should refuse or document a clear error.)
- How are open generics handled? (Plan: disallow unless explicitly implemented and documented in plan.md per constitution rules.)
- How does lifetime interplay across inner factory and public registrations? Must match IServiceCollection semantics; plan must cover this.

## Requirements *(mandatory)*

### Functional Requirements

- FR-001: System MUST provide a WithBindings(IServiceCollection, Action<IFluentBinder>) extension to configure fluent bindings.
- FR-002: System MUST provide Bind<TService>().To<TImplementation>(ServiceLifetime) mapping to equivalent ServiceDescriptor entries.
- FR-003: System MUST provide Decorate<TService>().With<TDecorator>(ServiceLifetime) that stacks decorators which accept Func<TService> next.
- FR-004: Decorators MUST be able to depend on other services from IServiceProvider; their ctor must accept Func<TService> (or Func<TService> typed parameter) representing next.
- FR-005: The fluent API MUST be opt-in (separate namespace/package) and minimally surface only necessary methods.
- FR-006: Observability: plan MUST document how to enumerate registrations and map fluent declarations to ServiceDescriptor entries.
- FR-007: Implementation MUST avoid adding large dependencies and must justify any added packages in plan.md per Minimal Dependencies rule.

### Key Entities

- IFluentBinder: top-level binder interface used in WithBindings.
- IBindSyntax<TService>, IDecorateSyntax<TService>: fluent syntax surfaces.
- InnerFactory<TService>: internal marker storing the seed factory Func<TService> (internal implementation detail).
- ServiceCollection registrations: the final result mapping to established ServiceDescriptor entries.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- SC-001: Bind/Decorate scenarios in sample program compile and run, producing expected output with stacked decorators (B(A(ServiceImpl))).
- SC-002: Unit tests covering translation from fluent calls to ServiceDescriptor entries exist and pass.
- SC-003: Integration tests with Host/GenericHost scenarios validate resolution semantics for transient/scoped/singleton lifetimes.
- SC-004: No new large third-party dependencies added; any added dependencies documented and justified in plan.md.