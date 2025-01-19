# Gitfo

A tool for managing multiple git repositories.

Do you develop solution that span multiple Git repositories?  Tired of manually checking the status of all of them and synchronizing them?  Gitfo is for you!

At the core, gitfo is configured with a single `.gitfo` file in a root development folder.  All repositories must be child folders of this Root.

## Requirements

- Gitfo is configured with a single `.gitfo` file in a directory referred to as Root
- Gitfo supports multiple profiles in one configuration file
- Gitfo supports repositories with different owners
- The Gitfo Root folder can have any valid folder name
- All Gitfo managed repositories shall be child folders of Root.
- Gitfo can clone missing repositories
- Gitfo can pull existing repositories
- Gitfo can fetch existing repositories
- Gitfo can output the status of all managed repositories
- Gitfo-managed repositories have a "default" branch
- The default branch can be specified per repository in the config
- An unspecified "default" branch will be `main`

## Command examples

| command | description |
| --- | --- |
| `gitfo generate` | Generate a `.gitfo` file for an existing root folder |
| `gitfo sync` | Does a `pulls` or `clone` of all repos in the default profile |
| `gitfo status` | Show the status of all repos in the default profile |
| `gitfo status -p {profile}` | Show the status of all repos in the selected profile |
| `gitfo pull` | Does a `git pull` for all repos in the default profile |
| `gitfo fetch` | Does a `git fetch` for all repos in the default profile |
| `gitfo checkout` | Does a `git checkout` for all repos in the default profile, checking out the configured default branch for each. |
| `gitfo checkout -b {branch-name}` | Does a `git checkout` for all repos in the default profile, checking out the specified branch for each (if it exists). |

** all commands support the `-p` option shown in `gitfo status above

