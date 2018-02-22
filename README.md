# IncludeChecker
Unused #includes in C++ source lead to unwanted dependencies and slower compile and link timings.

IncludeChecker finds most of these unused #includes using a simple heuristic: if you include a file, 
you need to use one or more symbols from it. If that's not the case, IncludeChecker will mark the #include as unused.

IncludeChecker is very customizable to be able to run as an automated task where it will fail as soon as an unused #include is detected.

Because of the simple heuristic it will not find all unused #includes, but it will find most of them.

Features:

- Scanning of single files or complete subdirectories.
- Troublesome includes can be ignored.
- Global type prefixes and suffixes can be configured.
- Easy to setup to run as a build check.

# Build status

![Appveyor build status](https://ci.appveyor.com/api/projects/status/38t579lkeknh1807?svg=true Appveyor build status)
