# Molport Quote Importer

## How to Run the Application

1. Open the solution in Visual Studio 2022.
2. Restore the NuGet packages.
3. Build the solution.
4. Run the WPF application.
5. Select the provided Excel quote file.
6. Review the parsed data and validation results.
7. Click **Import Quote** to simulate the import process.

---

## AI Usage

AI tools (ChatGPT) were used during development to:

* review the overall project structure;
* suggest code refactoring and improve readability;
* help implement validation rules;
* improve code maintainability by identifying opportunities such as extracting constants and reducing code duplication.

All AI-generated suggestions were manually reviewed, adapted where necessary, and tested before being included in the project.

---

## What I Would Improve with More Time

* Add dependency injection.
* Add unit tests for parsing and validation logic.
* Improve error handling for invalid or corrupted Excel files.
* Make worksheet names and column mappings configurable.
* Add logging.
* Improve the UI with filtering and searching.
* Export validation results to a separate report.
