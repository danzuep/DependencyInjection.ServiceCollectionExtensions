# Constitution for fluent service registration

## Core Principles

To support developer ergonomics while preserving the "You Aren't Gonna Need It" principle, an optional, narrowly-scoped extension that implements a fluent registration syntax on top of Microsoft.Extensions.DependencyInjection.ServiceCollection.

### Goals
- Provide a thin, opt-in fluent API that maps to ServiceCollection semantics (transient/scoped/singleton, constructor injection, named/metadata-less resolution where possible).
- Avoid adding features or behaviors that are not supported by the underlying Dependency Injection framework.
- Keep the implementation small, easily auditable, and fully testable with existing CI gates.

### Non-goals
- Do NOT introduce reflection beyond what is strictly necessary for ergonomics.

### Design constraints
- Surface: The fluent API MUST be opt-in (explicit package or namespace) and clearly documented as syntactic sugar over ServiceCollection. Public surface area MUST be minimal.
- Implementation: The extension MUST translate fluent calls to equivalent IServiceCollection registrations. Any behavior that cannot be cleanly mapped MUST be refused or exposed only behind an explicit opt-in flag documented in plan.md.
- Observability: The extension MUST provide minimal diagnostic hooks (e.g., ability to enumerate registrations, clear mapping between fluent call and final ServiceDescriptor) to help users reason about registrations in production.
- Dependencies: The extension MUST avoid adding large dependency trees; any additional dependency requires a plan.md justification and vetting per the Minimal Dependencies rules.

### Testing & compatibility
- Tests: Unit tests MUST cover fluent-to-ServiceCollection translation for the public API. Integration tests MUST validate expected resolution semantics within Host/GenericHost scenarios and common DI usage patterns.
- Compatibility: The extension MUST target supported platform versions declared in plan.md. It SHOULD be compatible with Microsoft.Extensions.DependencyInjection behavior and common extension patterns (e.g., AddXxx overloads).

### Plan requirements
Any project that implements this extension MUST include a plan.md containing:
- Motivation and user stories (why ergonomics are needed and what problems are solved).
- Explicit non-goals and decision log items that rejected unnecessary complexity.
- API surface listing: examples for common registrations (service -> implementation, open generics, decorators if supported, named registration alternatives) and how to map them to IServiceCollection calls.
- Security checklist and compatibility matrix for host runtimes and target frameworks.
- Testing strategy: unit + integration + CI gates and sample tests demonstrating failure-before-implementation (test-first encouraged).
- Observability & diagnostics plan: how to inspect registrations at runtime and how logs surface mapping.
- Dependency justification for any added packages and pinning strategy.

### Development workflow & CI
- PRs implementing this extension MUST include the plan.md, comprehensive unit tests validating translation, integration tests with Host scenarios, and examples in README/quickstart.
- Linting and static analysis MUST run in CI. Code size and complexity metrics (e.g., cyclomatic complexity) SHOULD be considered in review to enforce minimality.
- Reviews: The change MUST be reviewed by at least two maintainers; any API additions that broaden the surface require an explicit documented rationale and a second maintainer approval.

### Governance & versioning
- Because the extension introduces a specific developer-facing API, add it as a MINOR change to the constitution when the policy text above is added or materially expanded.
- API-breaking changes to the extension MUST follow the Governance rules: MAJOR bump for breaking changes, MINOR for new optional features, PATCH for clarifications.
- Periodic review: The extension and its plan.md MUST be reviewed annually alongside the constitution to ensure it still satisfies YAGNI and minimal dependency goals.

### Rationale
This policy keeps developer ergonomics achievable while preventing scope creep that would re-create a separate DI runtime. It enforces minimality, transparency, and traceability so the community can accept an ergonomic fluent surface without compromising the constitution's simplicity and low-dependency commitments.
