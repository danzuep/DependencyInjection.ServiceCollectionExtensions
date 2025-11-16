# Tasks: fluent-service-registration

**Input**: Design documents from `/specs/1-fluent-service-registration/`  
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Unit and integration tests are REQUIRED as per constitution.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

### Constitution Compliance

All generated tasks MUST reference the governing constitution principles where relevant:
- Tests tasks: tag as Code Quality
- Module boundary/refactor tasks: tag as Modularity
- Diagnostics tasks: tag as Observability

## Path Conventions

Single project: `src/`, `tests/` at repository root

## Phase 1: Setup (Shared Infrastructure)

- [ ] T001 Create project structure per implementation plan: create `src/FluentDependencyInjection/` and `tests/` directories (Modularity)
- [ ] T002 Initialize .NET class library and test projects (xUnit) with Microsoft.Extensions.DependencyInjection dependency (Minimal Dependencies)
- [ ] T003 [P] Configure linting (Roslyn analyzers) and formatting tools (dotnet format) (Code Quality)

---

## Phase 2: Foundational (Blocking Prerequisites)

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T004 Implement public API files:
  - `src/FluentDependencyInjection/ServiceCollectionExtensions.cs` (WithBindings extension) (Modularity)
  - `src/FluentDependencyInjection/IFluentBinder.cs`
  - `src/FluentDependencyInjection/IBindSyntax.cs`
  - `src/FluentDependencyInjection/IDecorateSyntax.cs`
- [ ] T005 Implement internal binder classes:
  - `src/FluentDependencyInjection/Binder/FluentBinder.cs`
  - `src/FluentDependencyInjection/Binder/BindSyntax.cs`
  - `src/FluentDependencyInjection/Binder/DecorateSyntax.cs`
  - `src/FluentDependencyInjection/Internal/InnerFactory{T}.cs` (Modularity)
- [ ] T006 [P] Add sample demo program under `samples/` reproducing the provided prototype and quickstart (Simple UX)
- [ ] T007 Configure CI: run unit + integration tests, static analysis, and complexity metrics (Code Quality)

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Basic binding (Priority: P1) 🎯 MVP

**Goal**: Implement Bind<TService>().To<TImplementation>() mapping to ServiceCollection registrations and unit tests validating translation.

**Independent Test**: Unit test that a binding produces ServiceDescriptor entries equivalent to manual registrations.

### Tests for User Story 1 (required) ⚠️ (Code Quality)
- [ ] T010 [P] [US1] Unit test: BindingsTranslationTests.cs in `tests/unit/BindingsTranslationTests.cs` verifying ServiceDescriptor count, ServiceType, ImplementationFactory semantics.
- [ ] T011 [P] [US1] Integration test: LifetimeBehaviorTests.cs in `tests/integration/LifetimeBehaviorTests.cs` verifying transient/scoped/singleton semantics under HostBuilder.

### Implementation for User Story 1
- [ ] T012 [P] [US1] Implement BindSyntax<TService>.To<TImplementation>() in `src/FluentDependencyInjection/Binder/BindSyntax.cs` (Modularity)
- [ ] T013 [US1] Add internal InnerFactory<TService> marker type in `src/FluentDependencyInjection/Internal/InnerFactory{T}.cs` (Modularity)
- [ ] T014 [US1] Quickstart example update `specs/1-fluent-service-registration/quickstart.md` demonstrating simple binding (Simple UX)
- [ ] T015 [US1] Add unit tests verifying provider resolves underlying implementation when no decorators present (Code Quality)

**Checkpoint**: User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Single decorator (Priority: P1)

**Goal**: Implement Decorate<TService>.With<TDecorator>() and validate decorator invocation wraps the seed implementation.

**Independent Test**: Unit test that a decorator wraps the seed; integration test that resolving IService produces decorated behavior.

### Tests for User Story 2 (required) ⚠️ (Code Quality)
- [ ] T018 [P] [US2] Unit test: DecoratorStackingTests.cs in `tests/unit/DecoratorStackingTests.cs` verifying output ordering and type composition.
- [ ] T019 [P] [US2] Integration test: HostResolutionTests.cs in `tests/integration/HostResolutionTests.cs` verifying decorator resolved via Host.

### Implementation for User Story 2
- [ ] T020 [P] [US2] Implement DecorateSyntax<TService>.With<TDecorator>() in `src/FluentDependencyInjection/Binder/DecorateSyntax.cs` (Modularity)
- [ ] T021 [US2] Implement wrapping logic that updates InnerFactory<TService> and public factory registration (Observability)
- [ ] T022 [US2] Add runtime checks and clear exception messages for invalid decorators (e.g., missing Func<TService> ctor param) (Simple UX, Security)
- [ ] T023 [US2] Add quickstart example in `specs/1-fluent-service-registration/quickstart.md` showing a decorator.

**Checkpoint**: User Story 2 functional and testable independently

---

## Phase 5: User Story 3 - Stacked decorators with DI in decorators (Priority: P1)

**Goal**: Support stacking multiple decorators and decorators depending on other DI services (e.g., Guid).

**Independent Test**: Integration test that registers Guid transient, stacks decorators A then B, and verifies each resolve produces new Guid value inside DecoratorB.

### Tests for User Story 3 (required) ⚠️ (Code Quality, Observability)
- [ ] T024 [P] [US3] Unit test: StackedDecoratorsTests.cs verifying decorator stack order and that ActivatorUtilities is used to provide other ctor deps.
- [ ] T025 [P] [US3] Integration test: Lifetime and per-resolve behavior test in `tests/integration/LifetimeBehaviorTests.cs`.

### Implementation for User Story 3
- [ ] T026 [P] [US3] Ensure DecorateSyntax wrapping logic uses ActivatorUtilities.CreateInstance(provider, decoratorType, previousFactory) to pass next and let DI supply other params (Modularity)
- [ ] T027 [US3] Document lifetime interaction and add tests for transient decorator behavior (Simple UX)
- [ ] T028 [US3] Add logging at Debug level when wrapping occurs to aid diagnostics (Observability)

**Checkpoint**: All user stories 1-3 should be independently functional

---

## Phase N: Polish & Cross-Cutting Concerns

- [ ] TXXX [P] Documentation updates in docs/ and README (Simple UX)
- [ ] TXXX Code cleanup and refactoring to keep minimal surface area (Modularity)
- [ ] TXXX Performance microbenchmarks (if requested) (Performance)
- [ ] TXXX [P] Additional unit tests in tests/unit/ (Code Quality)
- [ ] TXXX Security hardening and public API review (Security)
- [ ] TXXX Run quickstart.md validation and sample app (Simple UX)

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1) → Foundational (Phase 2) → User Stories (Phase 3+) → Polish

### Within Each User Story

- Tests MUST be written and fail before implementation (Code Quality)
- Models before services, services before public factory code, core implementation before integration

### Parallel Opportunities

- Setup and Foundational [P] tasks can run in parallel
- User stories are independent after foundational phase