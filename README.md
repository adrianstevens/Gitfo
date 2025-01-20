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

## Configuring `gitfo`

The best way to explain `gitfo` configuration is with an example.

Let's say you work on multiple repositories for multiple projects.  You want to on-board a new development machine (or a new developer) and you want to simplify pulling all of the repositories needed.

Lets assume that the repository tree looks like the following, with the first level of directories being the repo owner, and the next level being the repos themselves.

```
root/
├── owner-a/
│   ├── public-repo-1/
│   └── public-repo-2/
├── owner-b/
│   ├── public-repo-3/
│   └── public-repo-4/
└── owner-c/
    ├── public-repo-5/
    ├── private-repo-6/
    └── public-repo-7/
```

Let's say Project1 uses `owner-a` and `owner-b` repos, and Project2 uses `owner-a` and `owner-c` repos.  For this example, we'll jsut assume "all" repos, but using just some repos from each owner of each is fully supported.

You could create a `.gitfo` config file that looks like the one below.

```
{
  "profiles": {
    "project1": {
      "repos": [
        {
          "owner": "owner-a",
          "repo": "public-repo-1",
          "defaultBranch": "develop",
          "folder": "a"
        },
        {
          "owner": "owner-a",
          "repo": "public-repo-2",
          "defaultBranch": "develop",
          "folder": "a"
        },
        {
          "owner": "owner-b",
          "repo": "public-repo-3",
          "defaultBranch": "develop",
          "folder": "b"
        },
        {
          "owner": "owner-b",
          "repo": "public-repo-4",
          "defaultBranch": "develop",
          "folder": "b"
        }
      ]
    },
    "project2": {
      "repos": [
        {
          "owner": "owner-a",
          "repo": "public-repo-1",
          "defaultBranch": "develop",
          "folder": "a"
        },
        {
          "owner": "owner-a",
          "repo": "public-repo-2",
          "defaultBranch": "develop",
          "pat": null,
          "folder": "a"
        },
        {
          "owner": "owner-c",
          "repo": "public-repo-5",
          "defaultBranch": "main",
          "folder": "c"
        },
        {
          "owner": "owner-c",
          "repo": "public-repo-6",
          "defaultBranch": "main",
          "pat": "[your-access-token]",
          "folder": "c"
        },
        {
          "owner": "owner-c",
          "repo": "public-repo-7",
          "defaultBranch": "main",
          "folder": "c"
        }
      ]
    }
  }
}
```

Let's look at what this config means.

- You can do a `sync`/`pull`/`status`/etc on either project individually.  So `git status -p project1` would report the status of all repos for just `owner-a` and `owner-b` but not `owner-c`.
- All of `owner-a`'s repositories will be pulled to a folder named just `a` as an aid for you to keep directories clean.  Similar for `owner-b` and `owner-c`.
- The repos in `a` and `b` will all be pulled with a branch called `develop`, but the repos for `c` will pull `main`
- `owner-c/private-repo-6` uses a personal access token to connect

So to configure a new machine you simply:
- Create a single empty directory in the file system wherever you'd like
- Create a `.gitfo` file in that folder
- Navigate to that folder in a command window
- Run `gitfo sync -p project1` and then `gitfo sync -p project2` 
- All repositories will now be cloned