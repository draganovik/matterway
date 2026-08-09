# Documentation

This directory contains project diagrams and brand source files.

## Structure

- `diagrams/` contains editable PlantUML (`.puml`) diagram sources.
- `diagrams/exports/` contains PNG exports generated from those diagram sources.
- `screenshots/` contains examples screenshots.
- `brand/` contains editable brand source files.

Run `scripts/export-diagrams` from the repository root after changing PlantUML sources.
Single-page diagrams retain the same base filename as their source. Multi-page
sequence diagrams use numbered page suffixes, for example
`sequence-web-auth-01.png`, `sequence-web-auth-02.png`, and so on.
