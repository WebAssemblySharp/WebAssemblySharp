# WebAssemblySharp ![example workflow](https://github.com/WebAssemblySharp/WebAssemblySharp/actions/workflows/dotnet.yml/badge.svg)

**WebAssemblySharp** is an emerging WebAssembly runtime for C#, designed to enable the execution of WebAssembly modules within C# applications. This project is currently under active development.

## Overview

WebAssemblySharp aims to provide a seamless integration of WebAssembly (Wasm) into the C# ecosystem, allowing developers to run Wasm modules directly from their C# codebases. This integration facilitates the execution of code across different platforms and languages, leveraging the performance and security benefits inherent to WebAssembly.

## Features

- **Wasm Module Execution**: Load and execute WebAssembly modules within C# applications.

## Getting Started

As WebAssemblySharp is in the development phase, the following instructions are subject to change. Please refer to the project's GitHub repository for the most current information.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/WebAssemblySharp/WebAssemblySharp.git
   ```

2. **Navigate to the Project Directory**:

   ```bash
   cd WebAssemblySharp
   ```

3. **Restore Dependencies**:

   ```bash
   dotnet restore
   ```

4. **Build the Solution**:

   ```bash
   dotnet build
   ```

### Usage

To run the test project:

```bash
   dotnet run --project WebAssemblySharpTest
```

This command executes the test suite, demonstrating the current capabilities of WebAssemblySharp.


### Nightly Performance Tests

We execute performance tests every night to monitor the ongoing performance improvements of WebAssemblySharp. The results of these tests are publicly available for review. You can check the latest performance metrics, including memory and execution time benchmarks, at the following URL:

[WebAssemblySharp Performance Results](https://webassemblysharp.github.io/WebAssemblySharp/Pages/Benchmark/)

## Contributing

Contributions to WebAssemblySharp are welcome. To get involved:

1. **Fork the Repository**: Create a personal copy of the project.
2. **Create a Feature Branch**: Develop your feature or fix in a dedicated branch.
3. **Commit Your Changes**: Ensure your commits are descriptive and concise.
4. **Push to Your Fork**: Upload your changes to your forked repository.
5. **Submit a Pull Request**: Propose your changes for integration into the main project.

Please ensure that your contributions align with the project's coding standards and include appropriate tests.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

We extend our gratitude to the open-source community and projects that have inspired and supported the development of WebAssemblySharp.

---

*Note: WebAssemblySharp is currently in development. Features and instructions may evolve as the project progresses.

