```sh
# uv tool install specify-cli --from git+https://github.com/github/spec-kit.git # --force
# uv tool update-shell
# specify check
specify init $projectName; cd $projectName; gh repo create --source=. --private --description;
git remote add origin "https://github.com/danzuep/$projectName.git"; git branch -M main; git push -u origin main
```

<!-- Define guardrails for the project; non-negotiable principles that must be respected at all times. -->
/speckit.constitution Create a concise, technology‑agnostic set of high‑level principles that follow SOLID, KISS, DRY, YAGNI, and minimal dependencies.

<!-- Build a specification document based on the user prompt. -->
/speckit.specify Produce a concise specification for fluent dependency injection.

<!-- Create a technical plan for the specification. -->
/speckit.plan Produce a development plan and implementation blueprint for fluent dependency injection.

<!-- Break down the technical plan and the spec into a set of individual tasks that a LLM can tackle. -->
/speckit.tasks Break the plan into a prioritized list of granular tasks grouped by milestones: project scaffolding, core library, CLI, testing & CI, demo & docs. For each task include a short title, work description, estimated complexity (S/M/L) with rough time estimate, dependencies, acceptance criteria (pass/fail), and suggested owner/role. Provide 3–5 tasks per milestone, prioritize them, and produce an initial 2‑week sprint backlog (epics → stories → tickets) with the sprint backlog items identified. Acceptance: tasks are granular and actionable, prioritized and assigned to milestones, and an initial 2‑week sprint backlog is identified.