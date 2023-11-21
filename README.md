# 🛠 Package Development

**NOTE: This branch is for development purposes only.**

## Develop the Package with the `develop` Branch

1. Fork the repository.
2. Clone the repository.
3. Checkout `develop` branch.
4. Develop the package.
5. Test the package with the test runner (`Window > Generals > Test Runner`).
6. Commit with a message based
   on [Angular Commit Message Conventions](https://gist.github.com/stephenparish/9941e89d80e2bc58a153) as follows:

    - `fix:` fix a bug
    - `feat:` new feature
    - `docs:` changes only in documentation
    - `style:` changes only in formatting, white-space, etc
    - `refactor:` changes only in code structure (extract method, rename variable, move method, etc)
    - `perf:` changes only in code performance
    - `test:` add or update tests
    - `chore:` changes to the build process or auxiliary tools and libraries such as documentation generation

7. Create a pull request on GitHub. Fill out the description, link any related issues and submit your pull request.

For details, refer to [CONTRIBUTING](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/main/CONTRIBUTING.md)
and [CODE_OF_CONDUCT](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/main/CODE_OF_CONDUCT.md).

## How to Release This Package

When you push to the `preview`, `release`, or `v1.x` branch, this package is automatically released by GitHub Action.

[Semantic Release](https://semantic-release.gitbook.io/semantic-release/) is used for automatic release.

- Update the version in `package.json`.
- Update `CHANGELOG.md`.
- Commit documents and push.
- Update and tag the UPM branch.
- Release on GitHub.
- ~~Publish npm registry~~

Alternatively, you can release it manually with the following command:

```bash
$ cd Packages/src
$ npm run release -- --no-ci
```