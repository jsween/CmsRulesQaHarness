# CmsRulesQaHarness

A .NET API application for testing and validating CMS (Centers for Medicare & Medicaid Services) eligibility rules. This project provides a harness for determining program eligibility based on applicant demographics and financial information.

## Overview

The CMS Rules QA Harness evaluates applicant eligibility for various CMS programs including:
- **Medicare** - For applicants age 65+ within income thresholds
- **Medicaid** - For disabled or blind applicants within income thresholds
- **CHIP** - For pregnant applicants within income thresholds

## Project Structure

```
CmsRulesQaHarness/
├── CmsRulesQaHarness.API/          # Main API application
│   ├── Controllers/                # API endpoints
│   ├── Models/                     # Data models and enums
│   │   ├── Enums/                  # Program categories
│   │   ├── EligibilityRequest.cs   # Request model
│   │   ├── EligibilityResult.cs    # Response model
│   │   └── ValidationReasons.cs    # Validation messages
│   └── Services/                   # Business logic
│       ├── IEligibilityService.cs  # Service interface
│       └── EligibilityService.cs   # Eligibility determination logic
├── CmsRulesQaHarness.Tests/        # Unit tests
│   └── EligibilityRuleTests.cs     # Comprehensive test suite
└── README.md
```

## Technologies

- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **xUnit** - Testing framework

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026 (Community/Professional/Enterprise) or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/jsween/CmsRulesQaHarness.git
   cd CmsRulesQaHarness
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the API**
   ```bash
   dotnet run --project CmsRulesQaHarness.API
   ```

   The API will start at `https://localhost:7081`

### Running Tests

**Run all tests:**
```bash
dotnet test
```

**Run tests with coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

**Generate HTML coverage report:**
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"CmsRulesQaHarness.Tests\TestResults\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## API Usage

### Endpoint

**POST** `/api/Eligibility/determine`

### Request Body

```json
{
  "age": 67,
  "monthlyIncome": 1200,
  "isDisabled": false,
  "isPregnant": false,
  "isBlind": false,
  "householdSize": 1
}
```

### Response

```json
{
  "isEligible": true,
  "programCategory": 4,
  "reasons": [
    "Applicant is age 65 or older and within income threshold."
  ]
}
```

### Program Categories (Enum Values)

- `0` - None (not eligible)
- `1` - Invalid (validation error)
- `2` - Medicaid
- `3` - CHIP
- `4` - Medicare
- `5` - DualEligible

### Testing with HTTP File

The project includes an HTTP file for easy testing:

1. Open `CmsRulesQaHarness.API/EligibilityRequests.http` in Visual Studio
2. Click "Send Request" to test the endpoint

## Eligibility Rules

### Income Thresholds

| Program  | Condition          | Max Monthly Income |
|----------|--------------------|--------------------|
| Medicare | Age >= 65          | $1,600             |
| Medicaid | Disabled           | $1,800             |
| CHIP     | Pregnant           | $2,000             |
| Medicaid | Blind              | $2,200             |

### Rule Priority

Rules are evaluated in the following order (first match wins):
1. **Age 65+** - Medicare eligibility
2. **Disabled** - Medicaid eligibility
3. **Pregnant** - CHIP eligibility
4. **Blind** - Medicaid eligibility

### Validation Rules

- Age must be non-negative
- Monthly income must be non-negative
- Household size must be greater than zero

## Test Coverage

The test suite includes **24 comprehensive test cases** organized into:

### Positive Test Cases
- Aged applicants within income limits
- Disabled applicants within income limits
- Pregnant applicants within income limits
- Blind applicants within income limits

### Negative Test Cases
- Applicants with income too high
- Invalid input validation (negative values)
- Multiple conditions with income over all limits

### Edge Case Tests
- Exact income limit boundaries
- One dollar over/under income limits
- Age boundary tests (64 vs 65)
- Multiple condition combinations
- Rule priority validation
- Large household sizes

## Development

### Branch Strategy

- `master` - Main branch
- `feat/*` - Feature branches
- `tests/*` - Test-specific branches

### Code Standards

- XML documentation comments on all public classes and interfaces
- Self-documenting property names (no property-level comments needed)
- Regions used in test files to organize test categories
- Consistent naming conventions following .NET guidelines

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feat/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feat/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Author

- **jsween** - [GitHub Profile](https://github.com/jsween)

## Acknowledgments

- CMS program rules are simplified for demonstration purposes
- This is a QA harness for testing eligibility logic, not production CMS software