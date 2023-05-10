# ![wemogy logo](https://wemogyimages.blob.core.windows.net/logos/wemogy-github-tiny.png) Next Version (GitHub Action)

A GitHub Action for determining the version for the next release based on branch name and existing releases. This Action expects, that you use [Semantic Versioning](https://semver.org/) for your project

#### Single Project Repository

If your repository hosts a single project, this Action expects, that you manage releases in `release/vX.Y` branches, and that GitHub Releases are tagged with `vX.Y.Z`. The Action fetches all existing Github Releases form the repository and finds the highest one for the branch it is executed on. Then it returns this version's patch number increased by one.

#### Multi Project Repository

If your repository hosts more than one project and these projects have individual lifecycles, this Action expects, that these projects are located in different folders on root level and that the releases are managed in `release/folder/vX.Y` branches, where `folder` matches the exact folder name of the project. The task then finds the latest matching GitHub Release with a `folder-vX.Y.Z` tag and increases that number.

Check the examples below for more details.

## Usage

```yaml
- uses: wemogy/next-version-action@2.0.2
  id: release-version
  with:
    token: ${{ secrets.GITHUB_TOKEN }}
    projects: 'Single'

- run: echo ${{ steps.release-version.outputs.next-version }}
```

## Inputs

| Input | Description |
|-|-|
| `token` | **Required** A GitHub Access Token |
| `repo` | The repository name (Default: current repository) |
| `username` | The GitHub username (Default: current repository owner) |
| `branch` | The release branch to check (Default: current branch) |
| `projects` | The amount of projects in this repo (Single or Multi) (Default: Single) |
| `prefix` | A prefix to all versions and release branches (Default: v) |

## Outputs

| Output | Description |
|-|-|
| `next-version` | The next semantic version for the next release without prefix |
| `next-version-name` | The next semantic version for the next release with prefix |
| `folder` | The name of the folder for the branch |

## Examples

### Single Project Repository

Given your repo has GitHub Releases with the following tags:

- `v1.0.0`
- `v1.1.0`
- `v1.1.1`
- `v1.2.0`

and the following branches:

- `main`
- `release/v1.0`
- `release/v1.1`
- `release/v1.2`

then a task with the configuration below generates the following outputs:

```yaml
- uses: wemogy/next-release-version-action@1.0.1
  id: release-version
  with:
    token: ${{ secrets.GITHUB_TOKEN }}
    branch: 'release/v1.1'

- run: echo ${{ steps.release-version.outputs.next-version }} # Output: 1.1.2
- run: echo ${{ steps.release-version.outputs.next-version-name }} # Output: v1.1.2
- run: echo ${{ steps.release-version.outputs.folder }} # Output: <empty>
```

In this example, the Action identifies version `1.1.1` as the hightest one for the `release/v1.1` tag. So the next patch version would be `1.1.2` and get the tag `v1.1.2`.

### Multi Project Repository

Given your repo contains multiple projects in the following folders:

- `/project-a`
- `/project-b`

and GitHub Releases with the following tags:

- `project-a-v1.0.0`
- `project-a-v1.1.0`
- `project-b-v1.0.0`

and the following branches:

- `main`
- `release/project-a/v1.0`
- `release/project-a/v1.1`
- `release/project-b/v1.0`

then a task with the configuration below generates the following outputs:

```yaml
- uses: wemogy/next-release-version-action@1.0.1
  id: release-version
  with:
    token: ${{ secrets.GITHUB_TOKEN }}
    branch: 'release/project-a/v1.1'
    projects: 'Multi'

- run: echo ${{ steps.release-version.outputs.next-version }} # Output: 1.1.1
- run: echo ${{ steps.release-version.outputs.next-version-name }} # Output: v1.1.1
- run: echo ${{ steps.release-version.outputs.folder }} # Output: project-a
```

In this example, the Action identifies version `1.1.0` as the hightest one for the `release/project-a/1.1` tag. So the next patch version would be `1.1.1` and get the tag `project-a-v1.1.2`.

### Prefix

By default, this Action prepends a `v` to the version name and tags and expects that the release branches are named `release/vX.Y` or `release/project-a/vX.Y` with a `v` before the version number. You can adjust this version prefix with the `prefix` input.

```yaml
- uses: wemogy/next-release-version-action@1.0.1
  id: release-version
  with:
    token: ${{ secrets.GITHUB_TOKEN }}
    branch: 'release/1.1'
    projects: 'Single'
    prefix: '' # <- omit the prefix

- run: echo ${{ steps.release-version.outputs.next-version }} # Output: 1.1.1
- run: echo ${{ steps.release-version.outputs.next-version-name }} # Output: 1.1.1
```
