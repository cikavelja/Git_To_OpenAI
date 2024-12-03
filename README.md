
# OpenAI Code Review Summary for GitHub Project

This project demonstrates a Proof of Concept (POC) that integrates with OpenAI to generate code review summaries for each file in a GitHub project, categorized by file extension.

## Purpose

The goal of this project is to automate the process of code reviews by querying OpenAI for insights and summaries on the code within each file of a given GitHub repository. The code is analyzed based on its file extension to provide relevant feedback tailored to the specific programming language or framework.

## Features

- **GitHub Integration**: Fetches project files from a GitHub repository.
- **File Extension Categorization**: The analysis is performed separately for each file type, with OpenAI generating relevant reviews based on the language and file extension.
- **OpenAI Code Summarization**: Uses OpenAI API to provide code review summaries, highlighting areas of improvement and potential bugs.

## Setup

### Prerequisites

- A GitHub repository to analyze.
- An OpenAI API key for interacting with the OpenAI service.

### Installation

1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/cikavelja/Git_To_OpenAI.git
   ```

2. Open project in VisualStudio 2022:


3. Set up environment variables:
  [OpenAI Key](https://github.com/cikavelja/Git_To_OpenAI/blob/master/Git_To_OpenAI.Api/Program.cs#L88)


4. How to video:
  [YouTube](https://youtu.be/KLgB3E7RZh8?si=Uh6k-XuxUhb-chGd)

### Configuration

Modify the configuration file to specify your GitHub repository and any specific file extensions you'd like to focus on. The system will pull files from the repository, filter by the provided extensions, and send them to OpenAI for review.

## Usage

1. Run the application to start querying OpenAI for code review summaries:
   ```bash
   npm start
   ```

2. The application will fetch files from your GitHub project, analyze them, and print out the OpenAI-generated summaries for each file type.

## Example Output

For each file in the GitHub repository, the output might look like this:

```txt
File: src/main.py
Summary: This Python file contains a function that calculates the sum of an array. Consider adding type hints for better code readability.

File: src/utils.js
Summary: The JavaScript utility file is concise, but you might want to check for edge cases when handling input values.
```

## Contributing

Feel free to fork this project, submit issues, or open pull requests. Contributions are welcome!

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
