# Contributing

## Commit messages

This repository follows [Conventional Commits](https://www.conventionalcommits.org/). The format matters here specifically
because commit history drives automated release notes ([git-cliff](https://git-cliff.org/)) — the type of each commit
determines which section of the changelog it lands in, and a `!` marks a breaking change for versioning purposes.

```
<type>(<optional scope>): <description>
```

Examples:

```
feat: add device discovery
fix(zigbee): handle disconnect race
docs: document commit conventions
feat(api)!: change device response shape
```

Allowed types:

| Type       | Use for |
|------------|---------|
| `feat`     | A new feature |
| `fix`      | A bug fix |
| `docs`     | Documentation only |
| `style`    | Formatting/whitespace, no code meaning change |
| `refactor` | Neither fixes a bug nor adds a feature |
| `perf`     | A performance improvement |
| `test`     | Adding or correcting tests |
| `build`    | Build system or packaging changes |
| `ci`       | CI configuration changes |
| `chore`    | Anything else that doesn't modify src or test files |
| `revert`   | Reverts a previous commit |

The scope is optional and should name the affected area (e.g. `zigbee`, `web-host`, `site`, `utilities`).

A `!` immediately before the colon (optionally after the scope) marks a breaking change, e.g. `feat(api)!: ...`.

This is enforced by a Husky.Net `commit-msg` git hook (`.husky/commit-msg`), which rejects commits whose message doesn't
match this format. Git-generated `Merge` and `Revert "..."` commit messages are exempt.

## Formatting

CSharpier and `dotnet format` run automatically on staged `*.cs` files via a Husky.Net `pre-commit` hook. Don't bypass it.
